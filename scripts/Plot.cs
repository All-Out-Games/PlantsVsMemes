using System.Runtime.CompilerServices;
using AO;

public class PlotsManager : System<PlotsManager>
{
    public static Material LockedMaterial;

    public override void Start()
    {
        var shader = Assets.GetAsset<ShaderAsset>("$AO/shaders/greyscale.aosl");
        LockedMaterial = IM.CreateMaterial(shader);
    }

    public List<Plot> plots = new List<Plot>();

    public void ClaimPlot(MyPlayer player)
    {
        plots.FirstOrDefault(p => p.Owner.Alive() == false)?.Claim(player);
    }
}

public partial class Plot : Component
{
    [Serialized] public Entity TilesParent;
    [Serialized] public Entity PortalsParent;
    [Serialized] public Entity ExitsParent;
    [Serialized] public Entity CollectiblesPlacementParent;


    public Entity[,] TurretsOccupiedBy = new Entity[9, 7];
    public Entity[,] CollectiblesPlacementOccupiedBy = new Entity[8, 14];

    public MyPlayer Owner;

    public Sprite_Renderer[,] TurretTiles;
    public Vector4[,] OriginalTileColors;

    public Sprite_Renderer[,] CollectiblesPlacementTiles;

    public Entity[] Portals;
    public Spine_Animator[] Exits;

    public override void Awake()
    {
        PlotsManager.Instance.plots.Add(this);

        TurretTiles = new Sprite_Renderer[9, 7];
        OriginalTileColors = new Vector4[9, 7];
        foreach (var child in TilesParent.Children)
        {
            var spriteRenderer = child.GetComponent<Sprite_Renderer>();
            var tileCoords = GetTurretTileCoordsFromLocalPosition(child.LocalPosition);
            TurretTiles[tileCoords.Item1, tileCoords.Item2] = spriteRenderer;
            OriginalTileColors[tileCoords.Item1, tileCoords.Item2] = spriteRenderer.Tint;
        }

        Portals = new Entity[7];
        foreach (var child in PortalsParent.Children)
        {
            var tileCoords = GetTurretTileCoordsFromLocalPosition(child.LocalPosition);
            Portals[tileCoords.Item2] = child;
        }

        Exits = new Spine_Animator[7];
        foreach (var child in ExitsParent.Children)
        {
            var tileCoords = GetTurretTileCoordsFromLocalPosition(child.LocalPosition);
            Exits[tileCoords.Item2] = child.GetComponent<Spine_Animator>();
        }

        CollectiblesPlacementTiles = new Sprite_Renderer[8, 14];
        foreach (var area in CollectiblesPlacementParent.Children)
        {
            foreach (var tileEntity in area.Children)
            {
                var tileCoords = GetCollectiblesPlacementTileCoordsFromWorldPosition(tileEntity.Position);
                CollectiblesPlacementTiles[tileCoords.Item1, tileCoords.Item2] = tileEntity.GetComponent<Sprite_Renderer>();
            }
        }
    }

    public (int, int) GetTurretTileCoordsFromLocalPosition(Vector2 position)
    {
        var x = (int) Math.Round(position.X / 2);
        var y = (int) Math.Round((position.Y + 6)/2);
        return (x, y);
    }

    public (int, int) GetTurretTileCoordsFromWorldPosition(Vector2 position)
    {
        var tilePosition = position - Entity.Position;
        var tileX = (int) Math.Round(tilePosition.X / 2);
        var tileY = (int) Math.Round((tilePosition.Y + 6) / 2);
        return (tileX, tileY);
    }

    public (int, int) GetCollectiblesPlacementTileCoordsFromWorldPosition(Vector2 position)
    {
        var tilesZero = new Vector2(-10.5f, -6.5f);
        var tilePosition = position - tilesZero - Entity.Position;
        var tileX = (int) Math.Round(tilePosition.X);
        var tileY = (int) Math.Round(tilePosition.Y);
        return (tileX, tileY);
    }

    public RowBits GetRowBits(int row)
    {
        return (RowBits)(1 << row);
    }

    public override void Update()
    {
        if (Owner.Alive() == false)
        {
            return;
        }

        var unlockedRows = (RowBits) Owner.UnlockedRows.Value;
        for (var x = 0; x < TurretTiles.GetLength(0); x++)
        {
            for (var y = 0; y < TurretTiles.GetLength(1); y++)
            {
                bool rowUnlocked = unlockedRows.HasFlag(GetRowBits(y));
                if (rowUnlocked)
                {
                    TurretTiles[x, y].SetMaterial(null, false);
                }
                else
                {
                    
                    TurretTiles[x, y].SetMaterial(PlotsManager.LockedMaterial, false);
                }
            }
        }

        for (var i = 0; i < Portals.Length; i++)
        {
            Portals[i].LocalEnabled = unlockedRows.HasFlag(GetRowBits(i));
        }

        for (var i = 0; i < Exits.Length; i++)
        {
            Exits[i].Entity.LocalEnabled = unlockedRows.HasFlag(GetRowBits(i));
        }
    }

