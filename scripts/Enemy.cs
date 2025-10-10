using AO;

public partial class Enemy : Component
{
    [Serialized] public int Health;
    [Serialized] public int LaneIndex;
    [Serialized] public string EnemyType;
    [Serialized] public BrainrotModifier Modifier;

    [Serialized] public Plot OwnerPlot;
    [Serialized] public Vector2 StartPosition;
    [Serialized] public Vector2 TargetPosition;
    [Serialized] public float MoveSpeed = 1.0f;

    public Sprite_Renderer SpriteRenderer;

    public override void Awake()
    {
        SpriteRenderer = Entity.GetComponent<Sprite_Renderer>();
        
        var entry = BrainrotCatalog.Get(EnemyType);
        SpriteRenderer.Sprite = entry.Sprite;
        Log.Info("spawned enemy " + EnemyType);
    }

    public override void Update()
    {
        ServerUpdate();
    }

    void ServerUpdate()
    {
        if (!OwnerPlot.Alive() || !OwnerPlot.Owner.Alive())
        {
            Network.Despawn(Entity);
            Entity.Destroy();
            return;
        }

        if (Health <= 0)
        {
            HandleDeath();
            return;
        }

        var direction = TargetPosition - Entity.Position;
        var distance = direction.Length;

        if (distance < 0.5f)
        {
            ReachExit();
            return;
        }

        var normalizedDirection = direction / distance;
        Entity.Position += normalizedDirection * MoveSpeed * Time.DeltaTime;
    }

    void HandleDeath()
    {
        if (!Network.IsServer)
            return;

        Network.Despawn(Entity);
        Entity.Destroy();
    }

    void ReachExit()
    {
        if (!Network.IsServer)
            return;

        Network.Despawn(Entity);
        Entity.Destroy();
    }

    public void TakeDamage(int damage)
    {
        if (!Network.IsServer)
            return;

        Health -= damage;
    }
}

