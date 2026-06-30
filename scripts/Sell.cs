using AO;

public class Sell : System<Sell>
{
    public Window SellWindow;
    public bool IsSellWindowOpen = false;

    public override void Start()
    {
        SellWindow = Window.Make(new WindowOptions(){
            Title = "Sell Items",
            WindowBackgroundStyle = WindowBackgroundStyle.Simple,
            WindowTitleFlare = WindowTitleFlare.Red,
            Content = new TabbedWindowContent() {
                Tabs = new List<TabContent>() {
                    new TabContent() {
                        Label = "Brainrots",
                        Content = new SplitWindowContent() {
                            Axis = SplitAxis.Vertical,
                            SplitRatio = 0.15f,
                            First = new CustomWindowContent() {
                                DrawContent = (rect) => {
                                    var sellRow = rect.Inset(5);
                                    var sellButtonRect = sellRow.CutRight(200);
                                    var result = UI.DrawWindowButton(sellButtonRect, "Sell All", null, ButtonColor.Green, ButtonStyle.ArrowShiny);
                                    if (result.Clicked)
                                    {
                                        var p = (MyPlayer)Network.LocalPlayer;
                                        p.CallServer_RequestSellAllBrainrots();
                                        var tabbedContent = SellWindow.TryGet<TabbedWindowContent>();
                                        tabbedContent.Tabs[0].Content.TryGet<ListWindowContent>().Items = new List<CollectionElement>();
                                    }
                                }
                            },
                            Second = new ListWindowContent() {
                            }
                        }
                    },
                    new TabContent() {
                        Label = "Turrets",
                        Content = new SplitWindowContent() {
                            Axis = SplitAxis.Vertical,
                            SplitRatio = 0.15f,
                            First = new CustomWindowContent() {
                                DrawContent = (rect) => {
                                    var sellRow = rect.Inset(5);
                                    var sellButtonRect = sellRow.CutRight(200);
                                    var result = UI.DrawWindowButton(sellButtonRect, "Sell All", null, ButtonColor.Green, ButtonStyle.ArrowShiny);
                                    if (result.Clicked)
                                    {
                                        var p = (MyPlayer)Network.LocalPlayer;
                                        p.CallServer_RequestSellAllTurrets();
                                        var tabbedContent = SellWindow.TryGet<TabbedWindowContent>();
                                        tabbedContent.Tabs[1].Content.TryGet<ListWindowContent>().Items = new List<CollectionElement>();
                                    }
                                }
                            },
                            Second = new ListWindowContent() {
                            }
                        }
                    }
                }
            }
        });
    }

    public override void Update()
    {
        if (IsSellWindowOpen)
        {
            IsSellWindowOpen = UI.DrawWindow(SellWindow);
        }
    }

    public List<string> ItemsRequestedToSell = new List<string>();