    public void Claim(MyPlayer player)
    {
        Owner = player;
    }

    public bool CanTurretPlaceAt(int x, int y, int width, int height)
    {
        if (!Owner.Alive())
            return false;

        var unlockedRows = (RowBits)Owner.UnlockedRows.Value;

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int checkX = x + dx;
                int checkY = y + dy;

                if (checkX < 0 || checkX >= 9 || checkY < 0 || checkY >= 7)
                    return false;

                bool rowUnlocked = unlockedRows.HasFlag(GetRowBits(checkY));
                if (!rowUnlocked)
                    return false;

                if (TurretsOccupiedBy[checkX, checkY].Alive())
                    return false;
            }
        }

        return true;
    }

    public bool CanPlaceCollectibleAt(int x, int y, int width, int height)
    {
        if (!Owner.Alive())
            return false;

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int checkX = x + dx;
                int checkY = y + dy;
                if (CollectiblesPlacementOccupiedBy[checkX, checkY].Alive())
                    return false;
            }
        }

        return true;
    }

    public Vector2 GetWorldPositionForTurretTile(int x, int y)
    {
        return TurretTiles[x, y].Entity.Position;
    }

    public Vector2 GetWorldPositionForCollectiblesPlacementTile(int x, int y)
    {
        return CollectiblesPlacementTiles[x, y].Entity.Position;
    }

    // [ServerRpc]
    // public void ServerRpc_PlaceCollectible(int x, int y, int inventoryIndex)
    // {
    //     if (!Owner.Alive())
    //         return;

    //     if (inventoryIndex < 0 || inventoryIndex >= Owner.LocalInventory.Count)
    //         return;

    //     var collectible = Owner.LocalInventory[inventoryIndex];
        
    //     if (!CanPlaceAt(x, y, collectible.Width, collectible.Height))
    //         return;

    //     var collectibleEntity = Entity.Create();
    //     collectibleEntity.Position = GetWorldPositionForTile(x, y);

    //     var spineAnimator = collectibleEntity.AddComponent<Spine_Animator>();
    //     var spinePath = CollectibleDatabase.GetSpinePath(collectible.EnemyType);
    //     var skeleton = Assets.GetAsset<SpineSkeletonAsset>(spinePath);
    //     spineAnimator.SpineInstance.SetSkeleton(skeleton);
    //     spineAnimator.SpineInstance.SetAnimation("Idle", true);
    //     float scale = Math.Max(collectible.Width, collectible.Height) * 0.3f;
    //     spineAnimator.SpineInstance.Scale = new Vector2(scale, scale);

    //     var placed = collectibleEntity.AddComponent<PlacedCollectible>();
    //     placed.CollectibleType.Set(collectible.EnemyType);
    //     placed.Width = collectible.Width;
    //     placed.Height = collectible.Height;
    //     placed.MoneyPerSecond = collectible.MoneyPerSecond;
    //     placed.OwnerPlayer = Owner;
    //     placed.OwnerPlot = this;
    //     placed.GridX = x;
    //     placed.GridY = y;

    //     Network.Spawn(collectibleEntity);

    //     for (int dx = 0; dx < collectible.Width; dx++)
    //     {
    //         for (int dy = 0; dy < collectible.Height; dy++)
    //         {
    //             OccupiedBy[x + dx, y + dy] = collectibleEntity;
    //         }
    //     }

    //     Owner.RemoveFromInventory(inventoryIndex);
    // }

    // [ServerRpc]
    // public void RemovePlacement(int x, int y)
    // {
    //     if (!Owner.Alive())
    //         return;

    //     if (x < 0 || x >= 9 || y < 0 || y >= 7)
    //         return;

    //     var entity = OccupiedBy[x, y];
    //     if (!entity.Alive())
    //         return;

    //     var placedCollectible = entity.GetComponent<PlacedCollectible>();
    //     if (placedCollectible != null)
    //     {
    //         for (int dx = 0; dx < placedCollectible.Width; dx++)
    //         {
    //             for (int dy = 0; dy < placedCollectible.Height; dy++)
    //             {
    //                 int clearX = placedCollectible.GridX + dx;
    //                 int clearY = placedCollectible.GridY + dy;
    //                 if (clearX >= 0 && clearX < 9 && clearY >= 0 && clearY < 7)
    //                 {
    //                     OccupiedBy[clearX, clearY] = default;
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         OccupiedBy[x, y] = default;
    //     }

    //     Network.Despawn(entity);
    //     entity.Destroy();
    // }
}