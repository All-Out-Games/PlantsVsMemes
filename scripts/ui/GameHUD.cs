using AO;

public class GameHUD : Component
{
    public override void Update()
    {
        if (!Network.IsClient)
            return;

        var localPlayer = Network.LocalPlayer as MyPlayer;
        // DrawHUD(localPlayer.Money.Value, localPlayer.BaseHealth.Value);
        
        DrawCoinsDisplay(localPlayer);
        DrawTurretShopButton();
        DrawKeybindHelp(localPlayer);
    }
    
    void DrawCoinsDisplay(MyPlayer player)
    {
        if (player == null || !player.Entity.Alive())
            return;
            
        var coinRect = UI.SafeRect.TopRect().LeftRect().CutTop(80).CutLeft(300).Inset(10);
        
        // Background
        var bgColor = new Vector4(0.1f, 0.1f, 0.1f, 0.85f);
        UI.Image(coinRect, null, bgColor);
        
        // Border
        var borderColor = new Vector4(1f, 0.84f, 0f, 1f);
        UIUtils.DrawOutlines(coinRect, borderColor, 3f);
        
        // Coin icon (using emoji)
        var iconRect = coinRect.CutLeft(60).Inset(5);
        UI.Image(iconRect, Assets.GetAsset<Texture>("gold.png"), new Vector4(1f, 1f, 1f, 1f));
        
        // Coin amount
        var textTs = new UI.TextSettings()
        {
            Font = UI.Fonts.BarlowBold,
            Size = 36,
            Color = new Vector4(1f, 0.95f, 0.6f, 1f),
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            VerticalAlignment = UI.VerticalAlignment.Center,
            Outline = true,
            OutlineThickness = 3,
            OutlineColor = new Vector4(0f, 0f, 0f, 1f),
        };
        
        var balance = Economy.GetBalance(player, Config.Currency_Gold);
        UI.TextAsync(coinRect, StringUtils.FormatMoney(balance), textTs);
    }
    
    void DrawTurretShopButton()
    {
        var topButtonsRect = UI.SafeRect.TopRect().CutTop(100).Offset(0, -10).Inset(0, 600, 0, 600);
        var grid = UI.GridLayout.Make(topButtonsRect, 5, 1, UI.GridLayout.SizeSource.ElementCount, 15);


        var shopButtonRect = grid.Next();
        var buttonColor = new Vector4(0.2f, 0.6f, 0.2f, 0.9f);
        if (UI.DrawWindowButton(shopButtonRect, "Shop", null, ButtonColor.Green, ButtonStyle.Basic).Clicked)
        {
            TurretShop.Instance.IsOpen = true;
        }

        var fusionButtonRect = grid.Next();
        if (UI.DrawWindowButton(fusionButtonRect, "Fusion", null, ButtonColor.Blue, ButtonStyle.Basic).Clicked)
        {
            FusionSystem.Instance.OpenFusionWindow();
        }

        var craftingButtonRect = grid.Next();
        if (UI.DrawWindowButton(craftingButtonRect, "Crafting", null, ButtonColor.Green, ButtonStyle.Basic).Clicked)
        {
            CraftingSystem.Instance.OpenCraftingWindow();
        }
        
        var rebirthButtonRect = grid.Next();
        if (UI.DrawWindowButton(rebirthButtonRect, "Rebirth", null, ButtonColor.Red, ButtonStyle.Basic).Clicked)
        {
            Rebirth.Instance.OpenRebirthWindow();
        }
        
        var sellButtonRect = grid.Next();
        if (UI.DrawWindowButton(sellButtonRect, "Sell", null, ButtonColor.Red, ButtonStyle.Basic).Clicked)
        {
            Sell.Instance.OpenSellWindow();
        }
        
        // // Show time until next rotation
        // if (TurretShop.Instance != null)
        // {
        //     float timeLeft = TurretShop.Instance.NextRotationTime.Value - Time.TimeSinceStartup;
        //     if (timeLeft > 0)
        //     {
        //         var timerRect = buttonRect.BottomRect().CutBottom(25).Offset(0, 5);
        //         var timerTs = new UI.TextSettings()
        //         {
        //             Font = UI.Fonts.BarlowBold,
        //             Size = 16,
        //             Color = new Vector4(1f, 1f, 0.5f, 1f),
        //             HorizontalAlignment = UI.HorizontalAlignment.Center,
        //             VerticalAlignment = UI.VerticalAlignment.Center,
        //             Outline = true,
        //             OutlineThickness = 2,
        //         };
                
        //         int minutes = (int)(timeLeft / 60);
        //         int seconds = (int)(timeLeft % 60);
        //         UI.TextAsync(timerRect, $"Rotates: {minutes}:{seconds:D2}", timerTs);
        //     }
        // }
    }
    
