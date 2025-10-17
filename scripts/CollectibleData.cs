using AO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CollectibleEnemy
{
    [JsonPropertyName("type")]
    public string EnemyType { get; set; }
    
    [JsonPropertyName("w")]
    public int Width { get; set; }
    
    [JsonPropertyName("h")]
    public int Height { get; set; }
    
    [JsonPropertyName("mps")]
    public float MoneyPerSecond { get; set; }

    public CollectibleEnemy() { }

    public CollectibleEnemy(string type, int width, int height, float moneyPerSecond)
    {
        EnemyType = type;
        Width = width;
        Height = height;
        MoneyPerSecond = moneyPerSecond;
    }
}

public static class CollectibleDatabase
{
    static Dictionary<string, (int width, int height)> collectibleSizes = new Dictionary<string, (int, int)>();
    static bool isInitialized = false;

    public static void Initialize()
    {
        if (isInitialized)
            return;

        isInitialized = true;

        string debugStr = "";

        // Measure all brainrot collectibles
        foreach (var entry in BrainrotCatalog.Entries)
        {
            string collectibleId = entry.Key;
            var catalogEntry = entry.Value;
            
            // Create a temporary entity to measure sprite size
            var tempEntity = Entity.Create();
            var spriteRenderer = tempEntity.AddComponent<Sprite_Renderer>();
            spriteRenderer.Sprite = catalogEntry.Sprite;
            
            // Get world size of the sprite
            var worldSize = spriteRenderer.GetWorldSize();
            
            // Calculate tile dimensions (each tile is 1 unit)
            int tileWidth = Math.Max(1, (int)Math.Round(worldSize.X));
            int tileHeight = Math.Max(1, (int)Math.Round(worldSize.Y / 2));
            // int tileHeight = 1;
            
            // Store the size
            collectibleSizes[collectibleId] = (tileWidth, tileHeight);

            debugStr += $"{collectibleId}: {tileWidth}x{tileHeight}\n";
            
            // Clean up
            tempEntity.Destroy();
        }

        Log.Info(debugStr);

        // Log.Info($"CollectibleDatabase: Initialized {collectibleSizes.Count} collectible sizes");
    }

    public static (int width, int height) GetCollectibleSize(string collectibleId)
    {
        if (!isInitialized)
            Initialize();

        if (collectibleSizes.TryGetValue(collectibleId, out var size))
            return size;

        // Default size if not found
        return (1, 1);
    }

    public static string GetSpinePath(string enemyType)
    {
        return enemyType switch
        {
            "peashooter" => "anim/Peashooter/Peashooter.spine",
            "sunflower" => "anim/Sunflower/Sunflower.spine",
            "cactus" => "anim/Cactus/Cactus.spine",
            "walnut" => "anim/Walnut/Walnut.spine",
            "starfruit" => "anim/Starfruit/Starfruit.spine",
            "melonpult" => "anim/Melonpult/Melonpult.spine",
            "cobcannon" => "anim/Cobcannon/Cobcannon.spine",
            _ => "anim/Peashooter/Peashooter.spine",
        };
    }
}

public static class TurretDatabase
{
    public static bool IsTurret(string itemId)
    {
        return itemId switch
        {
            "peashooter" => true,
            "cactus" => true,
            "starfruit" => true,
            "melonpult" => true,
            "cobcannon" => true,
            _ => false,
        };
    }

    public static Vector2 GetPositionOffset(string turretType)
    {
        return turretType switch
        {
            "peashooter" => new Vector2(0, 0.25f),
            "cactus" => new Vector2(0, 0.25f),
            "starfruit" => new Vector2(0, 0),
            "melonpult" => new Vector2(0, 0.25f),
            _ => new Vector2(0, 0),
        };
    }
    public static float GetDepthOffset(string turretType)
    {
        return turretType switch
        {
            "peashooter" => -0.5f,
            "cactus" => -0.5f,
            "starfruit" => 0.5f,
            "melonpult" => -0.5f,
            _ => 0.0f,
        };
    }

    public static int GetCost(string turretType)
    {
        return turretType switch
        {
            "peashooter" => 50,
            "cactus" => 50,
            "starfruit" => 50,
            "melonpult" => 50,
            "cobcannon" => 50,
            _ => 50,
        };
    }

    public static string GetSpinePath(string turretType)
    {
        return turretType switch
        {
            "peashooter" => "anim/Peashooter/Peashooter.spine",
            "cactus" => "anim/Cactus/Cactus.spine",
            "starfruit" => "anim/Starfruit/Starfruit.spine",
            "melonpult" => "anim/Melonpult/Melonpult.spine",
            "cobcannon" => "anim/Cobcannon/Cobcannon.spine",
            _ => "anim/Peashooter/Peashooter.spine",
        };
    }

    public static string GetProjectileSpinePath(string turretType)
    {
        return turretType switch
        {
            "peashooter" => "anim/Peashooter/Peashooter_Projectile/Peashooter_Projectile.spine",
            "cactus" => "anim/Cactus/Cactus_Projectile/Cactus_Projectile.spine",
            "starfruit" => "anim/Starfruit/Starfruit_Projectile/Starfruit_Projectile.spine",
            "melonpult" => "anim/Melonpult/Melonpult_Projectile/Melonpult_Projectile.spine",
            "cobcannon" => "anim/Cobcannon/Cobcannon_Projectile/Cobcannon_Projectile.spine",
            _ => "anim/Peashooter/Peashooter_Projectile/Peashooter_Projectile.spine",
        };
    }

    public static (float range, float cooldown, int damage) GetStats(string turretType)
    {
        return turretType switch
        {
            "peashooter" => (10.0f, 1.5f, 25),
            "cactus"     => (10.0f, 0.5f, 250),
            "starfruit"  => (10.0f, 1.0f, 400),
            "melonpult"  => (10.0f, 3.0f, 1_000),
            "cobcannon"  => (10.0f, 5.0f, 10_000),
            _ => (6.0f, 1.5f, 10),
        };
    }
}

