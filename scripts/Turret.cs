using AO;

public partial class Turret : Component
{
    public float AttackRange = 6.0f;
    public float AttackCooldown = 1.5f;
    public int Damage = 10;
    [Serialized] public string TurretType = "peashooter";
    [Serialized] public int GridX = 0;
    [Serialized] public int GridY = 0;
    [Serialized] public Plot OwnerPlot;

    float lastAttackTime = 0;
    Spine_Animator spineAnimator;
    bool isShooting = false;
    float shootAnimTimer = 0;
    Entity currentTarget;

    public override void Awake()
    {
        var stats = TurretDatabase.GetStats(TurretType);
        AttackRange = stats.range;
        AttackCooldown = stats.cooldown;
        Damage = stats.damage;

        spineAnimator = Entity.GetComponent<Spine_Animator>();

        var spinePath = CollectibleDatabase.GetSpinePath(TurretType);
        var skeleton = Assets.GetAsset<SpineSkeletonAsset>(spinePath);
        spineAnimator.SpineInstance.SetSkeleton(skeleton);
        spineAnimator.SpineInstance.SetAnimation("Idle", true);
        spineAnimator.SpineInstance.Scale = new Vector2(2, 2);

        spineAnimator.DepthOffset = TurretDatabase.GetDepthOffset(TurretType);

        // Set up animation event listener for projectile spawning
        if (Network.IsClient)
        {
            spineAnimator.OnEvent += OnAnimationEvent;
        }

        OwnerPlot.OccupiedBy[GridX, GridY] = Entity;
    }

    public override void Update()
    {
        if (Network.IsServer)
        {
            ServerUpdate();
        }

        if (Network.IsClient)
        {
            ClientUpdate();
        }
    }

    void ServerUpdate()
    {
        if (!OwnerPlot.Alive() || !OwnerPlot.Owner.Alive())
        {
            Network.Despawn(Entity);
            Entity.Destroy();
            return;
        }

        if (Time.TimeSinceStartup - lastAttackTime < AttackCooldown)
        {
            return;
        }

        var target = FindTarget();
        if (target.Alive())
        {
            currentTarget = target;
            lastAttackTime = Time.TimeSinceStartup;
            CallClient_PlayShootAnimation(target);
            // Server also spawns its own projectile on the fire event
            SpawnProjectile(target);
        }
    }

    void ClientUpdate()
    {
        if (isShooting)
        {
            shootAnimTimer += Time.DeltaTime;
            if (shootAnimTimer > 0.5f)
            {
                isShooting = false;
                if (spineAnimator != null)
                {
                    spineAnimator.SpineInstance.SetAnimation("Idle", true);
                }
            }
        }
    }

    Entity FindTarget()
    {
        Entity closestEnemy = default;
        float closestDistance = float.MaxValue;

        foreach (var enemy in Scene.Components<Enemy>())
        {
            if (!enemy.Entity.Alive())
                continue;

            if (enemy.LaneIndex != GridY)
                continue;

            if (enemy.Health <= 0)
                continue;

            if (enemy.Position.X < Entity.Position.X)
                continue;

            var distance = (enemy.Entity.Position - Entity.Position).Length;
            if (distance <= AttackRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.Entity;
            }
        }

        return closestEnemy;
    }

    void SpawnProjectile(Entity target)
    {
        if (!target.Alive())
            return;

        // Get spawn position from bone
        var spawnPos = spineAnimator.GetBonePosition("PROJECTILE_SPAWN");
        var projectileEntity = Entity.Create();
        projectileEntity.Position = spawnPos;

        var projectileType = GetProjectileType(TurretType);
        
        var projSpineAnimator = projectileEntity.AddComponent<Spine_Animator>();
        var spinePath = GetProjectileSpinePath(projectileType);
        var skeleton = Assets.GetAsset<SpineSkeletonAsset>(spinePath);
        projSpineAnimator.SpineInstance.SetSkeleton(skeleton);
        projSpineAnimator.SpineInstance.SetAnimation("Idle", true);
        projSpineAnimator.SpineInstance.Scale = new Vector2(2, 2);

        var projectile = projectileEntity.AddComponent<Projectile>();
        projectile.TargetEnemy = target;
        projectile.Damage = Damage;
        projectile.ProjectileType = projectileType;
        projectile.IsServerProjectile = Network.IsServer;

        // Don't network spawn - each client creates their own
    }

    void OnAnimationEvent(string eventName)
    {
        // This is called on clients when the "fire" animation event triggers
        if (currentTarget.Alive() && eventName == "fire")
        {
            SpawnProjectile(currentTarget);
        }
    }

    string GetProjectileSpinePath(string type)
    {
        return type switch
        {
            "pea" => "anim/Peashooter/Peashooter_Projectile/Peashooter_Projectile.spine",
            "spike" => "anim/Cactus/Cactus_Projectile/Cactus_Projectile.spine",
            "star" => "anim/Starfruit/Starfruit_Projectile/Starfruit_Projectile.spine",
            "melon" => "anim/Melonpult/Melonpult_Projectile/Melonpult_Projectile.spine",
            "cob" => "anim/Cobcannon/Cobcannon_Projectile/Cobcannon_Projectile.spine",
            _ => "anim/Peashooter/Peashooter_Projectile/Peashooter_Projectile.spine",
        };
    }

    string GetProjectileType(string turretType)
    {
        return turretType switch
        {
            "peashooter" => "pea",
            "cactus" => "spike",
            "starfruit" => "star",
            "melonpult" => "melon",
            "cobcannon" => "cob",
            _ => "pea",
        };
    }

    [ClientRpc]
    public void PlayShootAnimation(Entity target)
    {
        if (spineAnimator != null)
        {
            currentTarget = target;
            spineAnimator.SpineInstance.SetAnimation("Shoot", false);
            isShooting = true;
            shootAnimTimer = 0;
        }
    }
}

