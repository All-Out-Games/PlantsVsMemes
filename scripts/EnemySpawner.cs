using AO;

public class EnemySpawner : Component
{
    [Serialized] public Plot TargetPlot;
    
    public float spawnInterval = 7.0f;
    public float[] lastSpawnTimes;

    ulong rngSeed;

    public override void Awake()
    {
        rngSeed = (ulong)new Random().NextInt64();
        lastSpawnTimes = new float[7];
        for (int i = 0; i < 7; i++)
        {
            lastSpawnTimes[i] = Time.TimeSinceStartup + RNG.RangeFloat(ref rngSeed, 0, spawnInterval);
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
        int health = GetEnemyHealth();

        var enemyEntity = Entity.Create();
        enemyEntity.AddComponent<Sprite_Renderer>();
        var enemy = enemyEntity.AddComponent<Enemy>(e => {
            e.OwnerPlot = TargetPlot;
            e.LaneIndex = lane;
            e.EnemyType = enemyType;
            e.Health = health;
            e.Modifier = modifier;

            e.StartPosition = TargetPlot.Portals[lane].Position;
            e.Entity.Position = e.StartPosition;
            e.TargetPosition = TargetPlot.Exits[lane].Entity.Position;
        });

        Network.Spawn(enemyEntity);
    }

    int GetEnemyHealth()
    {
        return 100;
    }
}

