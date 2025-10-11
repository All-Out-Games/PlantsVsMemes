using AO;
using System.Text.Json;

public partial class MyPlayer : Player
{
    public SyncVar<int> UnlockedRows = new SyncVar<int>(0);
    public SyncVar<string> InventoryData = new SyncVar<string>("");

    public static Item_Definition PeashooterItem;
    public static Item_Definition CactusItem;
    public static Item_Definition StarfruitItem;
    public static Item_Definition MelonpultItem;
    public static Item_Definition CobcannonItem;

    public Entity TurretGhost;
    public Entity CollectibleGhost;

    public string SelectedTurretType = "";
    Plot localPlot;

    public ulong RNGSeed;

    public override void Awake()
    {
        PlotsManager.Instance.ClaimPlot(this);

        if (Network.IsServer)
        {
            UnlockedRows.Set(Save.GetInt(this, "unlockedRows", (int)RowBits.Row3));
            InventoryData.Set(Save.GetString(this, "inventory", "[]"));

            GrantStartingTurrets();
        }

        RNGSeed = (ulong)new Random().NextInt64();

    }

    void GrantStartingTurrets()
    {
        if (!Network.IsServer)
            return;

        var peashooter = Inventory.CreateItem(PeashooterItem, 1);
        var cactus = Inventory.CreateItem(CactusItem, 1);
        var starfruit = Inventory.CreateItem(StarfruitItem, 1);
        var melonpult = Inventory.CreateItem(MelonpultItem, 1);
        var cobcannon = Inventory.CreateItem(CobcannonItem, 1);

        Inventory.MoveItemToInventory(peashooter, DefaultInventory);
        Inventory.MoveItemToInventory(cactus, DefaultInventory);
        Inventory.MoveItemToInventory(starfruit, DefaultInventory);
        Inventory.MoveItemToInventory(melonpult, DefaultInventory);
        Inventory.MoveItemToInventory(cobcannon, DefaultInventory);
    }

    public override void Update()
    {
        if (!IsLocal)
            return;

        if (localPlot == null || !localPlot.Entity.Alive())
        {
            foreach (var plot in Scene.Components<Plot>())
            {
                if (plot.Owner.Entity.Alive() && plot.Owner == this)
                {
                    localPlot = plot;
                    break;
                }
            }
        }

        var hotbarResult = Inventory.DrawHotbar(DefaultInventory.Id, new Inventory.DrawOptions()
        {
            HotbarItemCount = 5,
            AllowDragDrop = true,
            EnableUseFromHotbar = true,
            ScrollItemSelection = true,
            KeyboardItemSelection = true,
        });

        for (var x = 0; x < localPlot.TurretTiles.GetLength(0); x++)
        {
            for (var y = 0; y < localPlot.TurretTiles.GetLength(1); y++)
            {
                localPlot.TurretTiles[x, y].Tint = localPlot.OriginalTileColors[x, y];
            }
        }

        for (var x = 0; x < localPlot.CollectiblesPlacementTiles.GetLength(0); x++)
        {
            for (var y = 0; y < localPlot.CollectiblesPlacementTiles.GetLength(1); y++)
            {
                if (localPlot.CollectiblesPlacementTiles[x, y] != null) {
                    localPlot.CollectiblesPlacementTiles[x, y].Tint = new Vector4(100/255f, 100/255f, 100/255f, 125/255f);
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

                    var tileCoords = localPlot.GetTurretTileCoordsFromWorldPosition(Input.GetMousePosition());
                    var tileX = tileCoords.Item1;
                    var tileY = tileCoords.Item2;
                    if (tileX < localPlot.TurretTiles.GetLength(0) && tileY < localPlot.TurretTiles.GetLength(1) && tileX >= 0 && tileY >= 0)
                    {
                        var originalColor = localPlot.OriginalTileColors[tileX, tileY];
                        localPlot.TurretTiles[tileX, tileY].Tint = originalColor * new Vector4(1.2f, 0.5f, 0.5f, 1);
                        TurretGhost.Position = localPlot.GetWorldPositionForTurretTile(tileX, tileY);
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

                    var collectiblesPlacementTileCoords = localPlot.GetCollectiblesPlacementTileCoordsFromWorldPosition(Input.GetMousePosition());
                    var collectiblesPlacementTileX = collectiblesPlacementTileCoords.Item1;
                    var collectiblesPlacementTileY = collectiblesPlacementTileCoords.Item2;
                    if (collectiblesPlacementTileX < localPlot.CollectiblesPlacementTiles.GetLength(0) && collectiblesPlacementTileY < localPlot.CollectiblesPlacementTiles.GetLength(1) && collectiblesPlacementTileX >= 0 && collectiblesPlacementTileY >= 0)
                    {
                        localPlot.CollectiblesPlacementTiles[collectiblesPlacementTileX, collectiblesPlacementTileY].Tint = new Vector4(50/255f, 100/255f, 50/255f, 125/255f);
                        CollectibleGhost.Position = localPlot.GetWorldPositionForCollectiblesPlacementTile(collectiblesPlacementTileX, collectiblesPlacementTileY);
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
                var pos = localPlot.GetTurretTileCoordsFromWorldPosition(Input.GetMousePosition());
                if (localPlot.CanTurretPlaceAt(pos.Item1, pos.Item2, 1, 1))
                {
                    CallServer_PlaceTurret(pos.Item1, pos.Item2, hotbarResult.DroppedItem.Id);
                }
            }
            else 
            {
                var pos = localPlot.GetCollectiblesPlacementTileCoordsFromWorldPosition(Input.GetMousePosition());
                if (localPlot.CanPlaceCollectibleAt(pos.Item1, pos.Item2, 1, 1))
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
        var turret = turretEntity.AddComponent<Turret>(onBeforeAwake: (t) => {
            t.GridX = x;
            t.GridY = y;
            t.TurretType = itemToPlace.Definition.Id;
            t.OwnerPlot = playerPlot;
        });
        
        Network.Spawn(turretEntity);
        Inventory.DestroyItem(itemToPlace);
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

        if (!playerPlot.CanPlaceCollectibleAt(x, y, 1, 1))
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

        var collectibleEntity = Entity.Create();
        collectibleEntity.Position = playerPlot.GetWorldPositionForCollectiblesPlacementTile(x, y);
        // collectibleEntity.Position += new Vector2(0, RNG.RangeFloat(ref RNGSeed, -0.01f, 0.01f));
        // collectibleEntity.Position += CollectibleDatabase.GetPositionOffset(itemToPlace.Definition.Id);
        collectibleEntity.AddComponent<Sprite_Renderer>();
        var collectible = collectibleEntity.AddComponent<PlacedCollectible>(onBeforeAwake: (p) => {
            p.GridX = x;
            p.GridY = y;
            p.CollectibleType = itemToPlace.Definition.Id;
            p.OwnerPlayer = player;
            p.OwnerPlot = playerPlot;
        });
        Network.Spawn(collectibleEntity);
        Inventory.DestroyItem(itemToPlace);
    }

    public override void OnDestroy()
    {
        if (Network.IsServer)
        {
            // Save.SetInt(this, "money", Money.Value);
            // Save.SetInt(this, "unlockedRows", UnlockedRows.Value);
            // Save.SetString(this, "inventory", InventoryData.Value);
        }
    }
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
