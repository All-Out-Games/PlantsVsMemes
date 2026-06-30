using AO;

public class EnemySpawner : Component
{
    [Serialized] public Plot TargetPlot;
    
    public float spawnInterval = 11.0f;
    public float[] lastSpawnTimes;

    ulong rngSeed;

    public override void Awake()
    {
        rngSeed = (ulong)new Random().NextInt64();
        lastSpawnTimes = new float[7];
        for (int i = 0; i < 7; i++)
        {
            lastSpawnTimes[i] = Time.TimeSinceStartup + RNG.RangeFloat(ref rngSeed, 0, spawnInterval) - spawnInterval;
        }
    }

    public override void Update()
    {
        if (!Network.IsServer)
            return;

        if (!TargetPlot.Alive() || !TargetPlot.Owner.Alive())
            return;

        for (int i = 0; i < 7; i++)
        {
            var lastSpawn = lastSpawnTimes[i];
            if (Time.TimeSinceStartup - lastSpawn >= spawnInterval)
            {
                SpawnEnemy(i);
                lastSpawnTimes[i] = Time.TimeSinceStartup;
            }
        }
    }

    void SpawnEnemy(int lane)
    {
        var unlockedRows = (RowBits)TargetPlot.Owner.UnlockedRows.Value;
        if (!unlockedRows.HasFlag(TargetPlot.GetRowBits(lane)))
            return;
            
        string enemyType = BrainrotCatalog.GetRandomId();
        BrainrotModifier modifier = BrainrotCatalog.GetRandomModifier();

        var enemyEntity = Entity.Create();
        enemyEntity.AddComponent<Sprite_Renderer>();
        var enemy = enemyEntity.AddComponent<Enemy>(e => {
            e.OwnerPlot = TargetPlot;
            e.LaneIndex = lane;
            e.EnemyType = enemyType;
            e.Health = GetEnemyHealth(enemyType);
            e.MaxHealth = e.Health;
            e.Modifier = modifier;

            e.StartPosition = TargetPlot.Portals[lane].Position;
            e.Entity.Position = e.StartPosition;
            e.TargetPosition = TargetPlot.Exits[lane].Entity.Position;
        });

        Network.Spawn(enemyEntity);
    }

    int GetEnemyHealth(string enemyType)
    {
        var entry = BrainrotCatalog.Get(enemyType);
        
        // Base health by rarity (exponential scaling)
        int baseHealth = entry.Rarity switch
        {
            BrainrotValueRarity.Common => 100,
            BrainrotValueRarity.Rare => 1000,
            BrainrotValueRarity.Epic => 10000,
            BrainrotValueRarity.Legendary => 100000,
            BrainrotValueRarity.Mythic => 1000000,
            BrainrotValueRarity.Ethereal => 5000000,
            BrainrotValueRarity.Primal => 10000000,
            _ => 100
        };
        
        // Small adjustment based on gold generation (adds up to ~30% variation within same rarity)
        float goldAdjustment = 1.0f + (entry.BaseGoldGeneration / 10000.0f);
        
        return (int)(baseHealth * goldAdjustment);
    }
}

