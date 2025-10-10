using AO;
using System.Text.Json;

public partial class MyPlayer : Player
{
    public SyncVar<int> UnlockedRows = new SyncVar<int>(0);
    public SyncVar<string> InventoryData = new SyncVar<string>("");

    public List<CollectibleEnemy> LocalInventory = new List<CollectibleEnemy>();

    public static Item_Definition PeashooterItem;
    public static Item_Definition CactusItem;
    public static Item_Definition StarfruitItem;
    public static Item_Definition MelonpultItem;

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

        InventoryData.OnSync += (oldValue, newValue) =>
        {
            ParseInventory();
        };

        RNGSeed = (ulong)new Random().NextInt64();

        ParseInventory();
    }

    void GrantStartingTurrets()
    {
        if (!Network.IsServer)
            return;

        var peashooter = Inventory.CreateItem(PeashooterItem, 1);
        var cactus = Inventory.CreateItem(CactusItem, 1);
        var starfruit = Inventory.CreateItem(StarfruitItem, 1);
        var melonpult = Inventory.CreateItem(MelonpultItem, 1);

        Inventory.MoveItemToInventory(peashooter, DefaultInventory);
        Inventory.MoveItemToInventory(cactus, DefaultInventory);
        Inventory.MoveItemToInventory(starfruit, DefaultInventory);
        Inventory.MoveItemToInventory(melonpult, DefaultInventory);
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

        if (hotbarResult.DroppedItem != null)
        {
            var pos = localPlot.GetTileCoordsFromWorldPosition(Input.GetMousePosition());
            if (localPlot.CanPlaceAt(pos.Item1, pos.Item2, 1, 1))
            {
                CallServer_PlaceTurret(pos.Item1, pos.Item2, hotbarResult.DroppedItem.Id);
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

        if (!playerPlot.CanPlaceAt(x, y, 1, 1))
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
        turretEntity.Position = playerPlot.GetWorldPositionForTile(x, y);
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

    public override void OnDestroy()
    {
        if (Network.IsServer)
        {
            // Save.SetInt(this, "money", Money.Value);
            // Save.SetInt(this, "unlockedRows", UnlockedRows.Value);
            // Save.SetString(this, "inventory", InventoryData.Value);
        }
    }

    void ParseInventory()
    {
        LocalInventory.Clear();
        try
        {
            if (!string.IsNullOrEmpty(InventoryData.Value))
            {
                var items = JsonSerializer.Deserialize<List<CollectibleEnemy>>(InventoryData.Value);
                if (items != null)
                {
                    LocalInventory.AddRange(items);
                }
            }
        }
        catch
        {
            LocalInventory.Clear();
        }
    }


    public void AddToInventory(string enemyType)
    {
        if (!Network.IsServer)
            return;

        var collectible = CollectibleDatabase.GetCollectibleData(enemyType);
        LocalInventory.Add(collectible);
        
        var json = JsonSerializer.Serialize(LocalInventory);
        InventoryData.Set(json);
    }

    public void RemoveFromInventory(int index)
    {
        if (index < 0 || index >= LocalInventory.Count)
            return;

        LocalInventory.RemoveAt(index);
        var json = JsonSerializer.Serialize(LocalInventory);
        InventoryData.Set(json);
    }

    [ClientRpc]
    public void ShowGameOver()
    {
        Log.Info("Game Over!");
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
