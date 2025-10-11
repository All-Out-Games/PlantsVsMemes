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
    }

    public override void Update()
    {
    }
}

