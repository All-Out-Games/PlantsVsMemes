using AO;

public class Projectile : Component
{
    public Entity TargetEnemy;
    public int Damage = 10;
    public float MoveSpeed = 8.0f;
    public string ProjectileType = "pea";
    public bool IsServerProjectile = false;
    public bool IsCobcannon = false;

    Spine_Animator spineAnimator;

    public override void Awake()
    {
        spineAnimator = Entity.GetComponent<Spine_Animator>();
        spineAnimator.DepthOffset -= 0.03f;
    }

    public override void Update()
    {
        // Movement and visual updates happen on both client and server
        if (!TargetEnemy.Alive())
        {
            Entity.Destroy();
            return;
        }

        var enemy = TargetEnemy.GetComponent<Enemy>();
        if (enemy == null || enemy.Health <= 0)
        {
            Entity.Destroy();
            return;
        }

        // Move towards target
        var enemySize = TargetEnemy.GetComponent<Sprite_Renderer>().GetWorldSize();
        var direction = TargetEnemy.Position + new Vector2(0, enemySize.Y / 2) - Entity.Position;
        var distance = direction.Length;

        if (distance < 0.5f)
        {
            HitTarget();
            return;
        }

        var normalizedDirection = direction / distance;
        Entity.Position += normalizedDirection * MoveSpeed * Time.DeltaTime;

        // Update rotation
        // if (Network.IsClient && spineAnimator != null)
        // {
        //     var rotation = MathF.Atan2(direction.Y, direction.X) * (180.0f / MathF.PI);
        //     spineAnimator.Entity.Rotation = rotation;
        // }
    }

    void HitTarget()
    {
        // Only server projectiles deal damage
        if (IsServerProjectile && TargetEnemy.Alive())
        {
            var enemy = TargetEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Damage);
            }
        }

        Entity.Destroy();
    }
}

