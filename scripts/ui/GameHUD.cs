using AO;

public class GameHUD : Component
{
    public override void Update()
    {
        if (!Network.IsClient)
            return;

        var localPlayer = Network.LocalPlayer as MyPlayer;
        // DrawHUD(localPlayer.Money.Value, localPlayer.BaseHealth.Value);
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

