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
    
    // Rebirth Item Requirements - list of item IDs with quantities
    // Format: (itemId, quantity) - system will sum up quantities for each unique item
    public static readonly List<(string itemId, int quantity)> Rebirth1_Items = new List<(string, int)>
    {
        ("lirili_larila", 1),
        ("SchleemeriniMonsterini", 1)
    };
    
    public static readonly List<(string itemId, int quantity)> Rebirth2_Items = new List<(string, int)>
    {
        ("boneca_ambalabu", 1),
        ("cocofanto_elefanto", 1),
        ("BobritoBandito", 1)
    };
    
    public static readonly List<(string itemId, int quantity)> Rebirth3_Items = new List<(string, int)>
    {
        ("cappuccino_assassino", 1),
        ("brrbrr_patapim", 1),
        ("Smurfcat", 1)
    };
    
    public static readonly List<(string itemId, int quantity)> Rebirth4_Items = new List<(string, int)>
    {
        ("chimpanzini_bananini", 1),
        ("ballerina_cappuccina", 1),
        ("PipiPotato", 1)
    };
    
    public static readonly List<(string itemId, int quantity)> Rebirth5_Items = new List<(string, int)>
    {
        ("trigrrullini_watermellini", 1),
        ("zibra_zurbra_zibralini", 1)
    };
}