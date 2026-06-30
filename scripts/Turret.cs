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

    [Serialized] public float TimeUntilNextAttack = 0;

    Spine_Animator spineAnimator;
    Interactable interactable;
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
        spineAnimator.SpineInstance.Scale = new Vector2(2, 2);
        spineAnimator.DepthOffset = TurretDatabase.GetDepthOffset(TurretType);
        spineAnimator.OnEvent += OnAnimationEvent;
        var sm = StateMachine.Create();
        spineAnimator.SpineInstance.SetStateMachine(sm, true);
        var mainLayer = sm.CreateLayer("main_layer", 0);
        var idleState = mainLayer.CreateState("Idle", 0, true);
        var shootState = mainLayer.CreateState("Shoot", 0, false);
        var shootTrigger = sm.CreateVariable("shoot", StateMachineVariableKind.TRIGGER);
        mainLayer.InitialState = idleState;
        mainLayer.CreateTransition(idleState, shootState, false).CreateTriggerCondition(shootTrigger);
        mainLayer.CreateTransition(shootState, idleState, true);

        OwnerPlot.TurretsOccupiedBy[GridX, GridY] = Entity;

        // Setup interactable
        interactable = Entity.GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.CanUseCallback += (Player p) =>
            {
                var myPlayer = (MyPlayer)p;
                // Only the owner can pick it up
                return OwnerPlot.Alive() && OwnerPlot.Owner.Alive() && OwnerPlot.Owner == myPlayer;
            };

            interactable.OnInteract = (Player p) =>
            {
                if (!Network.IsServer) return;

                var myPlayer = (MyPlayer)p;
                if (!myPlayer.Entity.Alive() || !OwnerPlot.Alive() || OwnerPlot.Owner != myPlayer)
                    return;

                // Check if player has inventory space
                bool hasSpace = false;
                foreach (var itm in myPlayer.DefaultInventory.Items)
                {
                    if (itm == null)
                    {
                        hasSpace = true;
                        break;
                    }
                }

                if (!hasSpace)
                {
                    myPlayer.CallClient_ShowMessage("Inventory is full!", new RPCOptions() { Target = myPlayer });
                    return;
                }

                // Find the item definition for this turret type
                Item_Definition itemDef = null;
                if (TurretType == "peashooter") itemDef = MyPlayer.PeashooterItem;
                else if (TurretType == "cactus") itemDef = MyPlayer.CactusItem;
                else if (TurretType == "starfruit") itemDef = MyPlayer.StarfruitItem;
                else if (TurretType == "melonpult") itemDef = MyPlayer.MelonpultItem;
                else if (TurretType == "cobcannon") itemDef = MyPlayer.CobcannonItem;

                if (itemDef == null)
                    return;

                // Create item and add to inventory
                var item = Inventory.CreateItem(itemDef, 1);
                if (Inventory.CanMoveItemToInventory(item, myPlayer.DefaultInventory, out var willDestroyItem))
                {
                    Inventory.MoveItemToInventory(item, myPlayer.DefaultInventory);

                    // Clear the occupied tile
                    if (OwnerPlot.Alive())
                    {
                        OwnerPlot.TurretsOccupiedBy[GridX, GridY] = default;
                    }

                    // Despawn and destroy
                    Network.Despawn(Entity);
                    Entity.Destroy();
                    
                    // Save after picking up
                    myPlayer.SavePlacedItems();
                }
                else
                {
                    Inventory.DestroyItem(item);
                }
            };
        }
    }

    public override void Update()
    {
        // if (!OwnerPlot.Alive() || !OwnerPlot.Owner.Alive())
        // {
        //     Network.Despawn(Entity);
        //     Entity.Destroy();
        //     return;
        // }

        // Update interactable text
        if (Network.IsClient && interactable != null)
        {
            interactable.Text = "Pick up turret";
        }

        TimeUntilNextAttack -= Time.DeltaTime;
        if (TimeUntilNextAttack > 0)
        {
            return;
        }

        var target = FindTarget();
        if (target.Alive())
        {
            currentTarget = target;
            TimeUntilNextAttack = AttackCooldown;
            spineAnimator.SpineInstance.StateMachine.SetTrigger("shoot");
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

        var projectileType = GetProjectileType(TurretType);
        bool isCobcannon = projectileType == "cob";

        // Cobcannon spawns above target, others spawn at turret bone
        Vector2 spawnPos;
        if (isCobcannon)
        {
            // Spawn above the target
            float heightAboveTarget = 12f;
            spawnPos = target.Position + new Vector2(0, heightAboveTarget);
        }
        else
        {
            // Get spawn position from bone
            spawnPos = spineAnimator.GetBonePosition("PROJECTILE_SPAWN");
        }

        var projectileEntity = Entity.Create();
        projectileEntity.Position = spawnPos;
        
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
        projectile.IsCobcannon = isCobcannon;

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
}

