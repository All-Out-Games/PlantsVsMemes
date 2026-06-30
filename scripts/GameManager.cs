using AO;

public class GameManager : System<GameManager>
{
    public float ServerLuck = 1.0f;

    Entity hudEntity;

    public Material GoldMaterial;
	public Material DiamondMaterial;
	public Material RainbowMaterial;
	public Material WoodsMaterial;
	public Material VoidMaterial;
	public Material BleachMaterial;

    public override void Awake()
    {
        // Initialize collectible size database (both client and server need this)
        CollectibleDatabase.Initialize();

        MyPlayer.PeashooterItem = Item_Definition.Create(new ItemDescription()
        {
            Id = "peashooter",
            Icon = "grass.png",
            Name = "Peashooter",
            StackSize = 1,
        });

        MyPlayer.CactusItem = Item_Definition.Create(new ItemDescription()
        {
            Id = "cactus",
            Icon = "tile.png",
            Name = "Cactus",
            StackSize = 1,
        });

        MyPlayer.StarfruitItem = Item_Definition.Create(new ItemDescription()
        {
            Id = "starfruit",
            Icon = "grass.png",
            Name = "Starfruit",
            StackSize = 1,
        });

        MyPlayer.MelonpultItem = Item_Definition.Create(new ItemDescription()
        {
            Id = "melonpult",
            Icon = "tile.png",
            Name = "Melonpult",
            StackSize = 1,
        });

        MyPlayer.CobcannonItem = Item_Definition.Create(new ItemDescription()
        {
            Id = "cobcannon",
            Icon = "tile.png",
            Name = "Cobcannon",
            StackSize = 1,
        });

        if (Network.IsClient)
        {
            hudEntity = Entity.Create();
            hudEntity.AddComponent<GameHUD>();
            
            // FusionSystem, Rebirth, and CraftingSystem are Systems (singletons) and auto-initialize
            // They will be available via FusionSystem.Instance, Rebirth.Instance, and CraftingSystem.Instance
        }

        Economy.RegisterCurrency(Config.Currency_Gold, Config.Currency_GoldIcon);
    }

