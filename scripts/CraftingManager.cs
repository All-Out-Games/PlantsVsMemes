using AO;

public partial class CraftingSystem : Component
{
    public static CraftingSystem Instance;

    [NetSync] public int SecondsUntilNextRotation = 0;
    public float TimeUntilNextRotation = 0;
    
    public Window CraftingWindow;
    public bool IsCraftingWindowOpen = false;

    public int SelectedIndex = -1;

    public ulong RngSeed;

    public SyncVar<string> _availableCraftingRecipesSerialized = new SyncVar<string>("");
    public List<string> AvailableCraftingRecipes = new List<string>();

    public List<CraftingRecipe> CraftingRecipes = new List<CraftingRecipe>();

    public override void Awake()
    {
        Instance = this;
        RngSeed = RNG.Seed((ulong)new Random().NextInt64());

        _availableCraftingRecipesSerialized.OnSync += OnAvailableCraftingRecipesSync;
        
        // Initialize recipes dynamically from catalog
        InitializeCraftingRecipes();
        
        CraftingWindow = Window.Make(new WindowOptions(){
            Title = "Crafting",
            WindowBackgroundStyle = WindowBackgroundStyle.Simple,
            WindowBackgroundColor = WindowBackgroundColor.Brown,
            WindowTitleFlare = WindowTitleFlare.Green,
            Content = new SplitWindowContent() {
                Axis = SplitAxis.Horizontal,
                SplitRatio = 0.4f,
                First = new CustomWindowContent() {
                    DrawContent = (rect) => {
                        var bgRect = rect.Inset(10, 5, 10, 10);

                        var bgTexture = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/text_backer_black.png");
                        UI.Image(bgRect, bgTexture, new UI.NineSlice() { slice = new Vector4(34, 34, 34, 34), sliceScale = 1.0f});

                        bgRect = bgRect.Inset(10);

                        var sv = UI.PushScrollView("crafting", bgRect, new UI.ScrollViewSettings() { Vertical = true });
                        for (int i = 0; i < CraftingRecipes.Count; i++)
                        {
                            var recipe = CraftingRecipes[i];
                            if (AvailableCraftingRecipes.Contains(recipe.Result) == false) {
                                if (SelectedIndex == i) SelectedIndex = -1;
                                continue;
                            }

                            var brainrotEntry = BrainrotCatalog.Get(recipe.Result);
                            var itemRect = sv.contentRect.CutTop(200);
                            var texture = brainrotEntry.Rarity switch {
                                BrainrotValueRarity.Common => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                                BrainrotValueRarity.Rare => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_green.png"),
                                BrainrotValueRarity.Epic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_blue.png"),
                                BrainrotValueRarity.Legendary => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_yellow.png"),
                                BrainrotValueRarity.Mythic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_red.png"),
                                BrainrotValueRarity.Ethereal => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_white.png"),
                                _ => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                            };

                            var result = UI.BeginButton(itemRect, brainrotEntry.Name, new UI.ButtonSettings(), new UI.TextSettings());
                            UI.Image(itemRect, texture, new UI.NineSlice() { slice = new Vector4(34, 34, 34, 34), sliceScale = 1.0f});
                            
                            var iconRect = itemRect.CutLeft(150).Inset(20);
                            UI.Image(iconRect.FitAspect(brainrotEntry.Sprite.Aspect), brainrotEntry.Sprite);

                            var textRect = itemRect.Inset(20, 20, 20, 0);
                            var ts = UI.TextSettingsForButtons;
                            ts.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            ts.WordWrap = true;
                            UI.TextAsync(textRect, brainrotEntry.Name, ts);
                            
                            UI.EndButton();

                            if (result.Clicked)
                            {
                                SelectedIndex = i;
                            }

                            sv.contentRect.CutTop(10);
                        }
                        UI.PopScrollView();
                    }
                },
                Second = new CustomWindowContent() {
                    DrawContent = (rect) => {
                        var bgRect = rect.Inset(10, 10, 10, 5);
                        var bgTexture = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/text_backer_black.png");
                        UI.Image(bgRect, bgTexture, new UI.NineSlice() { slice = new Vector4(34, 34, 34, 34), sliceScale = 1.0f});

                        bgRect = bgRect.Inset(10);

                        if (SelectedIndex < 0 || SelectedIndex >= CraftingRecipes.Count)
                        {
                            var ts = UI.TextSettingsForButtons;
                            ts.HorizontalAlignment = UI.HorizontalAlignment.Center;
                            ts.VerticalAlignment = UI.VerticalAlignment.Center;
                            UI.TextAsync(bgRect, "Select a recipe on the left", ts);
                            return;
                        }

                        var recipe = CraftingRecipes[SelectedIndex];
                        var resultEntry = BrainrotCatalog.Get(recipe.Result);

                        // Result header
                        var headerRect = bgRect.CutTop(160).Inset(10);
                        var bannerTex = resultEntry.Rarity switch {
                            BrainrotValueRarity.Common => Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                            BrainrotValueRarity.Rare => Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_green.png"),
                            BrainrotValueRarity.Epic => Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_blue.png"),
                            BrainrotValueRarity.Legendary => Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_yellow.png"),
                            BrainrotValueRarity.Mythic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_red.png"),
                            BrainrotValueRarity.Ethereal => Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_white.png"),
                            _ => Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                        };
                        UI.Image(headerRect, bannerTex, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});

                        var iconRect = headerRect.CutLeft(150).Inset(10);
                        UI.Image(iconRect.FitAspect(resultEntry.Sprite.Aspect), resultEntry.Sprite);

                        var nameRect = headerRect.Inset(10, 10, 10, 10);
                        var nameTs = UI.TextSettingsForButtons;
                        nameTs.HorizontalAlignment = UI.HorizontalAlignment.Left;
                        nameTs.VerticalAlignment = UI.VerticalAlignment.Center;
                        nameTs.WordWrap = true;
                        UI.TextAsync(nameRect, resultEntry.Name, nameTs);

                        bgRect.CutTop(10);

                        // Ingredients list
                        var listRect = bgRect.CutTop(MathF.Min(bgRect.Height - 140, 300)).Inset(5);
                        var haveAllIngredients = true;

                        foreach (var ingredientId in recipe.Ingredients)
                        {
                            var rowRect = listRect.CutTop(80).Inset(5);
                            var ingEntry = BrainrotCatalog.Get(ingredientId);

                            var rowBg = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/ranking_backing/rectangle_backings/rectangle_backing_black.png");
                            UI.Image(rowRect, rowBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});

                            var ingIconRect = rowRect.CutLeft(90).Inset(8);
                            UI.Image(ingIconRect.FitAspect(ingEntry.Sprite.Aspect), ingEntry.Sprite);

                            int haveCount = Network.LocalPlayer.DefaultInventory.Items.Count(itm => itm != null && itm.Definition.Id == ingredientId);
                            int needCount = 1;
                            bool have = haveCount >= needCount;
                            if (!have) haveAllIngredients = false;

                            var textRect = rowRect.Inset(8, 8, 8, 0);
                            var ts = UI.TextSettingsForButtons;
                            ts.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            ts.VerticalAlignment = UI.VerticalAlignment.Center;
                            ts.WordWrap = true;
                            ts.Color = have ? new Vector4(0.5f, 1.0f, 0.5f, 1) : new Vector4(1.0f, 0.5f, 0.5f, 1);
                            UI.TextAsync(textRect, $"{ingEntry.Name}  {haveCount}/{needCount}", ts);

                            listRect.CutTop(8);
                        }

                        // Optional currency requirement
                        bool currencyOk = true;
                        if (string.IsNullOrEmpty(recipe.Currency) == false && recipe.Cost > 0)
                        {
                            var currencyRow = bgRect.CutTop(80).Inset(5);
                            var rowBg = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/ranking_backing/rectangle_backings/rectangle_backing_black.png");
                            UI.Image(currencyRow, rowBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});

                            var coinRect = currencyRow.CutLeft(90).Inset(8);
                            var coinTex = Assets.GetAsset<Texture>("gold.png");
                            UI.Image(coinRect.FitAspect(coinTex.Aspect), coinTex);

                            var balance = Economy.GetBalance(Network.LocalPlayer, recipe.Currency);
                            long costLong = (long)recipe.Cost;
                            currencyOk = balance >= costLong;

                            var ts = UI.TextSettingsForButtons;
                            ts.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            ts.VerticalAlignment = UI.VerticalAlignment.Center;
                            ts.WordWrap = true;
                            ts.Color = currencyOk ? new Vector4(0.5f, 1.0f, 0.5f, 1) : new Vector4(1.0f, 0.5f, 0.5f, 1);
                            UI.TextAsync(currencyRow.Inset(8, 8, 8, 0), $"Gold  {StringUtils.FormatMoney(balance)}/{StringUtils.FormatMoney(costLong)}", ts);

                            bgRect.CutTop(10);
                        }

                        // Craft button
                        var canCraft = haveAllIngredients && currencyOk;
                        var buttonRect = bgRect.BottomRect().Inset(0, 50, 0, 50).GrowTop(80).Offset(0, 20);
                        var craftIcon = resultEntry.Sprite;
                        var buttonResult = UI.DrawWindowButton(buttonRect, "Craft", craftIcon, canCraft ? ButtonColor.Green : ButtonColor.Grey, ButtonStyle.BasicShiny);
                        if (buttonResult.Clicked && canCraft)
                        {
                            CallServer_RequestCraft(recipe.Result);
                        }
                    }
                }
            }
        });
    }

    void InitializeCraftingRecipes()
    {
        CraftingRecipes.Clear();
        
        // Get some brainrots by rarity for recipe creation
        var commons = GetBrainrotsByRarity(BrainrotValueRarity.Common).Take(5).ToList();
        var rares = GetBrainrotsByRarity(BrainrotValueRarity.Rare).Take(5).ToList();
        var epics = GetBrainrotsByRarity(BrainrotValueRarity.Epic).Take(4).ToList();
        var legendaries = GetBrainrotsByRarity(BrainrotValueRarity.Legendary).Take(3).ToList();
        var mythics = GetBrainrotsByRarity(BrainrotValueRarity.Mythic).Take(2).ToList();
        
        // Recipe pattern: 3 lower rarity → 1 higher rarity + gold cost
        
        // Commons → Rare
        if (commons.Count >= 3 && rares.Count >= 1)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = rares[0],
                Ingredients = new List<string> { commons[0], commons[1], commons[2] },
                Currency = Config.Currency_Gold,
                Cost = 5000
            });
        }
        
        // Commons → Rare (alt)
        if (commons.Count >= 5 && rares.Count >= 2)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = rares[1],
                Ingredients = new List<string> { commons[3], commons[4], commons[0] },
                Currency = Config.Currency_Gold,
                Cost = 7500
            });
        }
        
        // Rares → Epic
        if (rares.Count >= 3 && epics.Count >= 1)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = epics[0],
                Ingredients = new List<string> { rares[0], rares[1], rares[2] },
                Currency = Config.Currency_Gold,
                Cost = 50000
            });
        }
        
        // Rares → Epic (alt)
        if (rares.Count >= 5 && epics.Count >= 2)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = epics[1],
                Ingredients = new List<string> { rares[3], rares[4], rares[0] },
                Currency = Config.Currency_Gold,
                Cost = 75000
            });
        }
        
        // Epics → Legendary
        if (epics.Count >= 3 && legendaries.Count >= 1)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = legendaries[0],
                Ingredients = new List<string> { epics[0], epics[1], epics[2] },
                Currency = Config.Currency_Gold,
                Cost = 500000
            });
        }
        
        // Epics → Legendary (alt)
        if (epics.Count >= 4 && legendaries.Count >= 2)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = legendaries[1],
                Ingredients = new List<string> { epics[1], epics[2], epics[3] },
                Currency = Config.Currency_Gold,
                Cost = 750000
            });
        }
        
        // Legendaries → Mythic
        if (legendaries.Count >= 3 && mythics.Count >= 1)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = mythics[0],
                Ingredients = new List<string> { legendaries[0], legendaries[1], legendaries[2] },
                Currency = Config.Currency_Gold,
                Cost = 5000000
            });
        }
        
        // Legendaries → Mythic (alt)
        if (legendaries.Count >= 3 && mythics.Count >= 2)
        {
            CraftingRecipes.Add(new CraftingRecipe() {
                Result = mythics[1],
                Ingredients = new List<string> { legendaries[1], legendaries[2], legendaries[0] },
                Currency = Config.Currency_Gold,
                Cost = 7500000
            });
        }
    }
    
    List<string> GetBrainrotsByRarity(BrainrotValueRarity rarity)
    {
        var result = new List<string>();
        foreach (var entry in BrainrotCatalog.Entries)
        {
            if (entry.Value.Rarity == rarity && !entry.Value.IsEvolution)
            {
                result.Add(entry.Key);
            }
        }
        return result;
    }

    public void OnAvailableCraftingRecipesSync(string oldSerialized, string newSerialized)
    {
        if (newSerialized.IsNullOrEmpty())
        {
            AvailableCraftingRecipes = new List<string>();
            return;
        }
        AvailableCraftingRecipes = newSerialized.Split(',').ToList();
    }

    [ServerRpc]
    public static void RequestCraft(string recipeId)
    {
        var recipe = Instance.CraftingRecipes.Find(r => r.Result == recipeId);
        if (recipe == null) return;

        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null) return;

        var itemDef = BrainrotCatalog.GetItemDefinition(recipeId);
        if (itemDef == null) return;

        bool canCraft = true;
        List<string> itemsInInventory = new List<string>();
        foreach (var itemInstance in player.DefaultInventory.Items)
        {
            if (itemInstance != null)
            {
                itemsInInventory.Add(itemInstance.Definition.Id);
            }
        }

        foreach (var ingredientId in recipe.Ingredients)
        {
            if (itemsInInventory.Contains(ingredientId) == false)
            {
                canCraft = false;
                break;
            }

            itemsInInventory.Remove(ingredientId);
        }

        if (canCraft == false) return;

        if (string.IsNullOrEmpty(recipe.Currency) == false && recipe.Cost > 0)
        {
            var balance = Economy.GetBalance(player, recipe.Currency);
            if (balance < recipe.Cost)
            {
                canCraft = false;
            }
        }

        if (canCraft == false) return;

        foreach (var ingredientId in recipe.Ingredients)
        {
            var itemInstance = player.DefaultInventory.Items.FirstOrDefault(i => i != null && i.Definition.Id == ingredientId);
            if (itemInstance == null) return;
            Inventory.DestroyItem(itemInstance);
        }

        if (string.IsNullOrEmpty(recipe.Currency) == false && recipe.Cost > 0)
        {
            Economy.WithdrawCurrency(player, recipe.Currency, recipe.Cost);
        }

        var resultEntry = BrainrotCatalog.Get(recipeId);
        var newItem = Inventory.CreateItem(itemDef, 1);
        if (Inventory.CanMoveItemToInventory(newItem, player.DefaultInventory, out var willDestroyItem))
        {
            Inventory.MoveItemToInventory(newItem, player.DefaultInventory);
            player.CallClient_ShowMessage($"Crafted {resultEntry.Name}!", new RPCOptions() { Target = player });
        }
        else
        {
            Inventory.DestroyItem(newItem);
            player.CallClient_ShowMessage("Inventory is full!", new RPCOptions() { Target = player });
        }
    }

    public override void Update()
    {
        // Server-only: rotate available crafting recipes every 30 minutes
        if (Network.IsServer)
        {
            TimeUntilNextRotation -= Time.DeltaTime;
            SecondsUntilNextRotation = (int)MathF.Ceiling(TimeUntilNextRotation);

            if (TimeUntilNextRotation <= 0)
            {
                TimeUntilNextRotation = 60*30;
                SecondsUntilNextRotation = 60*30;

                // Build candidate pools by rarity
                List<string> epic = new List<string>();
                List<string> legendary = new List<string>();
                List<string> mythic = new List<string>();
                
                for (int i = 0; i < CraftingRecipes.Count; i++)
                {
                    var r = CraftingRecipes[i];
                    var entry = BrainrotCatalog.Get(r.Result);
                    switch (entry.Rarity)
                    {
                        case BrainrotValueRarity.Epic: epic.Add(r.Result); break;
                        case BrainrotValueRarity.Legendary: legendary.Add(r.Result); break;
                        case BrainrotValueRarity.Mythic: mythic.Add(r.Result); break;
                    }
                }

                List<string> upcoming = new List<string>();
                
                // Always include 1 epic if available
                if (epic.Count > 0)
                {
                    int idx = RNG.RangeInt(ref RngSeed, 0, epic.Count-1);
                    upcoming.Add(epic[idx]);
                }
                
                // Always include 1 legendary if available
                if (legendary.Count > 0)
                {
                    int idx = RNG.RangeInt(ref RngSeed, 0, legendary.Count-1);
                    upcoming.Add(legendary[idx]);
                }
                
                // 30% chance to include a mythic
                if (mythic.Count > 0)
                {
                    float roll = RNG.RangeFloat(ref RngSeed, 0f, 1f);
                    if (roll < 0.30f)
                    {
                        int idx = RNG.RangeInt(ref RngSeed, 0, mythic.Count-1);
                        var m = mythic[idx];
                        if (upcoming.Contains(m) == false) upcoming.Add(m);
                    }
                }

                // Stable order for consistency
                upcoming.Sort(StringComparer.Ordinal);
                string csv = string.Join(",", upcoming);
                if (_availableCraftingRecipesSerialized.Value != csv)
                {
                    _availableCraftingRecipesSerialized.Set(csv);
                }
            }
        }

        if (IsCraftingWindowOpen)
        {
            IsCraftingWindowOpen = UI.DrawWindow(CraftingWindow);
        }
    }

    public void OpenCraftingWindow()
    {
        var localPlayer = Network.LocalPlayer as MyPlayer;
        if (localPlayer.RebirthLevel.Value < 1) 
        {
            Notifications.Show("You need to be at least Rebirth Level 1 to craft!");
            return;
        }

        IsCraftingWindowOpen = true;
    }

    public class CraftingRecipe
    {
        public List<string> Ingredients;
        public string Result;
        public string Currency;
        public long Cost;
    }
}