    void DrawKeybindHelp(MyPlayer player)
    {
        if (player == null || !player.Entity.Alive())
            return;
        
        // Show rebirth level if player has rebirths
        if (player.RebirthLevel.Value > 0)
        {
            var helpRect = UI.SafeRect.TopRect().RightRect().CutTop(80).CutRight(200).Inset(10);
            
            // Background
            var bgColor = new Vector4(0.05f, 0.05f, 0.1f, 0.7f);
            UI.Image(helpRect, null, bgColor);
            
            var textTs = new UI.TextSettings()
            {
                Font = UI.Fonts.BarlowBold,
                Size = 16,
                Color = new Vector4(0.9f, 0.9f, 1f, 1f),
                HorizontalAlignment = UI.HorizontalAlignment.Center,
                VerticalAlignment = UI.VerticalAlignment.Center,
            };
            
            var titleRect = helpRect.TopRect().CutTop(25).Inset(5);
            UI.TextAsync(titleRect, "REBIRTH", textTs with { Color = new Vector4(1f, 1f, 0.5f, 1f) });
            
            var rebirthRect = helpRect.CutTop(40).Inset(5);
            float multiplier = player.GetRebirthEarningsMultiplier();
            UI.TextAsync(rebirthRect, $"Level {player.RebirthLevel.Value}\nx{multiplier:F2} Earnings", textTs with { Size = 14, Color = new Vector4(1f, 0.8f, 0.2f, 1f) });
        }
    }

    public static void DrawHUD(int money, int baseHealth)
    {
        var topRect = UI.SafeRect.TopRect().CutTop(80).Inset(20);

        var moneyRect = topRect.LeftRect().CutLeft(300);
        UI.Image(moneyRect, null, new Vector4(0, 0, 0, 0.7f));
        
        var moneyTs = new UI.TextSettings()
        {
            Font = UI.Fonts.BarlowBold,
            Size = 40,
            Color = new Vector4(1f, 0.84f, 0f, 1f),
            DropShadowColor = new Vector4(0f, 0f, 0.02f, 0.5f),
            DropShadowOffset = new Vector2(0f, -3f),
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            VerticalAlignment = UI.VerticalAlignment.Center,
            Outline = true,
            OutlineThickness = 3,
        };
        
        UI.TextAsync(moneyRect, $"Money: ${money}", moneyTs);

        var healthRect = topRect.CenterRect().CutLeft(400);
        UI.Image(healthRect, null, new Vector4(0, 0, 0, 0.7f));
        
        var healthBarRect = healthRect.Inset(10);
        var healthFillWidth = healthBarRect.Width * (baseHealth / 100f);
        var healthFillRect = healthBarRect.CutLeft(healthFillWidth);
        
        var healthColor = baseHealth > 50 ? new Vector4(0, 1, 0, 0.8f) : 
                         baseHealth > 25 ? new Vector4(1, 1, 0, 0.8f) : 
                         new Vector4(1, 0, 0, 0.8f);
        
        UI.Image(healthFillRect, null, healthColor);
        
        var healthTs = new UI.TextSettings()
        {
            Font = UI.Fonts.BarlowBold,
            Size = 30,
            Color = Vector4.White,
            DropShadowColor = new Vector4(0f, 0f, 0.02f, 0.5f),
            DropShadowOffset = new Vector2(0f, -3f),
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            VerticalAlignment = UI.VerticalAlignment.Center,
            Outline = true,
            OutlineThickness = 3,
        };
        
        UI.TextAsync(healthRect, $"Base Health: {baseHealth}/100", healthTs);
    }
}

