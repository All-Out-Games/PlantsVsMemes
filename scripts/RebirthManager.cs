using AO;

public partial class Rebirth : System<Rebirth>
{
    public List<RebirthConfig> RebirthConfigs = new List<RebirthConfig>();
    
    // Dynamically populate rebirth configs from BrainrotCatalog
    void InitializeRebirthConfigs()
    {
        RebirthConfigs.Clear();
        
        // Rebirth 1: Commons
        var commonsForR1 = GetRandomBrainrotsByRarity(BrainrotValueRarity.Common, 2);
        RebirthConfigs.Add(new RebirthConfig() {
            ItemRequirements = commonsForR1,
            RequiredQuantityPerItem = 10,
            CurrencyRequirement = Config.Currency_Gold,
            CurrencyRequirementAmount = 50000,
            Rewards = new List<RebirthReward>(){
                new RebirthReward() { Text = "x1.25 Earnings" },
            },
        });
        
        // Rebirth 2: Rares
        var raresForR2 = GetRandomBrainrotsByRarity(BrainrotValueRarity.Rare, 2);
        RebirthConfigs.Add(new RebirthConfig() {
            ItemRequirements = raresForR2,
            RequiredQuantityPerItem = 5,
            CurrencyRequirement = Config.Currency_Gold,
            CurrencyRequirementAmount = 250000,
            Rewards = new List<RebirthReward>(){
                new RebirthReward() { Text = "x1.56 Earnings" },
            },
        });
        
        // Rebirth 3: Epics
        var epicsForR3 = GetRandomBrainrotsByRarity(BrainrotValueRarity.Epic, 2);
        RebirthConfigs.Add(new RebirthConfig() {
            ItemRequirements = epicsForR3,
            RequiredQuantityPerItem = 3,
            CurrencyRequirement = Config.Currency_Gold,
            CurrencyRequirementAmount = 1000000,
            Rewards = new List<RebirthReward>(){
                new RebirthReward() { Text = "x1.95 Earnings" },
            },
        });
        
        // Rebirth 4: Legendaries
        var legendariesForR4 = GetRandomBrainrotsByRarity(BrainrotValueRarity.Legendary, 2);
        RebirthConfigs.Add(new RebirthConfig() {
            ItemRequirements = legendariesForR4,
            RequiredQuantityPerItem = 2,
            CurrencyRequirement = Config.Currency_Gold,
            CurrencyRequirementAmount = 5000000,
            Rewards = new List<RebirthReward>(){
                new RebirthReward() { Text = "x2.44 Earnings" },
            },
        });
        
        // Rebirth 5: Mythics
        var mythicsForR5 = GetRandomBrainrotsByRarity(BrainrotValueRarity.Mythic, 2);
        RebirthConfigs.Add(new RebirthConfig() {
            ItemRequirements = mythicsForR5,
            RequiredQuantityPerItem = 1,
            CurrencyRequirement = Config.Currency_Gold,
            CurrencyRequirementAmount = 25000000,
            Rewards = new List<RebirthReward>(){
                new RebirthReward() { Text = "x3.05 Earnings" },
            },
        });
    }
    
    // Get random brainrots of a specific rarity
    List<string> GetRandomBrainrotsByRarity(BrainrotValueRarity rarity, int count)
    {
        var candidates = new List<string>();
        
        foreach (var entry in BrainrotCatalog.Entries)
        {
            // Only include base forms (not evolutions) that match the rarity
            if (entry.Value.Rarity == rarity && !entry.Value.IsEvolution)
            {
                candidates.Add(entry.Key);
            }
        }
        
        // Shuffle and take the first 'count' items
        var rng = new Random();
        candidates = candidates.OrderBy(x => rng.Next()).ToList();
        
        return candidates.Take(Math.Min(count, candidates.Count)).ToList();
    }

    public Window RebirthWindow;
    public bool IsRebirthWindowOpen = false;

    // Assets expected to exist for the rebirth animation
    // Note: these will be filled with actual asset paths later
    Texture RebirthStarburstTex = Assets.GetAsset<Texture>("starburst.png"); // starburst background sprite
    Texture RebirthBannerTex = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/modal_banner_small/modal_banner_gold.png"); // banner background sprite across the top
    Texture RebirthPanelTex = Assets.GetAsset<Texture>(""); // optional panel/frame behind center icon