    public void OpenSellWindow()
    {
        if (Network.IsServer) return;
        
        var tabContent = SellWindow.TryGet<TabbedWindowContent>();

        // Brainrots tab
        var brainrotsContent = tabContent.Tabs[0].Content.TryGet<ListWindowContent>();
        brainrotsContent.Items = new List<CollectionElement>();

        var brainrotsToSell = new List<Item_Instance>();
        foreach (var item in Network.LocalPlayer.DefaultInventory.Items)
        {
            if (item == null) continue;
            if (BrainrotCatalog.Entries.ContainsKey(item.Definition.Id))
            {
                brainrotsToSell.Add(item);
            }
        }

        // Sort by rarity
        brainrotsToSell.Sort((a, b) => {
            var aEntry = BrainrotCatalog.Get(a.Definition.Id);
            var bEntry = BrainrotCatalog.Get(b.Definition.Id);
            return aEntry.Rarity.CompareTo(bEntry.Rarity);
        });

        foreach (var item in brainrotsToSell)
        {
            var entry = BrainrotCatalog.Get(item.Definition.Id);
            
            // Get modifier from item metadata
            BrainrotModifier modifier = BrainrotModifier.None;
            string modifierStr = item.GetMetadata("modifier");
            if (!string.IsNullOrEmpty(modifierStr) && int.TryParse(modifierStr, out int modifierInt))
            {
                modifier = (BrainrotModifier)modifierInt;
            }
            
            var color = entry.Rarity switch {
                BrainrotValueRarity.Common => ElementColor.Grey,
                BrainrotValueRarity.Rare => ElementColor.Green,
                BrainrotValueRarity.Epic => ElementColor.Blue,
                BrainrotValueRarity.Legendary => ElementColor.Yellow,
                BrainrotValueRarity.Mythic => ElementColor.Red,
                BrainrotValueRarity.Ethereal => ElementColor.Rainbow,
                BrainrotValueRarity.Primal => ElementColor.Rainbow,
                BrainrotValueRarity.Secret => ElementColor.Rainbow,
                _ => ElementColor.Grey,
            };
            
            // Calculate sell value (50% of base price)
            long sellValue = entry.BasePrice / 2;
            
            // Apply modifier bonus to sell value
            float modifierMult = PlacedCollectible.GetModifierMultiplier(modifier);
            sellValue = (long)(sellValue * modifierMult);
            
            string modifierText = modifier != BrainrotModifier.None ? $" ({PlacedCollectible.GetModifierName(modifier)})" : "";
            string gpsText = $"{StringUtils.FormatMoney(entry.BaseGoldGeneration)}/s";

            brainrotsContent.Items.Add(new CollectionElement() {
                Label = $"{entry.Name}{modifierText}",
                Icon = entry.Sprite,
                ListItemActionButtonText = "Sell",
                ListItemButtonColor = ButtonColor.Green,
                ListItemButtonStyle = ButtonStyle.ArrowShiny,
                Color = color,
                Description = $"{entry.Rarity}\n{gpsText}\nSells For: {StringUtils.FormatMoney(sellValue)}",
                Userdata = item,
                ListItemAction = OnListItemAction,
            });
        }

        // Turrets tab
        var turretsContent = tabContent.Tabs[1].Content.TryGet<ListWindowContent>();
        turretsContent.Items = new List<CollectionElement>();
        
        foreach (var item in Network.LocalPlayer.DefaultInventory.Items)
        {
            if (item == null) continue;
            
            if (TurretDatabase.IsTurret(item.Definition.Id))
            {
                int turretCost = TurretDatabase.GetCost(item.Definition.Id);
                long sellValue = turretCost / 2; // Sell for 50% of cost
                
                var stats = TurretDatabase.GetStats(item.Definition.Id);
                
                turretsContent.Items.Add(new CollectionElement() {
                    Label = item.Definition.Name,
                    Icon = Assets.GetAsset<Texture>("grass.png"), // Placeholder icon
                    ListItemActionButtonText = "Sell",
                    ListItemButtonColor = ButtonColor.Green,
                    ListItemButtonStyle = ButtonStyle.ArrowShiny,
                    Color = ElementColor.Grey,
                    Description = $"Damage: {stats.damage}\nRange: {stats.range}m\nSells For: {StringUtils.FormatMoney(sellValue)}",
                    Userdata = item,
                    ListItemAction = OnListItemAction,
                });
            }
        }

        IsSellWindowOpen = true;
    }

    public void OnListItemAction(CollectionElement element)
    {
        var item = (Item_Instance)element.Userdata;
        if (ItemsRequestedToSell.Contains(item.Id)) return;

        Log.Info($"Selling item {item.Id}");

        var p = (MyPlayer)Network.LocalPlayer;
        p.CallServer_RequestSellSingleItem(item.Id);
        ItemsRequestedToSell.Add(item.Id);
        element.ListItemButtonColor = ButtonColor.Grey;
    }
}
