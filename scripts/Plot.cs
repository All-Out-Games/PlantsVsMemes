using System.Runtime.CompilerServices;
using AO;

public class PlotsManager : System<PlotsManager>
{
    public static Material LockedMaterial;

    public override void Start()
    {
        var shader = Assets.GetAsset<ShaderAsset>("$AO/shaders/greyscale.aosl");
        LockedMaterial = IM.CreateMaterial(shader);
    }

    public List<Plot> plots = new List<Plot>();

    public void ClaimPlot(MyPlayer player)
    {
        foreach (var plot in plots)
        {
            if (plot.Owner.Alive() == false)
            {
                player.PlotEntityId.Set((int)plot.Entity.NetworkId);
                break;
            }
        }
    }
}

public partial class Plot : Component
{
    [Serialized] public Entity TilesParent;
    [Serialized] public Entity PortalsParent;
    [Serialized] public Entity ExitsParent;
    [Serialized] public Entity CollectiblesPlacementParent;
    [Serialized] public Entity ZeroCollectiblesTile;
    [Serialized] public Entity RowPurchaseParent;

    public MyPlayer Owner;


    public Entity[,] TurretsOccupiedBy = new Entity[9, 7];
    public Entity[,] CollectiblesPlacementOccupiedBy = new Entity[16, 14];

    public Sprite_Renderer[,] TurretTiles;
    public Vector4[,] OriginalTileColors;

    public Sprite_Renderer[,] CollectiblesPlacementTiles;

    public Entity[] Portals;
    public Spine_Animator[] Exits;
    
    public Entity[] AreaEntities;
    public Entity[] AreaTiles;
    public Entity[] AreaUnowned;
    public Interactable[] AreaInteractables;
    public Interactable[] RowPurchaseInteractables;

    public override void Awake()
    {
        PlotsManager.Instance.plots.Add(this);

        TurretTiles = new Sprite_Renderer[9, 7];
        OriginalTileColors = new Vector4[9, 7];
        foreach (var child in TilesParent.Children)
        {
            var spriteRenderer = child.GetComponent<Sprite_Renderer>();
            var tileCoords = GetTurretTileCoordsFromLocalPosition(child.LocalPosition);
            TurretTiles[tileCoords.Item1, tileCoords.Item2] = spriteRenderer;
            OriginalTileColors[tileCoords.Item1, tileCoords.Item2] = spriteRenderer.Tint;
        }

        Portals = new Entity[7];
        foreach (var child in PortalsParent.Children)
        {
            var tileCoords = GetTurretTileCoordsFromLocalPosition(child.LocalPosition);
            Portals[tileCoords.Item2] = child;
        }

        Exits = new Spine_Animator[7];
        foreach (var child in ExitsParent.Children)
        {
            var tileCoords = GetTurretTileCoordsFromLocalPosition(child.LocalPosition);
            Exits[tileCoords.Item2] = child.GetComponent<Spine_Animator>();
        }

        RowPurchaseInteractables = new Interactable[7];
        foreach (var child in RowPurchaseParent.Children)
        {
            var tileCoords = GetTurretTileCoordsFromLocalPosition(child.LocalPosition);
            RowPurchaseInteractables[tileCoords.Item2] = child.TryGetChildByIndex(0).GetComponent<Interactable>();
            RowPurchaseInteractables[tileCoords.Item2].OnInteract = (p) => OnRowPurchaseInteract(p, RowPurchaseInteractables[tileCoords.Item2]);
            RowPurchaseInteractables[tileCoords.Item2].CanUseCallback = (p) => OnRowPurchaseCanUse(p, RowPurchaseInteractables[tileCoords.Item2]);
        }

        CollectiblesPlacementTiles = new Sprite_Renderer[16, 14];
        
        // Count and initialize areas
        int areaCount = 0;
        foreach (var area in CollectiblesPlacementParent.Children)
        {
            areaCount++;
        }
        
        AreaEntities = new Entity[areaCount];
        AreaTiles = new Entity[areaCount];
        AreaUnowned = new Entity[areaCount];
        AreaInteractables = new Interactable[areaCount];
        
        int areaIndex = 0;
        foreach (var area in CollectiblesPlacementParent.Children)
        {
            AreaEntities[areaIndex] = area;
            
            var tilesParent = area.TryGetChildByName("Tiles");
            AreaTiles[areaIndex] = tilesParent;
            
            var unownedChild = area.TryGetChildByName("Unowned");
            AreaUnowned[areaIndex] = unownedChild;

            AreaInteractables[areaIndex] = unownedChild.TryGetChildByName("Interactable").GetComponent<Interactable>();
            AreaInteractables[areaIndex].OnInteract = (p) => OnAreaInteract(p, area);
            AreaInteractables[areaIndex].CanUseCallback = (p) => OnAreaCanUse(p, areaIndex);
            
            foreach (var tileEntity in tilesParent.Children)
            {
                var tileCoords = GetCollectiblesPlacementTileCoordsFromWorldPosition(tileEntity.Position);
                CollectiblesPlacementTiles[tileCoords.Item1, tileCoords.Item2] = tileEntity.GetComponent<Sprite_Renderer>();
            }
            
            areaIndex++;
        }
    }

