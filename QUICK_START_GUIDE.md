# Quick Start Guide - Rebirth, Fusion, Crafting & Modifiers

## ✅ Build Status: 0 Errors, 0 Warnings

## Overview

All four systems are now **fully implemented** and dynamically pull from the **BrainrotCatalog**:

1. **Modifiers** - Affect enemy drops and collectible earnings
2. **Fusion** - Combine duplicates to evolve brainrots
3. **Rebirth** - Permanent earnings boosts via sacrifice
4. **Crafting** - Combine lower-tier brainrots to create higher-tier ones

---

## 🎮 How to Use In-Game

### HUD Buttons (Already Added!)

Five buttons appear in a horizontal row at the center-top of the screen:

1. **Shop** (Green) - Opens turret shop
2. **Fusion** (Blue) - Opens fusion window to evolve collectibles
3. **Crafting** (Green) - Opens crafting window (requires Rebirth Level 1)
4. **Rebirth** (Red) - Opens rebirth window for permanent upgrades
5. **Sell** (Red) - Opens sell window to liquidate items for gold

Simply click any button to open its window!

### Opening Windows Programmatically

**Fusion Window:**
```csharp
FusionSystem.Instance.OpenFusionWindow();
```

**Rebirth Window:**
```csharp
Rebirth.Instance.OpenRebirthWindow();
```

**Crafting Window:**
```csharp
CraftingSystem.Instance.OpenCraftingWindow();
```

**Sell Window:**
```csharp
Sell.Instance.OpenSellWindow();
```

---

## 📊 System Details

### Modifiers (Auto-Applied)
- Assigned randomly when enemies spawn
- Stored in item metadata
- Applied when collectibles are placed
- Multipliers:
  - Gold: **1.5x** earnings
  - Diamond: **2.0x** earnings
  - Rainbow: **3.0x** earnings
  - Woods: **1.25x** earnings

### Fusion (Evolution)
- **Full Window UI** with split-panel design
- Click **"Fusion Lab"** button (blue) in HUD
- Uses existing `BrainrotCatalog.EvolutionMap`
- Requires `BasePartsToEvolve` copies to evolve
- Shows before/after preview with sprites
- Creates evolved brainrot with higher rarity
- Plays evolution sound on success

### Rebirth (Permanent Progression)
- Click **"⟳ Rebirth ⟳"** button (red) in HUD
- **5 tiers** with increasing requirements
- Each tier requires **gold + specific rarity brainrots**
- Requirements **randomly generated** from catalog on init
- **1.25x earnings multiplier per rebirth** (stacking)
- Players **keep everything** - just pay the cost
- Full Window UI with progress tracking and celebration animation

### Crafting (Rarity Upgrade)
- Click **"Crafting"** button (green) in HUD
- **8+ recipes** dynamically generated from catalog
- Pattern: **3 brainrots → 1 higher rarity brainrot**
- Recipes **rotate every 30 minutes** on server
- Requires **Rebirth Level 1** to access (button greys out before then)
- Full split-panel Window UI with scrolling

### Sell (Item Liquidation)
- Click **"Sell"** button (red) in HUD
- **Two tabs**: Brainrots and Turrets
- **Brainrots**: Sell for 50% of base price (modifiers increase value!)
- **Turrets**: Sell for 50% of purchase cost
- Individual sell buttons or "Sell All" per tab
- Shows item details, stats, and sell value
- Color-coded by rarity for easy browsing

---

## 🔧 Configuration

### Modify Rebirth Tiers
Edit `Rebirth.InitializeRebirthConfigs()` in `scripts/RebirthManager.cs`:
```csharp
RebirthConfigs.Add(new RebirthConfig() {
    ItemRequirements = GetRandomBrainrotsByRarity(rarity, count),
    RequiredQuantityPerItem = 10,
    CurrencyRequirement = Config.Currency_Gold,
    CurrencyRequirementAmount = 50000,
    Rewards = new List<RebirthReward>(){
        new RebirthReward() { Text = "x1.25 Earnings" },
    },
});
```

### Modify Crafting Recipes
Edit `CraftingSystem.InitializeCraftingRecipes()` in `scripts/CraftingManager.cs`:
```csharp
CraftingRecipes.Add(new CraftingRecipe() {
    Result = targetBrainrotId,
    Ingredients = new List<string> { brainrot1, brainrot2, brainrot3 },
    Currency = Config.Currency_Gold,
    Cost = 5000
});
```

### Modify Modifier Multipliers
Edit values in `Config.cs`:
```csharp
public const float Modifier_Gold_Multiplier = 1.5f;
public const float Modifier_Diamond_Multiplier = 2.0f;
public const float Modifier_Rainbow_Multiplier = 3.0f;
public const float Modifier_Woods_Multiplier = 1.25f;
```

---

## 📁 Key Files

### Core Systems
- `scripts/RebirthManager.cs` - Rebirth system (named `Rebirth`)
- `scripts/CraftingManager.cs` - Crafting system (named `CraftingSystem`)
- `scripts/FusionManager.cs` - Fusion system
- `scripts/BrainrotCatalog.cs` - Data source for all brainrots

### Modified Files
- `scripts/Config.cs` - Constants and save keys
- `scripts/Enemy.cs` - Modifier metadata on drops
- `scripts/PlacedCollectible.cs` - Modifier application and display
- `scripts/MyPlayer.cs` - Rebirth level and multiplier
- `scripts/GameManager.cs` - System initialization
- `scripts/ui/GameHUD.cs` - Rebirth level display

---

## ✨ Features Summary

✅ **Modifiers** - Visual indicators, earnings boost, persistence  
✅ **Fusion** - Full Window UI, evolution with sound effects  
✅ **Rebirth** - Full Window UI, dynamic requirements, permanent boosts  
✅ **Crafting** - Full Window UI, rotating recipes, rarity upgrades  
✅ **Sell** - Full Window UI, tabbed design, modifier-aware pricing  
✅ **Integration** - All systems work together seamlessly  
✅ **HUD** - 5 accessible buttons in clean grid layout  
✅ **Persistence** - Everything saves and loads correctly  

**The MVP is complete and ready to play!** 🚀

