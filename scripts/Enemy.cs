using AO;

public partial class Enemy : Component
{
    [Serialized, NetSync] public int Health;
    [Serialized] public int MaxHealth;
    [Serialized] public int LaneIndex;
    [Serialized] public string EnemyType;
    [Serialized] public BrainrotModifier Modifier;

    [Serialized] public Plot OwnerPlot;
    [Serialized] public Vector2 StartPosition;
    [Serialized] public Vector2 TargetPosition;
    [Serialized] public float MoveSpeed = 1.0f;

    public Sprite_Renderer SpriteRenderer;
    
    // Healthbar settings
    float healthbarWidth = 1.25f;
    float healthbarHeight = 0.2f;
    float healthbarOffsetY = 0.5f;

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
    
    public override void LateUpdate()
    {
        DrawHealthbar();
    }
    
    void DrawHealthbar()
    {
        if (!Network.IsClient)
            return;
        
        float healthPercent = Health / (float)MaxHealth;
        
        // Calculate healthbar color based on health percentage
        Vector4 fillColor;
        if (healthPercent > 0.6f)
        {
            fillColor = new Vector4(0.2f, 0.9f, 0.2f, 1.0f); // Green
        }
        else if (healthPercent > 0.3f)
        {
            fillColor = new Vector4(0.95f, 0.85f, 0.1f, 1.0f); // Yellow
        }
        else
        {
            fillColor = new Vector4(0.95f, 0.25f, 0.15f, 1.0f); // Red
        }
        
        // Push world space context
        using var _1 = UI.PUSH_CONTEXT(UI.Context.World);
        using var _2 = IM.PUSH_Z(Entity.Position.Y - 0.001f);
        
        var pos = Entity.Position + new Vector2(0, healthbarOffsetY);
        var halfSize = new Vector2(healthbarWidth * 0.5f, healthbarHeight * 0.5f);
        
        // Draw background (dark red)
        var bgRect = new Rect(pos - halfSize, pos + halfSize);
        UI.Image(bgRect, UI.WhiteSprite, new Vector4(0.3f, 0.05f, 0.05f, 0.9f));
        
        // Draw border (black outline)
        var borderThickness = 0.03f;
        var borderRect = bgRect.Grow(borderThickness);
        UI.Image(borderRect, UI.WhiteSprite, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
        UI.Image(bgRect, UI.WhiteSprite, new Vector4(0.3f, 0.05f, 0.05f, 0.9f));

        // Draw fill bar (from left to right based on health)
        if (healthPercent > 0)
        {
            // var fillWidth = healthbarWidth * healthPercent;
            // var fillHalfSize = new Vector2(fillWidth * 0.5f, healthbarHeight * 0.5f);
            // var fillCenter = pos - new Vector2(healthbarWidth * 0.5f - fillWidth * 0.5f, 0);
            // var fillRect = new Rect(fillCenter - fillHalfSize, fillCenter + fillHalfSize);
            var fillRect = bgRect.SubRect(0, 0, healthPercent, 1);
            UI.Image(fillRect, UI.WhiteSprite, fillColor);
        }

        var ts = UIUtils.CenteredText(true);
        UI.TextAsync(bgRect, $"{FormatHealth(Health)}/{FormatHealth(MaxHealth)}", ts);
    }

    public string FormatHealth(int health)
    {
        if (health > 1_000)
        {
            return $"{health / 1_000}k";
        }
        else if (health > 1_000_000)
        {
            return $"{health / 1_000_000}m";
        }
        else if (health > 1_000_000_000)
        {
            return $"{health / 1_000_000_000}b";
        }
        else
        {
            return health.ToString();
        }
    }

    void ServerUpdate()
    {
        // if (!OwnerPlot.Alive() || !OwnerPlot.Owner.Alive())
        // {
        //     Network.Despawn(Entity);
        //     Entity.Destroy();
        //     return;
        // }

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

        if (OwnerPlot.Alive())
        {
            if (OwnerPlot.Owner.Alive())
            {
                bool hasRoom = false;
                foreach (var item in OwnerPlot.Owner.DefaultInventory.Items)
                {
                    if (item == null)
                    {
                        hasRoom = true;
                        break;
                    }
                }

                if (hasRoom)
                {
                    var itemDefinition = BrainrotCatalog.GetItemDefinition(EnemyType);
                    var itemInstance = Inventory.CreateItem(itemDefinition, 1);
                    
                    // Store modifier in item metadata
                    itemInstance.SetMetadata("modifier", ((int)Modifier).ToString());
                    
                    Inventory.MoveItemToInventory(itemInstance, OwnerPlot.Owner.DefaultInventory);
                }
            }
        }

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