    public void OnRowPurchaseInteract(Player p, Interactable interactable)
    {
        if (Network.IsServer == false) return;
        if (Owner != p) return;
        var rowStr = interactable.Entity.Parent.Name.Replace("R", "");
        var row = int.Parse(rowStr);
        var rowBits = (RowBits)Owner.UnlockedRows.Value;
        if (rowBits.HasFlag(GetRowBits(row)) == false)
        {
            var price = row switch {
                3 => 0,
                2 => 10_000,
                4 => 10_000,
                5 => 100_000,
                1 => 100_000,
                6 => 1_000_000,
                0 => 1_000_000,
            };
            if (Economy.GetBalance(Owner, Config.Currency_Gold) < price)
            {
                Owner.CallClient_ShowMessage("You don't have enough gold!", new RPCOptions() { Target = Owner });
            }
            else
            {
                Economy.WithdrawCurrency(Owner, Config.Currency_Gold, price);
            }
        }
    }

    public bool OnRowPurchaseCanUse(Player p, Interactable interactable)
    {
        if (Owner != p) return false;
        var rowStr = interactable.Entity.Parent.Name.Replace("R", "");
        var row = int.Parse(rowStr);
        var rowBits = (RowBits)Owner.UnlockedRows.Value;
        return rowBits.HasFlag(GetRowBits(row)) == false;
    }
    
    public bool OnAreaCanUse(Player p, int areaIndex)
    {
        return Owner.IsAreaUnlocked(areaIndex) == false;
    }

    public void OnAreaInteract(Player p, Entity areaEntity)
    {
        if (Network.IsServer) return;
        
        if (areaEntity.Name == "Area0")
        {
            return;
        }

        if (areaEntity.Name == "Area1")
        {
            Purchasing.PromptPurchase(PurchasingManager.BuyArea1ProductId);
        }

        if (areaEntity.Name == "Area2")
        {
            Purchasing.PromptPurchase(PurchasingManager.BuyArea2ProductId);
        }

        if (areaEntity.Name == "Area3")
        {
            Purchasing.PromptPurchase(PurchasingManager.BuyArea3ProductId);
        }
    }

    public (int, int) GetTurretTileCoordsFromLocalPosition(Vector2 position)
    {
        var x = (int) Math.Round(position.X / 2);
        var y = (int) Math.Round((position.Y + 6)/2);
        return (x, y);
    }

    public (int, int) GetTurretTileCoordsFromWorldPosition(Vector2 position)
    {
        var tilePosition = position - Entity.Position;
        var tileX = (int) Math.Round(tilePosition.X / 2);
        var tileY = (int) Math.Round((tilePosition.Y + 6) / 2);
        return (tileX, tileY);
    }

