using AO;

public partial class PlacedCollectible : Component
{
    [Serialized] public string CollectibleType;
    [Serialized] public MyPlayer OwnerPlayer;
    [Serialized] public Plot OwnerPlot;
    [Serialized] public BrainrotModifier Modifier = BrainrotModifier.None;

    public Sprite_Renderer SpriteRenderer;
    public BrainrotCatalogEntry Entry;
    public Interactable interactable;
    
    [Serialized] public int Width = 1;
    [Serialized] public int Height = 1;
    [Serialized] public int GridX = 0;
    [Serialized] public int GridY = 0;

    // Money generation
    float lastMoneyTime = 0f;
    float moneyGenerationInterval = 1.0f; // Generate money every second
    
    // Animation state
    float pulseStartTime = -999f;
    float pulseDuration = 0.3f;
    Vector2 moneyIconOffset = Vector2.Zero;
    float moneyIconAlpha = 0f;


    public override void Awake()
    {
        SpriteRenderer = Entity.GetComponent<Sprite_Renderer>();
        Entry = BrainrotCatalog.Get(CollectibleType);
        SpriteRenderer.Sprite = Entry.Sprite;

        // Get size from database
        var size = CollectibleDatabase.GetCollectibleSize(CollectibleType);
        Width = size.width;
        Height = size.height;
        
        // Initialize money generation timer
        lastMoneyTime = Time.TimeSinceStartup + new Random().NextFloat(0, 0.75f);

        // Position the collectible centered on its tiles
        if (OwnerPlot.Alive())
        {
            Entity.Position = OwnerPlot.GetCenteredWorldPositionForCollectible(GridX, GridY, Width, Height);
            
            // Mark all tiles as occupied
            for (int dx = 0; dx < Width; dx++)
            {
                for (int dy = 0; dy < Height; dy++)
                {
                    int checkX = GridX + dx;
                    int checkY = GridY + dy;
                    if (checkX >= 0 && checkX < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(0) &&
                        checkY >= 0 && checkY < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(1))
                    {
                        OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] = Entity;
                    }
                }
            }
        }