    public override void Start()
    {
        if (Network.IsClient)
        {
            var shader = Assets.GetAsset<ShaderAsset>("shaders/color_shimmer.aosl");
            if (shader != null)
            {
                GoldMaterial = IM.CreateMaterial(shader);
                if (GoldMaterial != null)
                {
                    GoldMaterial.SetUniform("noise_texture", References.NoiseTexture);
                    // Default palette (gold). For diamond, set a cool palette at runtime.
                    GoldMaterial.SetUniform("paletteShadow", new Vector3(0.32f, 0.21f, 0.05f));
                    GoldMaterial.SetUniform("paletteMid", new Vector3(0.937f, 0.749f, 0.016f));
                    GoldMaterial.SetUniform("paletteHi", new Vector3(1.00f, 0.92f, 0.25f));
                    GoldMaterial.SetUniform("paletteBlackPoint", 0.10f);
                    GoldMaterial.SetUniform("paletteWhitePoint", 0.90f);
                    GoldMaterial.SetUniform("paletteGamma", 0.90f);
                    GoldMaterial.SetUniform("tintStrength", 1.00f);
                    GoldMaterial.SetUniform("shadowBoost", 0.40f);
                    GoldMaterial.SetUniform("outlineLow", 0.06f);
                    GoldMaterial.SetUniform("outlineHigh", 0.12f);
                    GoldMaterial.SetUniform("outlineDarken", 0.65f);
                    GoldMaterial.SetUniform("outlineLumaMin", 0.00f);
                    GoldMaterial.SetUniform("outlineLumaMax", 0.18f);
                }
                DiamondMaterial = IM.CreateMaterial(shader);
                if (DiamondMaterial != null)
                {
                    DiamondMaterial.SetUniform("noise_texture", References.NoiseTexture);
                    // Diamond palette (deeper, bluer, higher saturation)
                    DiamondMaterial.SetUniform("paletteShadow", new Vector3(0.03f, 0.09f, 0.45f));
                    DiamondMaterial.SetUniform("paletteMid", new Vector3(0.30f, 0.82f, 1.00f));
                    DiamondMaterial.SetUniform("paletteHi", new Vector3(0.75f, 0.98f, 1.00f));
                    DiamondMaterial.SetUniform("paletteBlackPoint", 0.08f);
                    DiamondMaterial.SetUniform("paletteWhitePoint", 0.95f);
                    DiamondMaterial.SetUniform("paletteGamma", 0.80f);
                    DiamondMaterial.SetUniform("tintStrength", 1.00f);
                    DiamondMaterial.SetUniform("shadowBoost", 0.70f);
                    DiamondMaterial.SetUniform("outlineLow", 0.06f);
                    DiamondMaterial.SetUniform("outlineHigh", 0.12f);
                    DiamondMaterial.SetUniform("outlineDarken", 0.65f);
                    DiamondMaterial.SetUniform("outlineLumaMin", 0.00f);
                    DiamondMaterial.SetUniform("outlineLumaMax", 0.20f);
                }
            }


            var shaderRainbow = Assets.GetAsset<ShaderAsset>("shaders/rainbow.aosl");
            if (shaderRainbow != null)
            {
                RainbowMaterial = IM.CreateMaterial(shaderRainbow);
                if (RainbowMaterial != null)
                {
                    // Minimal uniforms for simplified rainbow shader
                    RainbowMaterial.SetUniform("rainbowScale", 1.2f);
                    RainbowMaterial.SetUniform("hueSpeed", 0.20f);
                    RainbowMaterial.SetUniform("hueSaturation", 1.00f);
                    RainbowMaterial.SetUniform("hueValue", 1.00f);
                    RainbowMaterial.SetUniform("tintStrength", 0.65f);
                    // Preserve black outlines
                    RainbowMaterial.SetUniform("outlineLow", 0.04f);
                    RainbowMaterial.SetUniform("outlineHigh", 0.16f);
                }
            }

            var shaderWoods = Assets.GetAsset<ShaderAsset>("shaders/woods.aosl");
            if (shaderWoods != null)
            {
                WoodsMaterial = IM.CreateMaterial(shaderWoods);
                if (WoodsMaterial != null)
                {
                    // Woods shader: tileable leaves overlay on bright-but-not-white
                    WoodsMaterial.SetUniform("leavesTex", References.LeafTileable);
                    // Bigger leaves: decrease tiling; reduce scroll to keep similar perceived speed
                    WoodsMaterial.SetUniform("leavesTiling", new Vector2(.5f, 0.8f));
                    WoodsMaterial.SetUniform("leavesScroll", new Vector2(0.0f, -0.02f));
                    WoodsMaterial.SetUniform("leavesStrength", 1f);
                    WoodsMaterial.SetUniform("leavesTint", new Vector4(0.7f, 1.0f, 0.7f, 1.0f));
                    WoodsMaterial.SetUniform("whiteLow", 0.84f);
                    WoodsMaterial.SetUniform("whiteHigh", 0.98f);
                    WoodsMaterial.SetUniform("darkLow", 0.12f);
                    WoodsMaterial.SetUniform("darkHigh", 0.3f);
                }
            }


            var shaderVoid = Assets.GetAsset<ShaderAsset>("shaders/void.aosl");
            if (shaderVoid != null)
            {
                VoidMaterial = IM.CreateMaterial(shaderVoid);
                if (VoidMaterial != null)
                {
                    VoidMaterial.SetUniform("noise_texture", References.NoiseTexture);
                    VoidMaterial.SetUniform("void_texture", References.SpaceNebula);


                    // Defaults
                    VoidMaterial.SetUniform("tintStrength", .96f);
                    VoidMaterial.SetUniform("shadowBoost", 0.6f);
                    VoidMaterial.SetUniform("paletteBlackPoint", 0.08f);
                    VoidMaterial.SetUniform("paletteWhitePoint", 0.95f);
                    VoidMaterial.SetUniform("paletteGamma", 0.9f);
                    // Screen-space mapping so the image sits on top of objects uniformly
                    VoidMaterial.SetUniform("voidScreenTiling", new Vector2(1.0f, 1.0f));
                    // Keep sprite-UV tiling at 1 (unused when screen mapping is active)
                    VoidMaterial.SetUniform("voidTiling", new Vector2(.5f, .5f));
                    VoidMaterial.SetUniform("voidScroll", new Vector2(0.02f, -0.01f));
                    VoidMaterial.SetUniform("voidRotation", 0.0f);
                    VoidMaterial.SetUniform("distortStrength", 0.08f);
                    VoidMaterial.SetUniform("distortScale", 2.5f);
                    VoidMaterial.SetUniform("outlineLow", 0.04f);
                    VoidMaterial.SetUniform("outlineHigh", 0.16f);
                    VoidMaterial.SetUniform("outlineDarken", 1f);
                    VoidMaterial.SetUniform("outlineLumaMin", 0.00f);
                    VoidMaterial.SetUniform("outlineLumaMax", 0.3f);
                }

            }
            // Bleach-to-white material used by EvolutionUI
            var shaderBleach = Assets.GetAsset<ShaderAsset>("shaders/bleach_to_white.aosl");
            if (shaderBleach != null)
            {
                BleachMaterial = IM.CreateMaterial(shaderBleach);
            }
        }

        if (Network.IsServer)
        {
            var entityGM = Entity.Create();
            entityGM.AddComponent<TurretShop>();
            entityGM.AddComponent<CraftingSystem>();
            foreach (var plot in References.Instance.PlotsParent.Children)
            {
                var e = Entity.Instantiate(Assets.GetAsset<Prefab>("Plot.prefab"));
                e.Position = plot.Position;
                Network.Spawn(e);
            }
            Network.Spawn(entityGM);
        }
    }
}

