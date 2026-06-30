# Brainrot Inventory System - Usage Guide

## Overview

Item_Definitions have been created for every Brainrot in the BrainrotCatalog. These Item_Definitions allow you to add Brainrots to player inventories and manage them as items.

## What Was Created

### 1. BrainrotCatalog Changes (`scripts/BrainrotCatalog.cs`)

- **ItemDefinitions Dictionary**: A dictionary mapping brainrot IDs to their Item_Definitions
- **InitializeItemDefinitions()**: Automatically called during static initialization to create Item_Definitions for all Brainrots
- **GetItemDefinition(string brainrotId)**: Retrieve an Item_Definition by brainrot ID
- **GrantBrainrotToPlayer(MyPlayer player, string brainrotId, int quantity)**: Helper method to grant Brainrot items to players

### 2. MyPlayer Changes (`scripts/MyPlayer.cs`)

- Added static constructor to initialize turret Item_Definitions
- Properly initialized all turret items (Peashooter, Cactus, Starfruit, Melonpult, Cobcannon)

## How to Use

### Get a Brainrot Item Definition

```csharp
// Get the Item_Definition for a specific Brainrot
var itemDef = BrainrotCatalog.GetItemDefinition("cappuccino_assassino");

if (itemDef != null)
{
    Log.Info($"Found item: {itemDef.Name}");
}
```

### Grant a Brainrot to a Player (Server-Side)

```csharp
[ServerRpc]
public void GivePlayerBrainrot()
{
    var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
    
    // Grant 3 Cappuccino Assassino to the player
    bool success = BrainrotCatalog.GrantBrainrotToPlayer(player, "cappuccino_assassino", 3);
    
    if (success)
    {
        Log.Info("Successfully granted Brainrot to player!");
    }
}
```

### Create Items Manually

```csharp
// Server-side only
if (Network.IsServer)
{
    var itemDef = BrainrotCatalog.GetItemDefinition("chimpanzini_bananini");
    var item = Inventory.CreateItem(itemDef, 1);
    
    // Check if we can add it to inventory
    if (Inventory.CanMoveItemToInventory(item, player.DefaultInventory, out var willDestroy))
    {
        Inventory.MoveItemToInventory(item, player.DefaultInventory);
    }
    else
    {
        // Inventory full, destroy the item
        Inventory.DestroyItem(item);
    }
}
```

### List All Available Brainrot Items

```csharp
foreach (var kv in BrainrotCatalog.ItemDefinitions)
{
    Log.Info($"Brainrot ID: {kv.Key}, Name: {kv.Value.Name}");
}
```

## Available Brainrot IDs (Examples)

Here are some example Brainrot IDs you can use:

- Common: `lirili_larila`, `pipi_corni`, `fluriflura`, `tim_cheese`
- Rare: `boneca_ambalabu`, `cocofanto_elefanto`, `tung_tung`, `ta_ta_sahur`
- Epic: `cappuccino_assassino`, `brrbrr_patapim`, `avocadini_antilopini`
- Legendary: `chimpanzini_bananini`, `ballerina_cappucina`, `chef_crabracadabra`, `pipi_potato`
- Mythic: `zibra_zurbra_zibralini`, `trigrrullini_watermellini`, `karkerkar_kurkur`, `dragon_cannelloni`

Check `BrainrotCatalog.Entries` for the complete list of all available Brainrots.

## Item Properties

Each Brainrot Item_Definition has:
- **Id**: The brainrot catalog ID (e.g., "cappuccino_assassino")
- **Name**: Display name (e.g., "Cappuccino Assassino")
- **Icon**: Path to the texture (auto-generated, may need manual adjustment)
- **StackSize**: 99 (can stack up to 99 of each brainrot)

## Notes

1. All Item_Definitions are created automatically when BrainrotCatalog is initialized
2. The helper method `GrantBrainrotToPlayer` handles inventory checks automatically
3. Item icons are generated based on common texture paths - if an icon doesn't appear, check the texture path in the InitializeItemDefinitions() method
4. Always perform inventory operations on the server side

## Total Items Created

The system creates Item_Definitions for **all Brainrots** in the catalog, including:
- Base forms
- Evolution forms
- Event-exclusive Brainrots
- Lucky blocks
- Special variants

Total: **200+ Item_Definitions** automatically generated!