        // Setup interactable
        interactable = Entity.GetComponent<Interactable>();
        interactable.PromptOffset = new Vector2(-0.5f, -0.5f);
        if (interactable != null)
        {
            interactable.CanUseCallback += (Player p) =>
            {
                var myPlayer = (MyPlayer)p;
                
                // Only the owner can pick it up
                if (!OwnerPlot.Alive() || !OwnerPlayer.Alive() || OwnerPlayer != myPlayer)
                    return false;
                
                // Check if player is standing on any of this collectible's tiles
                var playerPos = myPlayer.Entity.Position;
                var (playerTileX, playerTileY) = OwnerPlot.GetCollectiblesPlacementTileCoordsFromWorldPosition(playerPos);
                
                for (int dx = 0; dx < Width; dx++)
                {
                    for (int dy = 0; dy < Height; dy++)
                    {
                        int collectibleTileX = GridX + dx;
                        int collectibleTileY = GridY + dy;
                        
                        if (playerTileX == collectibleTileX && playerTileY == collectibleTileY)
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            };

            interactable.OnInteract = (Player p) =>
            {
                if (!Network.IsServer) return;

                var myPlayer = (MyPlayer)p;
                if (!myPlayer.Entity.Alive() || !OwnerPlot.Alive() || OwnerPlayer != myPlayer)
                    return;

                // Check if player has inventory space
                bool hasSpace = false;
                foreach (var item in myPlayer.DefaultInventory.Items)
                {
                    if (item == null)
                    {
                        hasSpace = true;
                        break;
                    }
                }

                if (!hasSpace)
                {
                    myPlayer.CallClient_ShowMessage("Inventory is full!", new RPCOptions() { Target = myPlayer });
                    return;
                }

                // Find the item definition for this collectible type
                var itemDef = BrainrotCatalog.GetItemDefinition(CollectibleType);
                if (itemDef == null)
                    return;

                // Create item and add to inventory
                var itemToReturn = Inventory.CreateItem(itemDef, 1);
                if (Inventory.CanMoveItemToInventory(itemToReturn, myPlayer.DefaultInventory, out var willDestroyItem))
                {
                    Inventory.MoveItemToInventory(itemToReturn, myPlayer.DefaultInventory);

                    // Clear all occupied tiles
                    if (OwnerPlot.Alive())
                    {
                        for (int dx = 0; dx < Width; dx++)
                        {
                            for (int dy = 0; dy < Height; dy++)
                            {
                                int checkX = GridX + dx;
                                int checkY = GridY + dy;
                                if (checkX >= 0 && checkX < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(0) &&
                                    checkY >= 0 && checkY < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(1))
                                {
                                    if (OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] == Entity)
                                    {
                                        OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] = default;
                                    }
                                }
                            }
                        }
                    }

                    // Despawn and destroy
                    Network.Despawn(Entity);
                    Entity.Destroy();
                    
                    // Save after picking up
                    myPlayer.SavePlacedItems();
                }
                else
                {
                    Inventory.DestroyItem(itemToReturn);
                }
            };
        }
    }

    public override void Update()
    {
        // Money generation (server only)
        if (Network.IsServer)
        {
            if (OwnerPlayer.Alive() && Time.TimeSinceStartup - lastMoneyTime >= moneyGenerationInterval)
            {
                lastMoneyTime = Time.TimeSinceStartup;
                
                long goldPerSecond = GetGoldPerSecond();
                if (goldPerSecond > 0)
                {
                    // Add money to player
                    Economy.DepositCurrency(OwnerPlayer, Config.Currency_Gold, goldPerSecond);
                    
                    // Trigger visual effect on clients
                    CallClient_PlayMoneyAnimation();
                }
            }
        }
        
        // Update interactable text
        if (Network.IsClient && interactable != null)
        {
            interactable.Text = "Pick up collectible";
        }
        
        // Update animation
        if (Network.IsClient)
        {
            UpdateMoneyAnimation();
        }
    }
    
    [ClientRpc]
    public void PlayMoneyAnimation()
    {
        pulseStartTime = Time.TimeSinceStartup;
        moneyIconOffset = Vector2.Zero;
        moneyIconAlpha = 1.0f;
    }
    
    void UpdateMoneyAnimation()
    {
        float timeSincePulse = Time.TimeSinceStartup - pulseStartTime;
        
        if (timeSincePulse > pulseDuration)
        {
            // Reset scale
            Entity.Scale = new Vector2(1f, 1f);
            moneyIconAlpha = 0f;
            return;
        }
        
        // Pulse animation
        float t = timeSincePulse / pulseDuration;
        float pulseAmount = MathF.Sin(t * MathF.PI) * 0.15f; // Pulse up to 15% larger
        Entity.Scale = new Vector2(1f + pulseAmount, 1f + pulseAmount);
        
        // Money icon rising
        moneyIconOffset = new Vector2(0, t * 1.5f); // Rise 1.5 units
        moneyIconAlpha = 1f - t; // Fade out
    }

    public override void LateUpdate()
    {
        using var world = UI.PUSH_CONTEXT(UI.Context.World);
        DrawBrainrotInfo();
        DrawMoneyIcon();
    }
    
    void DrawMoneyIcon()
    {
        if (!Network.IsClient || moneyIconAlpha <= 0f)
            return;
        
        // Position at the top of the collectible sprite
        var topPos = GetSpriteTopPosition() + moneyIconOffset;
        
        // Draw gold coin icon
        var iconSize = 0.4f;
        var halfSize = new Vector2(iconSize * 0.5f, iconSize * 0.5f);
        var iconRect = new Rect(topPos - halfSize, topPos + halfSize);
        
        var iconColor = new Vector4(1f, 1f, 1f, moneyIconAlpha);
        UI.Image(iconRect, Assets.GetAsset<Texture>(Config.Currency_GoldIcon), iconColor);
    }

    public bool ShouldDrawBrainrotInfo()
	{
        if (Network.IsServer) return false;
        if (!OwnerPlot.Alive()) return false;
        
        var localPlayerPos = Network.LocalPlayer.Entity.Position;
        
        // Convert player position to tile coordinates
        var (playerTileX, playerTileY) = OwnerPlot.GetCollectiblesPlacementTileCoordsFromWorldPosition(localPlayerPos);
        
        // Check if player is standing on any of this collectible's tiles
        for (int dx = 0; dx < Width; dx++)
        {
            for (int dy = 0; dy < Height; dy++)
            {
                int collectibleTileX = GridX + dx;
                int collectibleTileY = GridY + dy;
                
                if (playerTileX == collectibleTileX && playerTileY == collectibleTileY)
                {
                    return true;
                }
            }
        }

		return false;
	}

    public void DrawBrainrotInfo()
	{
		if (this.ShouldDrawBrainrotInfo() == false) return;

		string name = Entry.Name;
		string rarityText = GetRarityName();
		long goldPerMerge = GetGoldPerSecond();
		long price = Entry.BasePrice; // base price only

		// layout
		var (basePos, text, lineH) = GetLayout();


		var local = Network.LocalPlayer as MyPlayer;
		if (local != null)
		{
			var tempRect = new Rect(basePos).Grow(1);
			var cameraRect = Camera.GetCurrentCameraWorldRect();
			if (cameraRect.Overlaps(tempRect) == false)
			{
				//Log.Info($"BrainrotInfo not visible: {tempRect}");
				return;
			}
		}
		void Draw(Vector2 pos, string s, Vector4 color)
		{
			UI.TextAsync(new Rect(pos), s, text with { Color = color });
		}
		void DrawRainbowText(Vector2 pos, string s, float size)
		{
			UIUtils.DrawRainbowText(new Rect(pos), s, text);
		}

		using var _ = UI.PUSH_LAYER_RELATIVE(1);

		// Modifier tag above name
		if (Modifier != BrainrotModifier.None)
		{
			Draw(basePos + new Vector2(0, lineH), GetModifierName(Modifier), GetModifierColor(Modifier));
		}

		// If in room and local player close, draw XP bar while rarity can still evolve
		// if (State == BrainrotState.InRoom)
		// {

		// 	if (local != null && local.MergingRoom != null && local.MergingRoom == TargetRoom)
		// 	{
		// 		float dist = (local.Position - Entity.Position).Length;
		// 		if (dist < 2.0f)
		// 		{
		// 			DrawXpBar(BrainrotCatalog.HasEvolution(CatalogId) == false);
		// 		}
		// 	}
		// }

		// Name (white)
		Draw(basePos, name, new Vector4(1f, 1f, 1f, 1f));
		// Rarity (color by rarity)
		Draw(basePos + new Vector2(0, -lineH), rarityText, GetRarityColor(Entry.Rarity));
		// Size modifier (light blue)
		//Draw(basePos + new Vector2(0, -lineH*2f), $"x{sizeMult:0.#}", new Vector4(0.5f, 0.8f, 1f, 1f));
		//Draw(basePos + new Vector2(0, -lineH * 2f), $"{GetSizeName()}", new Vector4(0.5f, 0.8f, 1f, 1f));
		// Gold per merge (amber)
		Draw(basePos + new Vector2(0, -lineH * 2f), $"${StringUtils.FormatMoney(goldPerMerge)}/s", new Vector4(1f, 0.87f, 0.129f, 1f));
		// Price (green)
		Draw(basePos + new Vector2(0, -lineH * 3f), $"${StringUtils.FormatMoney(price)}", new Vector4(0.2f, 1f, 0.2f, 1f));
		if (Entry.IsNotStealable == true)
		{
			DrawRainbowText(basePos + new Vector2(0, -lineH * 4f), $"UNSTEALABLE", 0.2f);
			//Draw(basePos + new Vector2(0, -lineH * 4f), $"UNSTEALABLE", new Vector4(0.2f, 1f, 0.2f, 1f));
		}
	}

    public long GetGoldPerSecond()
	{
		// Gold per second depends on base species gold scaled by rarity and modifier
		double rarityScale = 1.0;
		double per = Entry.BaseGoldGeneration * rarityScale;
		
		// Apply modifier multiplier
		float modifierMultiplier = GetModifierMultiplier(Modifier);
		per *= modifierMultiplier;
		
		// Apply rebirth multiplier if player has rebirth level
		if (OwnerPlayer != null && OwnerPlayer.Entity.Alive())
		{
			float rebirthMultiplier = OwnerPlayer.GetRebirthEarningsMultiplier();
			per *= rebirthMultiplier;
		}

		return (long)System.Math.Round(per);
	}
	
	public static float GetModifierMultiplier(BrainrotModifier modifier)
	{
		return modifier switch
		{
			BrainrotModifier.Gold => Config.Modifier_Gold_Multiplier,
			BrainrotModifier.Diamond => Config.Modifier_Diamond_Multiplier,
			BrainrotModifier.Rainbow => Config.Modifier_Rainbow_Multiplier,
			BrainrotModifier.Woods => Config.Modifier_Woods_Multiplier,
			_ => Config.Modifier_None_Multiplier,
		};
	}

    private string GetRarityName()
	{
		switch (Entry.Rarity)
		{
			case BrainrotValueRarity.Rare: return "Rare";
			case BrainrotValueRarity.Epic: return "Epic";
			case BrainrotValueRarity.Legendary: return "Legendary";
			case BrainrotValueRarity.Mythic: return "Mythic";
			case BrainrotValueRarity.Ethereal: return "Ethereal";
			case BrainrotValueRarity.Primal: return "Primal";

			case BrainrotValueRarity.Secret: return "Secret";
			case BrainrotValueRarity.SecretForm1: return "Secret ev1";
			case BrainrotValueRarity.SecretForm2: return "Secret ev2";
			default: return "Common";
		}
	}

    public static Vector4 GetRarityColor(BrainrotValueRarity rarity)
	{
		switch (rarity)
		{
			case BrainrotValueRarity.Rare: return new Vector4(0f, 0.44f, 0.87f, 1f); // blue
			case BrainrotValueRarity.Epic: return new Vector4(0.64f, 0.21f, 0.93f, 1f); // purple
			case BrainrotValueRarity.Legendary: return new Vector4(1f, 1f, 0f, 1f); // yellow
			case BrainrotValueRarity.Mythic: return new Vector4(1f, 0f, 0f, 1f); // red
			case BrainrotValueRarity.Ethereal: return new Vector4(200f/255f, 162f/255f, 200f/255f, 1f); // #C8A2C8
			case BrainrotValueRarity.Primal: return new Vector4(47f/255f, 47f/255f, 47f/255f, 1f); // black #2F2F2F

			case BrainrotValueRarity.Secret:
			case BrainrotValueRarity.SecretForm1:
			case BrainrotValueRarity.SecretForm2:
				return new Vector4(0.74f, 0.37f, 1f, 1f); // #BC5EFF
			default:
				return new Vector4(0.8f, 0.8f, 0.8f, 1f); // common grey
		}
	}
	
	public static string GetModifierName(BrainrotModifier modifier)
	{
		return modifier switch
		{
			BrainrotModifier.Gold => "GOLD",
			BrainrotModifier.Diamond => "DIAMOND",
			BrainrotModifier.Rainbow => "RAINBOW",
			BrainrotModifier.Woods => "WOODS",
			_ => "",
		};
	}
	
	public static Vector4 GetModifierColor(BrainrotModifier modifier)
	{
		return modifier switch
		{
			BrainrotModifier.Gold => new Vector4(1f, 0.84f, 0f, 1f), // Gold
			BrainrotModifier.Diamond => new Vector4(0.3f, 0.82f, 1f, 1f), // Light blue/cyan
			BrainrotModifier.Rainbow => new Vector4(1f, 0.4f, 1f, 1f), // Pink/Magenta
			BrainrotModifier.Woods => new Vector4(0.4f, 0.8f, 0.2f, 1f), // Green
			_ => new Vector4(1f, 1f, 1f, 1f), // White
		};
	}

    public (Vector2 pos, UI.TextSettings text, float lineH) GetLayout()
	{
		//var basePos = Entity.VisualPosition + new Vector2(0f, 3f);
		var basePos = GetSpriteBottomPosition() + new Vector2(0f, 1.3f);
		float lineH = 0.23f;
		var text = UI.TextSettings.Default;
		text.HorizontalAlignment = UI.HorizontalAlignment.Center;
		text.VerticalAlignment = UI.VerticalAlignment.Center;
		text.Size = 0.2f;
		// Disable word wrapping so long names stay on a single line
		text.WordWrap = false;
		text.OverflowWrap = false;
		// Ensure outline is enabled (uses engine built-in outline)
		text.Outline = true;
		text.OutlineThickness = MathF.Max(1f, text.OutlineThickness);
		return (basePos, text, lineH);
	}

    public Vector2 GetSpriteTopPosition()
	{
		var halfSize = SpriteRenderer.GetWorldSize() * 0.5f;
		Matrix4 modelMatrix = SpriteRenderer.Entity.LocalToWorldMatrix;
		Vector2 offset = -SpriteRenderer.Sprite.Pivot * halfSize;
		Vector2 topLocal = new Vector2(0, halfSize.Y) + offset;
		Vector2 topCenterWorld = AOMath.TransformPoint(modelMatrix, topLocal);
		return topCenterWorld;
	}
	public Vector2 GetSpriteBottomPosition()
	{
		var halfSize = SpriteRenderer.GetWorldSize() * 0.5f;
		Matrix4 modelMatrix = SpriteRenderer.Entity.LocalToWorldMatrix;
		Vector2 offset = -SpriteRenderer.Sprite.Pivot * halfSize;
		Vector2 bottomLocal = new Vector2(0, -halfSize.Y) + offset;
		Vector2 bottomCenterWorld = AOMath.TransformPoint(modelMatrix, bottomLocal);
		return bottomCenterWorld;
	}

    public override void OnDestroy()
    {
        // Clear occupied tiles when destroyed
        if (OwnerPlot.Alive())
        {
            for (int dx = 0; dx < Width; dx++)
            {
                for (int dy = 0; dy < Height; dy++)
                {
                    int checkX = GridX + dx;
                    int checkY = GridY + dy;
                    if (checkX >= 0 && checkX < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(0) &&
                        checkY >= 0 && checkY < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(1))
                    {
                        if (OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] == Entity)
                        {
                            OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] = default;
                        }
                    }
                }
            }
        }
    }
}

