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
            "cactus" => 75,
            "starfruit" => 100,
            "melonpult" => 150,
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
            "peashooter" => (6.0f, 1.5f, 10),
            "cactus" => (8.0f, 2.0f, 15),
            "starfruit" => (5.0f, 1.0f, 8),
            "melonpult" => (10.0f, 3.0f, 30),
            "cobcannon" => (10.0f, 3.0f, 30),
            _ => (6.0f, 1.5f, 10),
        };
    }
}

