using AO;

public partial class PlacedCollectible : Component
{
    [Serialized] public string CollectibleType;
    [Serialized] public MyPlayer OwnerPlayer;
    [Serialized] public Plot OwnerPlot;
    [Serialized] public float MoneyPerSecond = 1.0f;

    public Sprite_Renderer SpriteRenderer;
    
    [Serialized] public int Width = 1;
    [Serialized] public int Height = 1;
    [Serialized] public int GridX = 0;
    [Serialized] public int GridY = 0;


    public override void Awake()
    {
        SpriteRenderer = Entity.GetComponent<Sprite_Renderer>();
        var defn = BrainrotCatalog.Get(CollectibleType);
        SpriteRenderer.Sprite = defn.Sprite;

        // Get size from database
        var size = CollectibleDatabase.GetCollectibleSize(CollectibleType);
        Width = size.width;
        Height = size.height;

        // Position the collectible centered on its tiles
        if (OwnerPlot.Alive())
        {
            Entity.Position = OwnerPlot.GetCenteredWorldPositionForCollectible(GridX, GridY, Width, Height);
            
            // Mark all tiles as occupied
            for (int dx = 0; dx < Width; dx++)
            {
                for (int dy = 0; dy < Height; dy++)
                {
                    int checkX = GridX + dx;
                    int checkY = GridY + dy;
                    if (checkX >= 0 && checkX < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(0) &&
                        checkY >= 0 && checkY < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(1))
                    {
                        OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] = Entity;
                    }
                }
            }
        }
    }

    public override void Update()
    {
    }

    public override void OnDestroy()
    {
        // Clear occupied tiles when destroyed
        if (OwnerPlot.Alive())
        {
            for (int dx = 0; dx < Width; dx++)
            {
                for (int dy = 0; dy < Height; dy++)
                {
                    int checkX = GridX + dx;
                    int checkY = GridY + dy;
                    if (checkX >= 0 && checkX < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(0) &&
                        checkY >= 0 && checkY < OwnerPlot.CollectiblesPlacementOccupiedBy.GetLength(1))
                    {
                        if (OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] == Entity)
                        {
                            OwnerPlot.CollectiblesPlacementOccupiedBy[checkX, checkY] = default;
                        }
                    }
                }
            }
        }
    }
}

