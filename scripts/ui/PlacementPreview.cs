using AO;

public static class PlacementPreview
{
    public static void DrawPreview(Plot plot, int x, int y, int width, int height)
    {
        if (!Network.IsClient)
            return;

        if (!plot.Entity.Alive())
            return;

        bool canPlace = plot.CanPlaceAt(x, y, width, height);
        var color = canPlace ? new Vector4(0, 1, 0, 0.3f) : new Vector4(1, 0, 0, 0.3f);

        using var _1 = UI.PUSH_CONTEXT(UI.Context.World);
        using var _2 = IM.PUSH_Z(-0.1f);

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int tileX = x + dx;
                int tileY = y + dy;

                if (tileX >= 0 && tileX < 9 && tileY >= 0 && tileY < 7)
                {
                    var tilePos = plot.Tiles[tileX, tileY].Entity.Position;
                    var tileSize = new Vector2(1.8f, 1.8f);
                    var rect = new Rect(tilePos - tileSize / 2, tilePos + tileSize / 2);

                    var circle = Assets.GetAsset<Texture>("$AO/circle.png");
                    UI.Image(rect, circle, color);
                }
            }
        }
    }
}