    public (int, int) GetCollectiblesPlacementTileCoordsFromWorldPosition(Vector2 position)
    {
        var tilesZero = ZeroCollectiblesTile.Position; //new Vector2(-10.5f, -6.5f);
        var tilePosition = position - tilesZero;
        var tileX = (int) Math.Round(tilePosition.X);
        var tileY = (int) Math.Round(tilePosition.Y);
        return (tileX, tileY);
    }
    
    public int GetAreaIndexFromTileCoords(int tileX, int tileY)
    {
        // Check each area to see if the tile belongs to it
        for (int areaIdx = 0; areaIdx < AreaTiles.Length; areaIdx++)
        {
            if (!AreaTiles[areaIdx].Alive())
                continue;
                
            foreach (var tileEntity in AreaTiles[areaIdx].Children)
            {
                var coords = GetCollectiblesPlacementTileCoordsFromWorldPosition(tileEntity.Position);
                if (coords.Item1 == tileX && coords.Item2 == tileY)
                {
                    return areaIdx;
                }
            }
        }
        return -1; // Not found in any area
    }

    public RowBits GetRowBits(int row)
    {
        return (RowBits)(1 << row);
    }

    public override void Update()
    {
        if (Owner.Alive() == false)
        {
            return;
        }

        var unlockedRows = (RowBits) Owner.UnlockedRows.Value;
        for (var x = 0; x < TurretTiles.GetLength(0); x++)
        {
            for (var y = 0; y < TurretTiles.GetLength(1); y++)
            {
                bool rowUnlocked = unlockedRows.HasFlag(GetRowBits(y));
                if (rowUnlocked)
                {
                    TurretTiles[x, y].SetMaterial(null, false);
                }
                else
                {
                    
                    TurretTiles[x, y].SetMaterial(PlotsManager.LockedMaterial, false);
                }
            }
        }

        for (var i = 0; i < Portals.Length; i++)
        {
            Portals[i].LocalEnabled = unlockedRows.HasFlag(GetRowBits(i));
        }

        for (var i = 0; i < Exits.Length; i++)
        {
            Exits[i].Entity.LocalEnabled = unlockedRows.HasFlag(GetRowBits(i));
            RowPurchaseInteractables[i].Entity.LocalEnabled = !Exits[i].Entity.LocalEnabled;
        }
        
        // Update area visibility based on unlock state
        for (var i = 0; i < AreaEntities.Length; i++)
        {
            bool isUnlocked = Owner.IsAreaUnlocked(i);
            
            if (AreaTiles[i].Alive())
            {
                AreaTiles[i].LocalEnabled = isUnlocked;
            }
            
            if (AreaUnowned[i].Alive())
            {
                AreaUnowned[i].LocalEnabled = !isUnlocked;
            }
        }
    }

