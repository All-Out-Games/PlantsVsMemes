using AO;

public static class Config
{
    public const string Version = "_v1";
    
    // Save Keys
    public const string SaveKey_UnlockedRows = "unlockedRows" + Version;
    public const string SaveKey_UnlockedAreas = "unlockedAreas" + Version;
    public const string SaveKey_Inventory = "inventory" + Version;
    public const string SaveKey_ReceivedStarterPeashooter = "receivedStarterPeashooter" + Version;
    public const string SaveKey_PlacedTurrets = "placedTurrets" + Version;
    public const string SaveKey_PlacedCollectibles = "placedCollectibles" + Version;
    public const string SaveKey_RebirthLevel = "rebirthLevel" + Version;

    // Currencies
    public const string Currency_Gold = "gold";
    public const string Currency_GoldIcon = "gold.png";
    
    // Modifier Multipliers
    public const float Modifier_None_Multiplier = 1.0f;
    public const float Modifier_Gold_Multiplier = 1.5f;
    public const float Modifier_Diamond_Multiplier = 2.0f;
    public const float Modifier_Rainbow_Multiplier = 3.0f;
    public const float Modifier_Woods_Multiplier = 1.25f;
}