    // Animation state (client-only usage)
    bool _showRebirthAnim = false;
    float _rebirthAnimStartTime = 0f;
    float _rebirthAnimDuration = 5f;
    string _grantedRescueeId = null;
    List<RebirthReward> _lastRewardsForAnim = new List<RebirthReward>();

    public override void Awake()
    {
        // Initialize rebirth configs from brainrot catalog
        InitializeRebirthConfigs();
        
        RebirthWindow = Window.Make(new WindowOptions(){
            Title = "Rebirth",
            WindowBackgroundColor = WindowBackgroundColor.Black,
            WindowBackgroundStyle = WindowBackgroundStyle.Simple,
            WindowTitleFlare = WindowTitleFlare.Gold,
            Content = new CustomWindowContent() {
                DrawContent = (rect) => {
                    if (Network.IsServer) return;

                    var player = (MyPlayer)Network.LocalPlayer;
                    if (player == null) return;

                    var bgRect = rect.Inset(10);
                    var bgTexture = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/text_backer_black.png");
                    UI.Image(bgRect, bgTexture, new UI.NineSlice() { slice = new Vector4(34, 34, 34, 34), sliceScale = 1.0f});
                    bgRect = bgRect.Inset(12);

                    var tsCenter = UI.TextSettingsForButtons;
                    tsCenter.HorizontalAlignment = UI.HorizontalAlignment.Center;
                    tsCenter.VerticalAlignment = UI.VerticalAlignment.Center;
                    tsCenter.Size = 52f;

                    var tsLeft = UI.TextSettingsForButtons;
                    tsLeft.HorizontalAlignment = UI.HorizontalAlignment.Left;
                    tsLeft.VerticalAlignment = UI.VerticalAlignment.Center;
                    tsLeft.Size = 36f;

                    int nextIndex = player.RebirthLevel.Value;
                    if (RebirthConfigs == null || RebirthConfigs.Count == 0 || nextIndex >= RebirthConfigs.Count)
                    {
                        UI.TextAsync(bgRect, "Max rebirths reached!", tsCenter);
                        return;
                    }

                    var cfg = RebirthConfigs[nextIndex];

                    // Requirements title
                    {
                        var titleRect = bgRect.CutTop(40);
                        UI.TextAsync(titleRect.Inset(6), "Requirements:", tsLeft);
                        bgRect.CutTop(4);
                    }

                    int totalReq = 0;
                    int completedReq = 0;

                    // Currency requirement (optional)
                    if (string.IsNullOrEmpty(cfg.CurrencyRequirement) == false && cfg.CurrencyRequirementAmount > 0)
                    {
                        totalReq++;
                        var currencyRow = bgRect.CutTop(84).Inset(6);
                        var rowBg = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/ranking_backing/rectangle_backings/rectangle_backing_black.png");
                        UI.Image(currencyRow, rowBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});

                        var iconRect = currencyRow.CutLeft(96).Inset(10);
                        var coinTex = Assets.GetAsset<Texture>(Config.Currency_GoldIcon);
                        UI.Image(iconRect.FitAspect(coinTex.Aspect), coinTex);

                        long bal = Economy.GetBalance(player, cfg.CurrencyRequirement);
                        bool ok = bal >= cfg.CurrencyRequirementAmount;
                        if (ok) completedReq++;

                        var lineTs = UI.TextSettingsForButtons;
                        lineTs.HorizontalAlignment = UI.HorizontalAlignment.Left;
                        lineTs.VerticalAlignment = UI.VerticalAlignment.Center;
                        lineTs.Color = ok ? new Vector4(0.55f, 1.0f, 0.55f, 1) : new Vector4(1.0f, 0.6f, 0.6f, 1);
                        lineTs.Size = 34f;
                        UI.TextAsync(currencyRow.Inset(10, 10, 10, 0), $"{cfg.CurrencyRequirement}  {StringUtils.FormatMoney(bal)}/{StringUtils.FormatMoney(cfg.CurrencyRequirementAmount)}", lineTs);

                        bgRect.CutTop(6);
                    }

                    // Item requirements
                    if (cfg.ItemRequirements != null && cfg.ItemRequirements.Count > 0)
                    {
                        var listRect = bgRect.CutTop(200).Inset(2);
                        var grid = UI.GridLayout.Make(listRect, cfg.ItemRequirements.Count, 1, UI.GridLayout.SizeSource.ElementCount, 6);

                        for (int i = 0; i < cfg.ItemRequirements.Count; i++)
                        {
                            var id = cfg.ItemRequirements[i];
                            var brainrotEntry = BrainrotCatalog.Get(id);

                            var rowRect = grid.Next();
                            var rowBg = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/ranking_backing/rectangle_backings/rectangle_backing_black.png");
                            UI.Image(rowRect, rowBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});

                            var iconRect = rowRect.CutLeft(90).Inset(8);
                            UI.Image(iconRect.FitAspect(brainrotEntry.Sprite.Aspect), brainrotEntry.Sprite);

                            int haveCount = player.DefaultInventory.Items.Count(itm => itm != null && itm.Definition.Id == id);
                            int needCount = cfg.RequiredQuantityPerItem;
                            bool have = haveCount >= needCount;
                            totalReq++;
                            if (have) completedReq++;

                            var lineTs = UI.TextSettingsForButtons;
                            lineTs.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            lineTs.VerticalAlignment = UI.VerticalAlignment.Center;
                            lineTs.Color = have ? new Vector4(0.55f, 1.0f, 0.55f, 1) : new Vector4(1.0f, 0.6f, 0.6f, 1);
                            lineTs.Size = 32f;
                            UI.TextAsync(rowRect.Inset(8, 8, 8, 0), $"{brainrotEntry.Name}  {haveCount}/{needCount}", lineTs);
                        }

                        bgRect.CutTop(6);
                    }

                    // Unlocks title
                    {
                        var titleRect = bgRect.CutTop(40);
                        UI.TextAsync(titleRect.Inset(6), "You Unlock:", tsLeft);
                        bgRect.CutTop(4);
                    }

                    // Rewards grid
                    if (cfg.Rewards != null && cfg.Rewards.Count > 0)
                    {
                        var rewardsRect = bgRect.CutTop(200).Inset(2);
                        var grid = UI.GridLayout.Make(rewardsRect, cfg.Rewards.Count, 1, UI.GridLayout.SizeSource.ElementCount, 6);
                        for (int i = 0; i < cfg.Rewards.Count; i++)
                        {
                            var reward = cfg.Rewards[i];
                            var cell = grid.Next();
                            var cellBg = Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/ranking_backing/rectangle_backings/rectangle_backing_black.png");
                            UI.Image(cell, cellBg, new UI.NineSlice(){ slice = new Vector4(34,34,34,34), sliceScale = 1.0f});

                            if (string.IsNullOrEmpty(reward.Icon) == false)
                            {
                                var icon = Assets.GetAsset<Texture>(reward.Icon);
                                UI.Image(cell.Inset(25).FitAspect(icon.Aspect), icon);
                            }

                            var rts = UI.TextSettingsForButtons;
                            rts.HorizontalAlignment = UI.HorizontalAlignment.Center;
                            rts.VerticalAlignment = UI.VerticalAlignment.Center;
                            rts.Size = 26f;
                            UI.TextAsync(cell.CutBottom(50), reward.Text, rts);
                        }
                        bgRect.CutTop(6);
                    }

                    // Progress + Button
                    {
                        float pct = totalReq <= 0 ? 0f : (float)completedReq / (float)totalReq;
                        var progressRect = bgRect.CutTop(40);
                        var pts = UI.TextSettingsForButtons;
                        pts.HorizontalAlignment = UI.HorizontalAlignment.Center;
                        pts.VerticalAlignment = UI.VerticalAlignment.Center;
                        pts.Size = 30f;
                        UI.TextAsync(progressRect, $"Progress: {(int)(pct * 100)}% ({completedReq}/{totalReq})", pts);

                        // Button
                        var btnRect = bgRect.Inset(0, 50, 0, 50);
                        var color = (pct >= 1f) ? ButtonColor.Green : ButtonColor.Grey;
                        var rebirthIcon = Assets.GetAsset<Texture>("ui/Rebirth.png");
                        
                        var result = UI.DrawWindowButton(btnRect, "Rebirth", rebirthIcon, color, ButtonStyle.BasicShiny);
                        if (result.Clicked)
                        {
                            CallServer_RequestRebirth();
                            Instance.IsRebirthWindowOpen = false;
                        }
                    }
                },
            }
        });      
    }

    public static bool CanRebirth(MyPlayer player)
    {
        if (player.RebirthLevel.Value >= Instance.RebirthConfigs.Count) return false;

        var rebirthConfig = Instance.RebirthConfigs[player.RebirthLevel.Value];

        if (rebirthConfig.CurrencyRequirement != null && rebirthConfig.CurrencyRequirementAmount > 0)
        {
            if (Economy.GetBalance(player, rebirthConfig.CurrencyRequirement) < rebirthConfig.CurrencyRequirementAmount) return false;
        }

        // Check each required collectible type with its quantity requirement
        foreach (var collectibleId in rebirthConfig.ItemRequirements)
        {
            int haveCount = player.DefaultInventory.Items.Count(itm => itm != null && itm.Definition.Id == collectibleId);
            int needCount = rebirthConfig.RequiredQuantityPerItem;
            
            if (haveCount < needCount) return false;
        }

        return true;
    }

    [ServerRpc]
    public static void RequestRebirth()
    {
        var player = Network.GetRemoteCallContextPlayer() as MyPlayer;
        if (player == null) return;

        if (CanRebirth(player) == false) return;

        var rebirthConfig = Instance.RebirthConfigs[player.RebirthLevel.Value];
        int newLevel = player.RebirthLevel.Value + 1;

        // Consume required collectibles (correct quantity per type)
        foreach (var collectibleId in rebirthConfig.ItemRequirements)
        {
            int consumed = 0;
            int needToConsume = rebirthConfig.RequiredQuantityPerItem;
            
            foreach (var itemInstance in player.DefaultInventory.Items.ToList())
            {
                if (consumed >= needToConsume) break;
                
                if (itemInstance != null && itemInstance.Definition.Id == collectibleId)
                {
                    Inventory.DestroyItem(itemInstance);
                    consumed++;
                }
            }
        }

        if (string.IsNullOrEmpty(rebirthConfig.CurrencyRequirement) == false && rebirthConfig.CurrencyRequirementAmount > 0)
        {
            Economy.WithdrawCurrency(player, rebirthConfig.CurrencyRequirement, rebirthConfig.CurrencyRequirementAmount);
        }

        player.RebirthLevel.Set(newLevel);
        Save.SetInt(player, Config.SaveKey_RebirthLevel, newLevel);
        
        // Notify player of rebirth
        float newMultiplier = player.GetRebirthEarningsMultiplier();
        CallClient_NotifyRebirth(player, newLevel, "", new RPCOptions() { Target = player });
        player.CallClient_ShowMessage($"⟳ Reborn to Level {newLevel}! Earnings x{newMultiplier:F2}! ⟳", new RPCOptions() { Target = player });
    }

    [ClientRpc]
    public static void NotifyRebirth(MyPlayer player, int level, string grantedItem)
    {
        if (player.IsLocal)
        {
            // Trigger client-side rebirth animation
            Instance._showRebirthAnim = true;
            Instance._rebirthAnimStartTime = Time.TimeSinceStartup;
            Instance._grantedRescueeId = grantedItem;
            Instance.IsRebirthWindowOpen = false;

            // Capture rewards for UI
            int cfgIndex = Math.Max(0, Math.Min(level - 1, Instance.RebirthConfigs.Count - 1));
            Instance._lastRewardsForAnim = new List<RebirthReward>(Instance.RebirthConfigs[cfgIndex].Rewards);
        }
    }


    public override void Update()
    {
        // Draw rebirth screen-space animation on client
        if (Network.IsClient && _showRebirthAnim)
        {
            using var _ = UI.PUSH_LAYER(11);

            float raw = (Time.TimeSinceStartup - _rebirthAnimStartTime) / _rebirthAnimDuration;
            if (raw < 0f) raw = 0f; if (raw > 1f) raw = 1f;
            float t = raw;
            UI.Image(UI.ScreenRect, UI.WhiteSprite, Vector4.Black * 0.6f);

            var centerRect = UI.ScreenRect.CenterRect();
            var burstRect = centerRect.Grow(350);

            float spin = (Time.TimeSinceStartup - _rebirthAnimStartTime) * 60f;
            using (var rot = UI.PUSH_ROTATE_ABOUT_POINT(spin, centerRect.Center))
            {
                if (RebirthStarburstTex != null)
                {
                    UI.Image(burstRect, RebirthStarburstTex, new Vector4(1,1,1,0.8f));
                }
            }

            var headerRect = UI.SafeRect.TopCenterRect().Grow(0, 275, 125, 275).Offset(0, -150);
            UI.Image(headerRect, Assets.GetAsset<Texture>("$AO/ui/kit/Modal Augments/modal_banner_small/modal_banner_gold.png"));
            UI.TextAsync(headerRect.Inset(6).Offset(0, 5), "Rebirth Rewards", UI.TextSettingsForButtons);

            float itemTransitionInDuration = 0.33f;

            var rewardsRect = burstRect.Inset(200, 0, 200, 0).Grow(0, 100, 0, 100);
            var rewardsCount = _lastRewardsForAnim.Count;
            var rewardsWidth = rewardsRect.Width / rewardsCount;
            for (int i = 0; i < rewardsCount; i++)
            {
                var thisItemFadeInTime = _rebirthAnimStartTime + (itemTransitionInDuration * (i+1));
                var thisItemFadeInT = (Time.TimeSinceStartup - thisItemFadeInTime) / itemTransitionInDuration;
                if (thisItemFadeInT < 0f) thisItemFadeInT = 0f;
                if (thisItemFadeInT > 1f) thisItemFadeInT = 1f;
                var thisItemFadeInAlpha = Ease.OutBack(thisItemFadeInT);
                using var _1 = UI.PUSH_COLOR_MULTIPLIER(Vector4.White * thisItemFadeInAlpha);
                
                var rewardRect = rewardsRect.CutLeftUnscaled(rewardsWidth);
                var itemRect = rewardRect.Inset(3);
                itemRect = itemRect.Slide(0, -(1-thisItemFadeInT));

                var reward = _lastRewardsForAnim[i];
                Texture itemTex = null;
                string text = reward.Text;
                
                // Use gold icon or generic reward visual
                if (!string.IsNullOrEmpty(reward.Icon))
                {
                    itemTex = Assets.GetAsset<Texture>(reward.Icon);
                }
                
                if (itemTex == null)
                {
                    itemTex = Assets.GetAsset<Texture>("gold.png");
                }
                
                if (itemTex != null)
                {
                    UI.Image(itemRect.FitAspect(itemTex.Aspect), itemTex, Vector4.White);
                }
                else
                {
                    UI.Image(itemRect, UI.WhiteSprite, Vector4.White);
                }

                var ts = UI.TextSettingsForHeaders;
                ts.Size = 45f;
                ts.WordWrap = true;
                var textRect = itemRect.BottomRect().Grow(75, 0, 75, 0);
                UI.TextAsync(textRect, text, ts);
            }

            // Auto-hide at end
            if (t >= 1f)
            {
                _showRebirthAnim = false;
            }
        }

        if (IsRebirthWindowOpen)
        {
            if (UI.DrawWindow(RebirthWindow) == false)
            {
                IsRebirthWindowOpen = false;
            }
        }
    }

    public void OpenRebirthWindow()
    {
        IsRebirthWindowOpen = true;
    }
}

public class RebirthInteractable : Component
{
    public Interactable Interactable;

    public override void Awake()
    {
        Interactable = GetComponent<Interactable>();
        Interactable.OnInteract += OnInteract;
    }

    public void OnInteract(Player player)
    {
        if (player.IsLocal)
        {
            Rebirth.Instance.OpenRebirthWindow();
        }
    }
}

public class RebirthConfig
{
    public List<string> ItemRequirements;
    public int RequiredQuantityPerItem = 1; // How many of each item in ItemRequirements
    public string CurrencyRequirement;
    public long CurrencyRequirementAmount;
    public List<RebirthReward> Rewards;
}

public class RebirthReward
{
    public string Icon;
    public string Text;
}