    public bool CanTurretPlaceAt(int x, int y, int width, int height)
    {
        if (!Owner.Alive())
            return false;

        var unlockedRows = (RowBits)Owner.UnlockedRows.Value;

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int checkX = x + dx;
                int checkY = y + dy;

                if (checkX < 0 || checkX >= 9 || checkY < 0 || checkY >= 7)
                    return false;

                bool rowUnlocked = unlockedRows.HasFlag(GetRowBits(checkY));
                if (!rowUnlocked)
                    return false;

                if (TurretsOccupiedBy[checkX, checkY].Alive())
                    return false;
            }
        }

        return true;
    }

    public bool CanPlaceCollectibleAt(int x, int y, int width, int height)
    {
        if (!Owner.Alive())
            return false;

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int checkX = x + dx;
                int checkY = y + dy;
                
                // Check bounds
                if (checkX < 0 || checkX >= CollectiblesPlacementOccupiedBy.GetLength(0) ||
                    checkY < 0 || checkY >= CollectiblesPlacementOccupiedBy.GetLength(1))
                    return false;
                
                // Check if the area is unlocked
                int areaIndex = GetAreaIndexFromTileCoords(checkX, checkY);
                if (areaIndex >= 0 && !Owner.IsAreaUnlocked(areaIndex))
                    return false;
                
                // Check if occupied
                if (CollectiblesPlacementOccupiedBy[checkX, checkY].Alive())
                    return false;
            }
        }

        return true;
    }

    public Vector2 GetWorldPositionForTurretTile(int x, int y)
    {
        return TurretTiles[x, y].Entity.Position;
    }

    public Vector2 GetWorldPositionForCollectiblesPlacementTile(int x, int y)
    {
        return CollectiblesPlacementTiles[x, y].Entity.Position;
    }

    public Vector2 GetCenteredWorldPositionForCollectible(int x, int y, int width, int height)
    {
        // Calculate the center position for a multi-tile collectible
        float centerOffsetX = (width - 1) * 0.5f;
        float centerOffsetY = (height - 1) * 0.5f;
        
        Vector2 basePos = GetWorldPositionForCollectiblesPlacementTile(x, y);
        return basePos + new Vector2(centerOffsetX, centerOffsetY);
    }

    // [ServerRpc]
    // public void ServerRpc_PlaceCollectible(int x, int y, int inventoryIndex)
    // {
    //     if (!Owner.Alive())
    //         return;

    //     if (inventoryIndex < 0 || inventoryIndex >= Owner.LocalInventory.Count)
    //         return;

    //     var collectible = Owner.LocalInventory[inventoryIndex];
        
    //     if (!CanPlaceAt(x, y, collectible.Width, collectible.Height))
    //         return;

    //     var collectibleEntity = Entity.Create();
    //     collectibleEntity.Position = GetWorldPositionForTile(x, y);

    //     var spineAnimator = collectibleEntity.AddComponent<Spine_Animator>();
    //     var spinePath = CollectibleDatabase.GetSpinePath(collectible.EnemyType);
    //     var skeleton = Assets.GetAsset<SpineSkeletonAsset>(spinePath);
    //     spineAnimator.SpineInstance.SetSkeleton(skeleton);
    //     spineAnimator.SpineInstance.SetAnimation("Idle", true);
    //     float scale = Math.Max(collectible.Width, collectible.Height) * 0.3f;
    //     spineAnimator.SpineInstance.Scale = new Vector2(scale, scale);

    //     var placed = collectibleEntity.AddComponent<PlacedCollectible>();
    //     placed.CollectibleType.Set(collectible.EnemyType);
    //     placed.Width = collectible.Width;
    //     placed.Height = collectible.Height;
    //     placed.MoneyPerSecond = collectible.MoneyPerSecond;
    //     placed.OwnerPlayer = Owner;
    //     placed.OwnerPlot = this;
    //     placed.GridX = x;
    //     placed.GridY = y;

    //     Network.Spawn(collectibleEntity);

    //     for (int dx = 0; dx < collectible.Width; dx++)
    //     {
    //         for (int dy = 0; dy < collectible.Height; dy++)
    //         {
    //             OccupiedBy[x + dx, y + dy] = collectibleEntity;
    //         }
    //     }

    //     Owner.RemoveFromInventory(inventoryIndex);
    // }

    // [ServerRpc]
    // public void RemovePlacement(int x, int y)
    // {
    //     if (!Owner.Alive())
    //         return;

    //     if (x < 0 || x >= 9 || y < 0 || y >= 7)
    //         return;

    //     var entity = OccupiedBy[x, y];
    //     if (!entity.Alive())
    //         return;

    //     var placedCollectible = entity.GetComponent<PlacedCollectible>();
    //     if (placedCollectible != null)
    //     {
    //         for (int dx = 0; dx < placedCollectible.Width; dx++)
    //         {
    //             for (int dy = 0; dy < placedCollectible.Height; dy++)
    //             {
    //                 int clearX = placedCollectible.GridX + dx;
    //                 int clearY = placedCollectible.GridY + dy;
    //                 if (clearX >= 0 && clearX < 9 && clearY >= 0 && clearY < 7)
    //                 {
    //                     OccupiedBy[clearX, clearY] = default;
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         OccupiedBy[x, y] = default;
    //     }

    //     Network.Despawn(entity);
    //     entity.Destroy();
    // }
}