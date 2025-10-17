using AO;

public partial class TurretShop : Component
{
    public static TurretShop Instance;

    public float NextRotationTime = 0f;

    public Shop turretShop;
    public ShopCategory turretsCategory;
    
    float rotationInterval = 300f; // 5 minutes in seconds
    
    List<ShopCategory.ProductDescription> allTurretProducts = new List<ShopCategory.ProductDescription>();
    int currentRotationIndex = 0;

    public SyncVar<string> SerializedItemData = new SyncVar<string>("");

    public bool IsOpen = false;

    public ulong RNGSeed;

    public override void Awake()
    {
        CreateTurretShop();
        Instance = this;

        SerializedItemData.OnSync += OnSerializedItemDataSync;
    }

    void OnSerializedItemDataSync(string oldData, string newData)
    {
        turretsCategory.ClearProducts();
        var itemIds = newData.Split("|");
        foreach (var itemId in itemIds)
        {
            turretsCategory.AddProduct(allTurretProducts.FirstOrDefault(p => p.Id == itemId));
        }
    }

    public override void Update()
    {
        if (Network.IsServer)
        {
            if (Time.TimeSinceStartup >= NextRotationTime)
            {
                RNGSeed = (ulong)new Random().NextInt64();
                RotateTurrets(RNGSeed);
                NextRotationTime = Time.TimeSinceStartup + rotationInterval;
            }
        }

        if (Network.IsClient)
        {
            if (IsOpen)
            {
                IsOpen = turretShop.Draw(UI.ScreenRect);
            }
        }
    }

    void CreateTurretShop()
    {
        if (turretShop != null)
            return;

        turretShop = Economy.CreateShop("Turret Shop");
        
        if (Network.IsClient)
        {
            turretShop.SetCustomDisplay(CustomTurretShopDisplay);
        }
        
        if (Network.IsServer)
        {
            turretShop.SetPurchaseHandler(OnTurretPurchase);
        }

        turretsCategory = turretShop.AddCategory("Turrets");
        turretsCategory.Icon = "grass.png";

        // Create all turret products
        allTurretProducts.Add(CreateTurretProduct("peashooter", "Peashooter", "Basic shooter turret", 500));
        allTurretProducts.Add(CreateTurretProduct("cactus", "Cactus", "Long range spike shooter", 2500));
        allTurretProducts.Add(CreateTurretProduct("starfruit", "Starfruit", "Fast multi-directional shooter", 25_000));
        allTurretProducts.Add(CreateTurretProduct("melonpult", "Melonpult", "Heavy damage catapult", 100_000));
        allTurretProducts.Add(CreateTurretProduct("cobcannon", "Cobcannon", "Explosive artillery", 1_000_000));
    }

    ShopCategory.ProductDescription CreateTurretProduct(string turretType, string name, string description, int price)
    {
        var stats = TurretDatabase.GetStats(turretType);

        
        return new ShopCategory.ProductDescription(){
            Id = $"__TURRET__{turretType}",
            Name = name,
            Description = $"{description}\nRange: {stats.range}m | Damage: {stats.damage} | Cooldown: {stats.cooldown}s",
            Icon = "grass.png",
            Currency = Config.Currency_Gold,
            Price = price,
        };
    }

    public void RotateTurrets(ulong seed)
    {
        Util.Assert(Network.IsServer);

        // Select 2-3 random turrets for this rotation
        var availableProducts = new List<ShopCategory.ProductDescription>(allTurretProducts);
        int numToShow = RNG.RangeInt(ref seed, 2, 4); // 2 or 3 turrets

        var serializedItemData = new List<string>();
        
        // Shuffle and take first numToShow products
        for (int i = 0; i < numToShow && availableProducts.Count > 0; i++)
        {
            int randomIndex = RNG.RangeInt(ref seed, 0, availableProducts.Count-1);
            var selectedProduct = availableProducts[randomIndex];
            serializedItemData.Add(selectedProduct.Id);
            Log.Warn($"[TurretShop] Rotation {currentRotationIndex}: Added {selectedProduct.Name}");
        }

        currentRotationIndex++;
        SerializedItemData.Set(string.Join("|", serializedItemData));
        
        Log.Warn($"[TurretShop] Rotated turrets. Next rotation in {rotationInterval}s");
    }

    bool OnTurretPurchase(Player _player, GameProduct product)
    {
        var player = (MyPlayer)_player;

        if (player == null || !player.Entity.Alive())
            return false;

        var balance = Economy.GetBalance(player, Config.Currency_Gold);

        // Check if player can afford this turret
        long price = product.Price;
        if (balance < price)
        {
            Log.Info($"[TurretShop] Player {player.Name} cannot afford {product.Name} (cost: {price}, has: {balance})");
            return false;
        }

        // Extract turret type from product ID (format: __TURRET__<type>)
        string turretType = product.Id.Substring(10); // Remove "__TURRET__" prefix

        // Get the item definition for this turret
        Item_Definition turretItem = turretType switch
        {
            "peashooter" => MyPlayer.PeashooterItem,
            "cactus" => MyPlayer.CactusItem,
            "starfruit" => MyPlayer.StarfruitItem,
            "melonpult" => MyPlayer.MelonpultItem,
            "cobcannon" => MyPlayer.CobcannonItem,
            _ => null,
        };

        if (turretItem == null)
        {
            Log.Error($"[TurretShop] Unknown turret type: {turretType}");
            return false;
        }

        // Try to add the item to player's inventory
        var item = Inventory.CreateItem(turretItem, 1);
        if (Inventory.CanMoveItemToInventory(item, player.DefaultInventory, out var willDestroyItem))
        {
            Inventory.MoveItemToInventory(item, player.DefaultInventory);
            
            // Play purchase sound
            // SFX.Play(purchaseSound, new SFX.PlaySoundDesc { Positional = false, Volume = 0.5f });
            
            return true;
        }
        else
        {
            Inventory.DestroyItem(item);
            Log.Info($"[TurretShop] Player {player.Name} inventory full - cannot purchase {turretType}");
            return false;
        }
    }

    void CustomTurretShopDisplay(GameProduct product, Rect rect)
    {
        // You can customize the shop UI here if needed
        // For now, using default display
    }
}

