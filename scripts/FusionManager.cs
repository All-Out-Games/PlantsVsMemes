using AO;

public partial class FusionSystem : System<FusionSystem>
{
    public Window FusionWindow;
    public bool IsFusionWindowOpen = false;
    public int SelectedIndex = -1;

    public override void Awake()
    {
        FusionWindow = Window.Make(new WindowOptions(){
            Title = "Fusion Lab",
            WindowBackgroundColor = WindowBackgroundColor.Blue,
            WindowBackgroundStyle = WindowBackgroundStyle.Simple,
            WindowTitleFlare = WindowTitleFlare.Blue,
            Content = new SplitWindowContent() {
                Axis = SplitAxis.Horizontal,
                SplitRatio = 0.4f,
                First = new CustomWindowContent() {
                    DrawContent = (rect) => {
                        var bgRect = rect.Inset(10, 5, 10, 10);

                        var bgTexture = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/text_backer_black.png");
                        UI.Image(bgRect, bgTexture, new UI.NineSlice() { slice = new Vector4(34, 34, 34, 34), sliceScale = 1.0f});

                        bgRect = bgRect.Inset(10);

                        var player = (MyPlayer)Network.LocalPlayer;
                        if (player == null) return;

                        // Count collectibles in inventory
                        var collectibleCounts = new Dictionary<string, int>();
                        foreach (var item in player.DefaultInventory.Items)
                        {
                            if (item != null && BrainrotCatalog.Entries.ContainsKey(item.Definition.Id))
                            {
                                if (!collectibleCounts.ContainsKey(item.Definition.Id))
                                    collectibleCounts[item.Definition.Id] = 0;
                                collectibleCounts[item.Definition.Id]++;
                            }
                        }

                        // Get list of fusible collectibles
                        var fusibleList = new List<(string id, int count, int required, string evolutionId)>();
                        foreach (var kv in collectibleCounts)
                        {
                            string collectibleId = kv.Key;
                            int count = kv.Value;
                            
                            if (BrainrotCatalog.TryGetEvolution(collectibleId, out string evolutionId))
                            {
                                var entry = BrainrotCatalog.Get(collectibleId);
                                int requiredCount = entry.BasePartsToEvolve;
                                fusibleList.Add((collectibleId, count, requiredCount, evolutionId));
                            }
                        }
                        
                        // Sort by rarity (highest to lowest)
                        fusibleList.Sort((a, b) => {
                            var rarityA = BrainrotCatalog.Get(a.id).Rarity;
                            var rarityB = BrainrotCatalog.Get(b.id).Rarity;
                            return rarityA.CompareTo(rarityB); // Descending order
                        });

                        if (fusibleList.Count == 0)
                        {
                            var ts = UI.TextSettingsForButtons;
                            ts.HorizontalAlignment = UI.HorizontalAlignment.Center;
                            ts.VerticalAlignment = UI.VerticalAlignment.Center;
                            UI.TextAsync(bgRect, "No fusible collectibles in inventory", ts);
                            return;
                        }

                        // Display scrollable list of fusible items
                        var sv = UI.PushScrollView("fusion", bgRect, new UI.ScrollViewSettings() { Vertical = true });
                        
                        for (int i = 0; i < fusibleList.Count; i++)
                        {
                            var (collectibleId, count, required, evolutionId) = fusibleList[i];
                            var entry = BrainrotCatalog.Get(collectibleId);
                            
                            var itemRect = sv.contentRect.CutTop(180);
                            
                            // Rarity-based background color
                            var texture = entry.Rarity switch {
                                BrainrotValueRarity.Common => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                                BrainrotValueRarity.Rare => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_green.png"),
                                BrainrotValueRarity.Epic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_blue.png"),
                                BrainrotValueRarity.Legendary => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_yellow.png"),
                                BrainrotValueRarity.Mythic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_red.png"),
                                _ => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                            };

                            var result = UI.BeginButton(itemRect, entry.Name, new UI.ButtonSettings(), new UI.TextSettings());
                            UI.Image(itemRect, texture, new UI.NineSlice() { slice = new Vector4(34, 34, 34, 34), sliceScale = 1.0f});
                            
                            // Brainrot icon
                            var iconRect = itemRect.CutLeft(140).Inset(15);
                            UI.Image(iconRect.FitAspect(entry.Sprite.Aspect), entry.Sprite);

                            // Name and progress
                            var textRect = itemRect.Inset(15, 15, 15, 0);
                            var ts = UI.TextSettingsForButtons;
                            ts.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            ts.VerticalAlignment = UI.VerticalAlignment.Top;
                            ts.WordWrap = true;
                            ts.Size = 32f;
                            UI.TextAsync(textRect.CutTop(50), entry.Name, ts);
                            
                            // Progress count
                            bool canFuse = count >= required;
                            var progressTs = ts with { 
                                Size = 24f, 
                                Color = canFuse ? new Vector4(0.5f, 1.0f, 0.5f, 1) : new Vector4(1.0f, 0.6f, 0.6f, 1) 
                            };
                            UI.TextAsync(textRect.CutTop(85), $"Copies: {count}/{required}", progressTs);
                            
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

                        var player = (MyPlayer)Network.LocalPlayer;
                        if (player == null) return;

                        // Count collectibles in inventory
                        var collectibleCounts = new Dictionary<string, int>();
                        foreach (var item in player.DefaultInventory.Items)
                        {
                            if (item != null && BrainrotCatalog.Entries.ContainsKey(item.Definition.Id))
                            {
                                if (!collectibleCounts.ContainsKey(item.Definition.Id))
                                    collectibleCounts[item.Definition.Id] = 0;
                                collectibleCounts[item.Definition.Id]++;
                            }
                        }

                        // Get fusible list
                        var fusibleList = new List<(string id, int count, int required, string evolutionId)>();
                        foreach (var kv in collectibleCounts)
                        {
                            string collectibleId = kv.Key;
                            int count = kv.Value;
                            
                            if (BrainrotCatalog.TryGetEvolution(collectibleId, out string evolutionId))
                            {
                                var entry = BrainrotCatalog.Get(collectibleId);
                                int requiredCount = entry.BasePartsToEvolve;
                                fusibleList.Add((collectibleId, count, requiredCount, evolutionId));
                            }
                        }
                        
                        // Sort by rarity (highest to lowest)
                        fusibleList.Sort((a, b) => {
                            var rarityA = BrainrotCatalog.Get(a.id).Rarity;
                            var rarityB = BrainrotCatalog.Get(b.id).Rarity;
                            return rarityA.CompareTo(rarityB); // Descending order
                        });

                        if (SelectedIndex < 0 || SelectedIndex >= fusibleList.Count)
                        {
                            var ts = UI.TextSettingsForButtons;
                            ts.HorizontalAlignment = UI.HorizontalAlignment.Center;
                            ts.VerticalAlignment = UI.VerticalAlignment.Center;
                            UI.TextAsync(bgRect, "Select a collectible on the left to fuse", ts);
                            return;
                        }

                        var selected = fusibleList[SelectedIndex];
                        var sourceEntry = BrainrotCatalog.Get(selected.id);
                        var targetEntry = BrainrotCatalog.Get(selected.evolutionId);

                        // Evolution preview section
                        var previewRect = bgRect.CutTop(280f).Inset(10);
                        var originalPreviewRectWidth = previewRect.Width;

                        // Source collectible
                        var sourceRect = previewRect.CutLeftUnscaled(originalPreviewRectWidth * 0.4f).Inset(5);
                        var sourceBg = sourceEntry.Rarity switch {
                            BrainrotValueRarity.Common => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                            BrainrotValueRarity.Rare => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_green.png"),
                            BrainrotValueRarity.Epic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_blue.png"),
                            BrainrotValueRarity.Legendary => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_yellow.png"),
                            BrainrotValueRarity.Mythic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_red.png"),
                            _ => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                        };
                        UI.Image(sourceRect, sourceBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});
                        
                        var sourceIconRect = sourceRect.TopRect().CutTop(180).Inset(20);
                        UI.Image(sourceIconRect.FitAspect(sourceEntry.Sprite.Aspect), sourceEntry.Sprite);
                        
                        var sourceNameRect = sourceRect.BottomRect().CutBottom(70).Inset(10);
                        var sourceTs = UI.TextSettingsForButtons with { Size = 24f, WordWrap = true };
                        UI.TextAsync(sourceNameRect, sourceEntry.Name, sourceTs);

                        // Target collectible
                        var targetRect = previewRect.CutRightUnscaled(originalPreviewRectWidth * 0.4f).Inset(5);
                        var targetBg = targetEntry.Rarity switch {
                            BrainrotValueRarity.Common => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                            BrainrotValueRarity.Rare => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_green.png"),
                            BrainrotValueRarity.Epic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_blue.png"),
                            BrainrotValueRarity.Legendary => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_yellow.png"),
                            BrainrotValueRarity.Mythic => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_red.png"),
                            _ => Assets.GetAsset<Texture>($"$AO/ui/kit/Modal Augments/banner_panel_1/int_sq_panel_1_grey.png"),
                        };
                        UI.Image(targetRect, targetBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});
                        
                        var targetIconRect = targetRect.TopRect().CutTop(180).Inset(20);
                        UI.Image(targetIconRect.FitAspect(targetEntry.Sprite.Aspect), targetEntry.Sprite);
                        
                        var targetNameRect = targetRect.BottomRect().CutBottom(70).Inset(10);
                        var targetTs = UI.TextSettingsForButtons with { Size = 24f, WordWrap = true };
                        UI.TextAsync(targetNameRect, targetEntry.Name, targetTs);

                        // // Arrow
                        var arrowTexture = Assets.GetAsset<Texture>("next_arrow.png");
                        UI.Image(previewRect.FitAspect(arrowTexture.Aspect), arrowTexture);

                        // Progress info
                        var progressRect = bgRect.CutTop(100).Inset(10);
                        var progressBg = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/ranking_backing/rectangle_backings/rectangle_backing_black.png");
                        UI.Image(progressRect, progressBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});

                        var progressTs = UI.TextSettingsForButtons;
                        progressTs.HorizontalAlignment = UI.HorizontalAlignment.Center;
                        progressTs.VerticalAlignment = UI.VerticalAlignment.Center;
                        progressTs.Size = 36f;
                        
                        bool canFuse = selected.count >= selected.required;
                        progressTs.Color = canFuse ? new Vector4(0.5f, 1.0f, 0.5f, 1) : new Vector4(1.0f, 0.6f, 0.6f, 1);
                        
                        UI.TextAsync(progressRect, $"Copies Collected: {selected.count}/{selected.required}", progressTs);

                        bgRect.CutTop(10);

                        // Info text
                        var infoRect = bgRect.CutTop(80).Inset(10);
                        var infoTs = UI.TextSettingsForButtons with { Size = 22f, WordWrap = true };
                        infoTs.HorizontalAlignment = UI.HorizontalAlignment.Center;
                        infoTs.VerticalAlignment = UI.VerticalAlignment.Top;
                        UI.TextAsync(infoRect, $"Fusing will consume {selected.required} copies and create 1 evolved version", infoTs);

                        // Fuse button
                        var buttonRect = bgRect.BottomRect().Inset(0, 50, 0, 50).GrowTop(80).Offset(0, 20);
                        var fuseIcon = targetEntry.Sprite;
                        var buttonColor = canFuse ? ButtonColor.Blue : ButtonColor.Grey;
                        var buttonResult = UI.DrawWindowButton(buttonRect, "FUSE", fuseIcon, buttonColor, ButtonStyle.BasicShiny);
                        
                        if (buttonResult.Clicked && canFuse)
                        {
                            CallServer_FuseCollectible(selected.id);
                            SelectedIndex = -1; // Reset selection after fusion
                        }
                    }
                }
            }
        });
    }

    [ServerRpc]
    public static void FuseCollectible(string collectibleId)
    {
        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null || !player.Entity.Alive())
            return;
            
        if (!BrainrotCatalog.TryGetEvolution(collectibleId, out string evolutionId))
            return;
        
        var entry = BrainrotCatalog.Get(collectibleId);
        int requiredCount = entry.BasePartsToEvolve;
        
        var itemsToConsume = new List<Item_Instance>();
        foreach (var item in player.DefaultInventory.Items)
        {
            if (item != null && item.Definition.Id == collectibleId)
            {
                itemsToConsume.Add(item);
                if (itemsToConsume.Count >= requiredCount)
                    break;
            }
        }
        
        if (itemsToConsume.Count < requiredCount)
        {
            player.CallClient_ShowMessage($"Not enough copies! Need {requiredCount}", new RPCOptions() { Target = player });
            return;
        }

        foreach (var item in itemsToConsume)
        {
            Inventory.DestroyItem(item);
        }
        
        var evolvedItemDef = BrainrotCatalog.GetItemDefinition(evolutionId);
        var evolvedItem = Inventory.CreateItem(evolvedItemDef, 1);
        Inventory.MoveItemToInventory(evolvedItem, player.DefaultInventory);
        
        var evolutionEntry = BrainrotCatalog.Get(evolutionId);
        player.CallClient_ShowMessage($"✨ Fused into {evolutionEntry.Name}! ✨", new RPCOptions() { Target = player });
        
        if (evolutionEntry.Sound != null)
        {
            player.CallClient_PlayFusionSound(evolutionEntry.Sound.Name);
        }
    }

    public override void Update()
    {
        if (IsFusionWindowOpen)
        {
            IsFusionWindowOpen = UI.DrawWindow(FusionWindow);
        }
    }

    public void OpenFusionWindow()
    {
        IsFusionWindowOpen = true;
    }
}
