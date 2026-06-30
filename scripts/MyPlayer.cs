using AO;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class MyPlayer : Player
{
    public SyncVar<int> UnlockedRows = new SyncVar<int>(0);
    public SyncVar<int> UnlockedAreas = new SyncVar<int>(1); // Start with Area0 unlocked (bit 0 = 1)
    public SyncVar<string> InventoryData = new SyncVar<string>("");
    public SyncVar<int> RebirthLevel = new SyncVar<int>(0);

    public static Item_Definition PeashooterItem;
    public static Item_Definition CactusItem;
    public static Item_Definition StarfruitItem;
    public static Item_Definition MelonpultItem;
    public static Item_Definition CobcannonItem;

    public Entity TurretGhost;
    public Entity CollectibleGhost;

    public string SelectedTurretType = "";
    
    public SyncVar<int> PlotEntityId = new SyncVar<int>(0);
    public Plot Plot;

    public ulong RNGSeed;

    public override void Awake()
    {
        PlotEntityId.OnSync += OnPlotEntityIdSync;

        if (Network.IsServer)
        {
            UnlockedRows.Set(Save.GetInt(this, Config.SaveKey_UnlockedRows, (int)RowBits.Row3));
            UnlockedAreas.Set(Save.GetInt(this, Config.SaveKey_UnlockedAreas, 1)); // Default: Area0 unlocked
            InventoryData.Set(Save.GetString(this, Config.SaveKey_Inventory, "[]"));
            RebirthLevel.Set(Save.GetInt(this, Config.SaveKey_RebirthLevel, 0));
            
            // Grant starter peashooter if they haven't received it yet
            int hasReceivedStarter = Save.GetInt(this, Config.SaveKey_ReceivedStarterPeashooter, 0);
            if (hasReceivedStarter == 0)
            {
                GrantStarterPeashooter();
                Save.SetInt(this, Config.SaveKey_ReceivedStarterPeashooter, 1);
            }

            PlotsManager.Instance.ClaimPlot(this);
            LoadPlacedItems();
        }

        RNGSeed = (ulong)new Random().NextInt64();
    }

    public void OnPlotEntityIdSync(int old, int newPlotEntityId)
    {
        foreach (var plot in Scene.Components<Plot>())
        {
            Log.Info("Checking plot " + plot.Entity.NetworkId + " against " + newPlotEntityId);
            if (plot.Entity.NetworkId == (ulong)newPlotEntityId)
            {
                Plot = plot;
                break;
            }
        }

        if (Plot != null)
        {
            Log.Info("Claimed plot " + Plot.Entity.NetworkId);
            Plot.Owner = this;
            Teleport(Plot.Entity.Position);
        }
    }
    
    void GrantStarterPeashooter()
    {
        if (!Network.IsServer)
            return;
            
        var starterItem = Inventory.CreateItem(PeashooterItem, 1);
        if (Inventory.CanMoveItemToInventory(starterItem, DefaultInventory, out var willDestroyItem))
        {
            Inventory.MoveItemToInventory(starterItem, DefaultInventory);
            Log.Info($"[MyPlayer] Granted starter peashooter to {Name}");
        }
        else
        {
            Inventory.DestroyItem(starterItem);
            Log.Info($"[MyPlayer] Failed to grant starter peashooter to {Name} - inventory full");
        }
    }

    public override void Update()
    {
        if (Plot.Alive() == false) return;
        if (!IsLocal)
            return;

        var hotbarResult = Inventory.DrawHotbar(DefaultInventory.Id, new Inventory.DrawOptions()
        {
            HotbarItemCount = 8,
            AllowDragDrop = true,
            EnableUseFromHotbar = true,
            ScrollItemSelection = true,
            KeyboardItemSelection = true,
        });

        for (var x = 0; x < Plot.TurretTiles.GetLength(0); x++)
        {
            for (var y = 0; y < Plot.TurretTiles.GetLength(1); y++)
            {
                Plot.TurretTiles[x, y].Tint = Plot.OriginalTileColors[x, y];
            }
        }

        for (var x = 0; x < Plot.CollectiblesPlacementTiles.GetLength(0); x++)
        {
            for (var y = 0; y < Plot.CollectiblesPlacementTiles.GetLength(1); y++)
            {
                if (Plot.CollectiblesPlacementTiles[x, y] != null) 
                {
                    // Check if this tile is occupied by a collectible - color it based on rarity
                    if (Plot.CollectiblesPlacementOccupiedBy[x, y].Alive())
                    {
                        var collectibleEntity = Plot.CollectiblesPlacementOccupiedBy[x, y];
                        var placedCollectible = collectibleEntity.GetComponent<PlacedCollectible>();
                        if (placedCollectible != null && placedCollectible.Entry != null)
                        {
                            // Get rarity color and apply it with some transparency
                            Vector4 rarityColor = PlacedCollectible.GetRarityColor(placedCollectible.Entry.Rarity);
                            Plot.CollectiblesPlacementTiles[x, y].Tint = rarityColor * 0.6f + new Vector4(0.2f, 0.2f, 0.2f, 0.5f);
                        }
                        else
                        {
                            // Fallback to yellow if we can't get the rarity
                            Plot.CollectiblesPlacementTiles[x, y].Tint = new Vector4(1.0f, 1.0f, 0.3f, 0.5f);
                        }
                    }
                    else
                    {
                        Plot.CollectiblesPlacementTiles[x, y].Tint = new Vector4(100/255f, 100/255f, 100/255f, 125/255f);
                    }
                }
            }
        }

        var currentPayload = UI.TryGetCurrentDragDropPayload();
        if (currentPayload != null)
        {
            if (currentPayload as Item_Instance != null)
            {
                var item = currentPayload as Item_Instance;
                if (TurretDatabase.IsTurret(item.Definition.Id))
                {
                    if (TurretGhost == null)
                    {
                        TurretGhost = Entity.Create();
                        TurretGhost.Scale = new Vector2(2, 2);
                        var sa = TurretGhost.AddComponent<Spine_Animator>();
                        var spinePath = CollectibleDatabase.GetSpinePath(item.Definition.Id);
                        var skeleton = Assets.GetAsset<SpineSkeletonAsset>(spinePath);
                        sa.SpineInstance.SetSkeleton(skeleton);
                    }

                    var tileCoords = Plot.GetTurretTileCoordsFromWorldPosition(Input.GetMousePosition());
                    var tileX = tileCoords.Item1;
                    var tileY = tileCoords.Item2;
                    if (tileX < Plot.TurretTiles.GetLength(0) && tileY < Plot.TurretTiles.GetLength(1) && tileX >= 0 && tileY >= 0)
                    {
                        var originalColor = Plot.OriginalTileColors[tileX, tileY];
                        Plot.TurretTiles[tileX, tileY].Tint = originalColor * new Vector4(1.2f, 0.5f, 0.5f, 1);
                        TurretGhost.Position = Plot.GetWorldPositionForTurretTile(tileX, tileY);
                    }
                }
                else
                {
                    if (CollectibleGhost == null)
                    {
                        CollectibleGhost = Entity.Create();
                        var sa = CollectibleGhost.AddComponent<Sprite_Renderer>();
                        var defn = BrainrotCatalog.Get(item.Definition.Id);
                        sa.Sprite = defn.Sprite;
                        
                    }

                    var collectiblesPlacementTileCoords = Plot.GetCollectiblesPlacementTileCoordsFromWorldPosition(Input.GetMousePosition());
                    var collectiblesPlacementTileX = collectiblesPlacementTileCoords.Item1;
                    var collectiblesPlacementTileY = collectiblesPlacementTileCoords.Item2;
                    
                    // Get the size of this collectible
                    var size = CollectibleDatabase.GetCollectibleSize(item.Definition.Id);
                    int width = size.width;
                    int height = size.height;
                    
                    if (collectiblesPlacementTileX < Plot.CollectiblesPlacementTiles.GetLength(0) && collectiblesPlacementTileY < Plot.CollectiblesPlacementTiles.GetLength(1) && collectiblesPlacementTileX >= 0 && collectiblesPlacementTileY >= 0)
                    {
                        // Color all tiles that this collectible would occupy
                        bool canPlace = Plot.CanPlaceCollectibleAt(collectiblesPlacementTileX, collectiblesPlacementTileY, width, height);
                        Vector4 tileColor = canPlace ? new Vector4(50/255f, 100/255f, 50/255f, 125/255f) : new Vector4(100/255f, 50/255f, 50/255f, 125/255f);
                        
                        for (int dx = 0; dx < width; dx++)
                        {
                            for (int dy = 0; dy < height; dy++)
                            {
                                int checkX = collectiblesPlacementTileX + dx;
                                int checkY = collectiblesPlacementTileY + dy;
                                if (checkX < Plot.CollectiblesPlacementTiles.GetLength(0) && 
                                    checkY < Plot.CollectiblesPlacementTiles.GetLength(1) && 
                                    checkX >= 0 && checkY >= 0)
                                {
                                    Plot.CollectiblesPlacementTiles[checkX, checkY].Tint = tileColor;
                                }
                            }
                        }
                        
                        // Position ghost at center of occupied tiles
                        CollectibleGhost.Position = Plot.GetCenteredWorldPositionForCollectible(collectiblesPlacementTileX, collectiblesPlacementTileY, width, height);
                    }
                }
            }
        }
        else
        {
            if (TurretGhost != null)
            {
                TurretGhost.Destroy();
                TurretGhost = null;
            }
            if (CollectibleGhost != null)
            {
                CollectibleGhost.Destroy();
                CollectibleGhost = null;
            }
        }

        if (hotbarResult.DroppedItem != null)
        {
            if (TurretDatabase.IsTurret(hotbarResult.DroppedItem.Definition.Id))
            {
                var pos = Plot.GetTurretTileCoordsFromWorldPosition(Input.GetMousePosition());
                if (Plot.CanTurretPlaceAt(pos.Item1, pos.Item2, 1, 1))
                {
                    CallServer_PlaceTurret(pos.Item1, pos.Item2, hotbarResult.DroppedItem.Id);
                }
            }
            else 
            {
                var pos = Plot.GetCollectiblesPlacementTileCoordsFromWorldPosition(Input.GetMousePosition());
                var size = CollectibleDatabase.GetCollectibleSize(hotbarResult.DroppedItem.Definition.Id);
                if (Plot.CanPlaceCollectibleAt(pos.Item1, pos.Item2, size.width, size.height))
                {
                    CallServer_PlaceCollectible(pos.Item1, pos.Item2, hotbarResult.DroppedItem.Id);
                }
            }
        }
    }
    

    [ServerRpc]
    public void PlaceTurret(int x, int y, string itemId)
    {
        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null || !player.Entity.Alive())
            return;

        Plot playerPlot = null;
        foreach (var plot in Scene.Components<Plot>())
        {
            if (plot.Owner.Entity.Alive() && plot.Owner == player)
            {
                playerPlot = plot;
                break;
            }
        }

        if (playerPlot == null)
            return;

        if (!playerPlot.CanTurretPlaceAt(x, y, 1, 1))
            return;

        Item_Instance itemToPlace = null;
        foreach (var item in player.DefaultInventory.Items)
        {
            if (item != null && item.Id == itemId)
            {
                itemToPlace = item;
                break;
            }
        }

        if (itemToPlace == null)
            return;

        var turretEntity = Entity.Create();
        turretEntity.Position = playerPlot.GetWorldPositionForTurretTile(x, y);
        turretEntity.Position += new Vector2(0, RNG.RangeFloat(ref RNGSeed, -0.01f, 0.01f));
        turretEntity.Position += TurretDatabase.GetPositionOffset(itemToPlace.Definition.Id);
        turretEntity.AddComponent<Spine_Animator>();
        turretEntity.AddComponent<Interactable>();
        var turret = turretEntity.AddComponent<Turret>(onBeforeAwake: (t) => {
            t.GridX = x;
            t.GridY = y;
            t.TurretType = itemToPlace.Definition.Id;
            t.OwnerPlot = playerPlot;
        });
        
        Network.Spawn(turretEntity);
        Inventory.DestroyItem(itemToPlace);
        
        // Save after placing
        player.SavePlacedItems();
    }

    [ServerRpc]
    public void PlaceCollectible(int x, int y, string itemId)
    {
        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null || !player.Entity.Alive())
            return;

        Plot playerPlot = null;
        foreach (var plot in Scene.Components<Plot>())
        {
            if (plot.Owner.Entity.Alive() && plot.Owner == player)
            {
                playerPlot = plot;
                break;
            }
        }

        if (playerPlot == null)
            return;

        // Get the collectible size
        var size = CollectibleDatabase.GetCollectibleSize(itemId);
        int width = size.width;
        int height = size.height;

        if (!playerPlot.CanPlaceCollectibleAt(x, y, width, height))
            return;
            
        Item_Instance itemToPlace = null;
        foreach (var item in player.DefaultInventory.Items)
        {
            if (item != null && item.Id == itemId)
            {
                itemToPlace = item;
                break;
            }
        }

        if (itemToPlace == null)
            return;

        // Read modifier from item metadata
        BrainrotModifier modifier = BrainrotModifier.None;
        string modifierStr = itemToPlace.GetMetadata("modifier");
        if (!string.IsNullOrEmpty(modifierStr) && int.TryParse(modifierStr, out int modifierInt))
        {
            modifier = (BrainrotModifier)modifierInt;
        }

        var collectibleEntity = Entity.Create();
        // Position at center of occupied tiles
        collectibleEntity.Position = playerPlot.GetCenteredWorldPositionForCollectible(x, y, width, height);
        collectibleEntity.AddComponent<Sprite_Renderer>();
        collectibleEntity.AddComponent<Interactable>();
        var collectible = collectibleEntity.AddComponent<PlacedCollectible>(onBeforeAwake: (p) => {
            p.GridX = x;
            p.GridY = y;
            p.CollectibleType = itemToPlace.Definition.Id;
            p.OwnerPlayer = player;
            p.OwnerPlot = playerPlot;
            p.Modifier = modifier;
        });
        Network.Spawn(collectibleEntity);
        Inventory.DestroyItem(itemToPlace);
        
        // Save after placing
        player.SavePlacedItems();
    }

    public override void OnDestroy()
    {
        if (Network.IsServer)
        {
            Save.SetInt(this, Config.SaveKey_UnlockedRows, UnlockedRows.Value);
            Save.SetInt(this, Config.SaveKey_RebirthLevel, RebirthLevel.Value);
            // Save.SetString(this, Config.SaveKey_Inventory, InventoryData.Value);
            
            // Save placed items on disconnect
            SavePlacedItems();
        }
    }
    
    public float GetRebirthEarningsMultiplier()
    {
        // Each rebirth adds 25% to earnings (1.25x per rebirth, stacking multiplicatively)
        return MathF.Pow(1.25f, RebirthLevel.Value);
    }

    [ClientRpc]
    public void ShowMessage(string message)
    {
        if (!IsLocal) return;
        
        // Display message using UI
        Log.Info(message);
    }
    
    [ClientRpc]
    public void PlayFusionSound(string soundAssetName)
    {
        if (!IsLocal) return;
        
        var sound = Assets.GetAsset<AudioAsset>(soundAssetName);
        if (sound != null)
        {
            SFX.Play(sound, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
        }
    }
    
    [ClientRpc]
    public void PlayRebirthEffect()
    {
        if (!IsLocal) return;
        
        // Play a special sound or visual effect for rebirth
        // For now, just log it
        Log.Info("⟳ REBIRTH COMPLETE! ⟳");
    }
    
    [ServerRpc]
    public void RequestSellSingleItem(string itemId)
    {
        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null || !player.Entity.Alive())
            return;
        
        // Find the item in inventory
        Item_Instance itemToSell = null;
        foreach (var item in player.DefaultInventory.Items)
        {
            if (item != null && item.Id == itemId)
            {
                itemToSell = item;
                break;
            }
        }
        
        if (itemToSell == null)
            return;
        
        long sellValue = CalculateSellValue(itemToSell);
        
        // Destroy the item and give gold
        Inventory.DestroyItem(itemToSell);
        Economy.DepositCurrency(player, Config.Currency_Gold, sellValue);
        
        player.CallClient_ShowMessage($"Sold for {StringUtils.FormatMoney(sellValue)} gold!", new RPCOptions() { Target = player });
    }
    
    [ServerRpc]
    public void RequestSellAllBrainrots()
    {
        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null || !player.Entity.Alive())
            return;
        
        long totalValue = 0;
        int count = 0;
        
        var itemsToSell = new List<Item_Instance>();
        foreach (var item in player.DefaultInventory.Items)
        {
            if (item != null && BrainrotCatalog.Entries.ContainsKey(item.Definition.Id))
            {
                itemsToSell.Add(item);
            }
        }
        
        foreach (var item in itemsToSell)
        {
            long sellValue = CalculateSellValue(item);
            totalValue += sellValue;
            count++;
            Inventory.DestroyItem(item);
        }
        
        if (totalValue > 0)
        {
            Economy.DepositCurrency(player, Config.Currency_Gold, totalValue);
            player.CallClient_ShowMessage($"Sold {count} brainrots for {StringUtils.FormatMoney(totalValue)} gold!", new RPCOptions() { Target = player });
        }
    }
    
    [ServerRpc]
    public void RequestSellAllTurrets()
    {
        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null || !player.Entity.Alive())
            return;
        
        long totalValue = 0;
        int count = 0;
        
        var itemsToSell = new List<Item_Instance>();
        foreach (var item in player.DefaultInventory.Items)
        {
            if (item != null && TurretDatabase.IsTurret(item.Definition.Id))
            {
                itemsToSell.Add(item);
            }
        }
        
        foreach (var item in itemsToSell)
        {
            long sellValue = CalculateSellValue(item);
            totalValue += sellValue;
            count++;
            Inventory.DestroyItem(item);
        }
        
        if (totalValue > 0)
        {
            Economy.DepositCurrency(player, Config.Currency_Gold, totalValue);
            player.CallClient_ShowMessage($"Sold {count} turrets for {StringUtils.FormatMoney(totalValue)} gold!", new RPCOptions() { Target = player });
        }
    }
    
    long CalculateSellValue(Item_Instance item)
    {
        // Check if it's a brainrot
        if (BrainrotCatalog.Entries.ContainsKey(item.Definition.Id))
        {
            var entry = BrainrotCatalog.Get(item.Definition.Id);
            long baseValue = entry.BasePrice / 2; // Sell for 50% of base price
            
            // Apply modifier bonus to sell value
            BrainrotModifier modifier = BrainrotModifier.None;
            string modifierStr = item.GetMetadata("modifier");
            if (!string.IsNullOrEmpty(modifierStr) && int.TryParse(modifierStr, out int modifierInt))
            {
                modifier = (BrainrotModifier)modifierInt;
            }
            
            float modifierMult = PlacedCollectible.GetModifierMultiplier(modifier);
            return (long)(baseValue * modifierMult);
        }
        
        // Check if it's a turret
        if (TurretDatabase.IsTurret(item.Definition.Id))
        {
            int turretCost = TurretDatabase.GetCost(item.Definition.Id);
            return turretCost / 2; // Sell for 50% of cost
        }
        
        return 0;
    }

    void LoadPlacedItems()
    {
        if (!Network.IsServer)
            return;
            

        // Find the player's plot
        Plot playerPlot = null;
        foreach (var plot in Scene.Components<Plot>())
        {
            if (plot.Owner.Entity.Alive() && plot.Owner == this)
            {
                playerPlot = plot;
                break;
            }
        }

        if (playerPlot == null)
            return;

        // Load turrets
        string turretsJson = Save.GetString(this, Config.SaveKey_PlacedTurrets, "[]");
        try
        {
            var turrets = JsonSerializer.Deserialize<List<SavedTurretData>>(turretsJson);
            if (turrets != null)
            {
                foreach (var turretData in turrets)
                {
                    // Validate position
                    if (!playerPlot.CanTurretPlaceAt(turretData.GridX, turretData.GridY, 1, 1))
                        continue;

                    var turretEntity = Entity.Create();
                    turretEntity.Position = playerPlot.GetWorldPositionForTurretTile(turretData.GridX, turretData.GridY);
                    turretEntity.Position += new Vector2(0, RNG.RangeFloat(ref RNGSeed, -0.01f, 0.01f));
                    turretEntity.Position += TurretDatabase.GetPositionOffset(turretData.TurretType);
                    turretEntity.AddComponent<Spine_Animator>();
                    turretEntity.AddComponent<Interactable>();
                    turretEntity.AddComponent<Turret>(onBeforeAwake: (t) => {
                        t.GridX = turretData.GridX;
                        t.GridY = turretData.GridY;
                        t.TurretType = turretData.TurretType;
                        t.OwnerPlot = playerPlot;
                    });
                    Network.Spawn(turretEntity);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[MyPlayer] Failed to load turrets: {ex.Message}");
        }

        // Load collectibles
        string collectiblesJson = Save.GetString(this, Config.SaveKey_PlacedCollectibles, "[]");
        try
        {
            var collectibles = JsonSerializer.Deserialize<List<SavedCollectibleData>>(collectiblesJson);
            if (collectibles != null)
            {
                foreach (var collectibleData in collectibles)
                {
                    var size = CollectibleDatabase.GetCollectibleSize(collectibleData.CollectibleType);
                    
                    // Validate position
                    if (!playerPlot.CanPlaceCollectibleAt(collectibleData.GridX, collectibleData.GridY, size.width, size.height))
                        continue;

                    var collectibleEntity = Entity.Create();
                    collectibleEntity.Position = playerPlot.GetCenteredWorldPositionForCollectible(
                        collectibleData.GridX, collectibleData.GridY, size.width, size.height);
                    collectibleEntity.AddComponent<Sprite_Renderer>();
                    collectibleEntity.AddComponent<Interactable>();
                    collectibleEntity.AddComponent<PlacedCollectible>(onBeforeAwake: (p) => {
                        p.GridX = collectibleData.GridX;
                        p.GridY = collectibleData.GridY;
                        p.CollectibleType = collectibleData.CollectibleType;
                        p.OwnerPlayer = this;
                        p.OwnerPlot = playerPlot;
                        p.Modifier = (BrainrotModifier)collectibleData.Modifier;
                    });
                    Network.Spawn(collectibleEntity);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[MyPlayer] Failed to load collectibles: {ex.Message}");
        }
    }

    public void UnlockArea(int areaIndex)
    {
        if (!Network.IsServer)
            return;
        
        int areaBit = 1 << areaIndex;
        int newValue = UnlockedAreas.Value | areaBit;
        UnlockedAreas.Set(newValue);
        Save.SetInt(this, Config.SaveKey_UnlockedAreas, newValue);
        
        Log.Info($"[MyPlayer] Unlocked Area{areaIndex} for {Name}");
    }
    
    public bool IsAreaUnlocked(int areaIndex)
    {
        int areaBit = 1 << areaIndex;
        return (UnlockedAreas.Value & areaBit) != 0;
    }

    public void SavePlacedItems()
    {
        if (!Network.IsServer)
            return;

        // Find the player's plot
        Plot playerPlot = null;
        foreach (var plot in Scene.Components<Plot>())
        {
            if (plot.Owner != null && plot.Owner.Entity.Alive() && plot.Owner == this)
            {
                playerPlot = plot;
                break;
            }
        }

        if (playerPlot == null)
            return;

        // Save all turrets
        var turretDataList = new List<SavedTurretData>();
        foreach (var turret in Scene.Components<Turret>())
        {
            if (turret.OwnerPlot.Alive() && turret.OwnerPlot == playerPlot)
            {
                turretDataList.Add(new SavedTurretData
                {
                    TurretType = turret.TurretType,
                    GridX = turret.GridX,
                    GridY = turret.GridY
                });
            }
        }
        string turretsJson = JsonSerializer.Serialize(turretDataList);
        Save.SetString(this, Config.SaveKey_PlacedTurrets, turretsJson);

        // Save all collectibles
        var collectibleDataList = new List<SavedCollectibleData>();
        foreach (var collectible in Scene.Components<PlacedCollectible>())
        {
            if (collectible.OwnerPlayer.Alive() && collectible.OwnerPlayer == this)
            {
                collectibleDataList.Add(new SavedCollectibleData
                {
                    CollectibleType = collectible.CollectibleType,
                    GridX = collectible.GridX,
                    GridY = collectible.GridY,
                    Modifier = (int)collectible.Modifier
                });
            }
        }
        string collectiblesJson = JsonSerializer.Serialize(collectibleDataList);
        Save.SetString(this, Config.SaveKey_PlacedCollectibles, collectiblesJson);
    }
}

public class SavedTurretData
{
    [JsonPropertyName("type")]
    public string TurretType { get; set; }
    
    [JsonPropertyName("x")]
    public int GridX { get; set; }
    
    [JsonPropertyName("y")]
    public int GridY { get; set; }
}

public class SavedCollectibleData
{
    [JsonPropertyName("type")]
    public string CollectibleType { get; set; }
    
    [JsonPropertyName("x")]
    public int GridX { get; set; }
    
    [JsonPropertyName("y")]
    public int GridY { get; set; }
    
    [JsonPropertyName("mod")]
    public int Modifier { get; set; }
}

public enum RowBits 
{
    Row0 = 1 << 0,
    Row1 = 1 << 1,
    Row2 = 1 << 2,
    Row3 = 1 << 3,
    Row4 = 1 << 4,
    Row5 = 1 << 5,
    Row6 = 1 << 6,
}

public enum AreaBits
{
    Area0 = 1 << 0,
    Area1 = 1 << 1,
    Area2 = 1 << 2,
    Area3 = 1 << 3,
}
