using AO;
public class References : Component
{
    public static References Instance;
    public override void Awake() => Instance = this;
    
    [Serialized] public Entity PlotsParent;

    public static AudioAsset MusicBG = Assets.KeepLoaded<AudioAsset>("Sounds/Music/Cool_Vibes.wav", synchronous: false);
    public static AudioAsset MusicBG_WoodEvent = Assets.KeepLoaded<AudioAsset>("Sounds/Music/Darkest_Child.wav", synchronous: false);
    
    public static AudioAsset RocketExplodeSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/rocket_explode.wav", synchronous: false);
    public static AudioAsset RocketShootSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/rocketlauncher_shoot.wav", synchronous: false);
    public static AudioAsset FartGunShootSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/fartgun_shoot_02.wav", synchronous: false);
    public static AudioAsset BearTrapPlacedSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/beartrap_spawn.wav", synchronous: false);
    public static AudioAsset BearTrapActivatedSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/beartrap_activated.wav", synchronous: false);
    public static AudioAsset CloneSpawnSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/clone_spawn.wav", synchronous: false);
    public static AudioAsset TaserShoot = Assets.KeepLoaded<AudioAsset>("Sounds/Items/taser.wav", synchronous: false);
    public static AudioAsset TaserStun = Assets.KeepLoaded<AudioAsset>("Sounds/Items/taser_stun.wav", synchronous: false);
    public static AudioAsset ActivateStoneHeadSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/murder_mirror-reviever_activate.wav", synchronous: false);
    public static AudioAsset TurnedToStoneHeadSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/cm_turn_to_stone.wav", synchronous: false);
    public static AudioAsset SwapPositionsSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/swap_positions.wav", synchronous: false);
    public static AudioAsset TurretPlaceSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/turret_place.wav", synchronous: false);
    public static AudioAsset TurretShootSound = Assets.KeepLoaded<AudioAsset>("Sounds/Items/turret_shoot.wav", synchronous: false);

    public static Texture BoostersStorageSize2 = Assets.KeepLoaded<Texture>("ui/storage_x2.png", synchronous: false);
    public static Texture BoostersStorageSize3 = Assets.KeepLoaded<Texture>("ui/storage_x3.png", synchronous: false);
    public static Texture XP_boost_1 = Assets.KeepLoaded<Texture>("ui/xp_boost_1.png", synchronous: false);
    public static Texture XP_boost_2 = Assets.KeepLoaded<Texture>("ui/xp_boost_2.png", synchronous: false);
    public static Texture XP_boost_3 = Assets.KeepLoaded<Texture>("ui/xp_boost_3.png", synchronous: false);
    public static Texture StoneHead = Assets.KeepLoaded<Texture>("ui/stone_head.png", synchronous: true);
    public static Texture SuperSword = Assets.KeepLoaded<Texture>("ui/dream_sword.png", synchronous: true);
    public static Texture SpeedCoil = Assets.KeepLoaded<Texture>("ui/speedcoil.png", synchronous: true);
    public static Texture RPG = Assets.KeepLoaded<Texture>("ui/rocket.png", synchronous: true);
    public static Texture Skull = Assets.KeepLoaded<Texture>("ui/skull.png", synchronous: true);
    public static Texture PedestalSlot = Assets.KeepLoaded<Texture>("Textures/pedestale.png", synchronous: false);
    public static Texture Slap = Assets.KeepLoaded<Texture>("ui/weapon_1.png", synchronous: false);
    public static Texture FlyingCarpet = Assets.KeepLoaded<Texture>("ui/flyingcarpet.png", synchronous: false);
    public static Texture InvisibilityCape = Assets.KeepLoaded<Texture>("ui/cape.png", synchronous: false);
    public static Texture NoiseTexture = Assets.KeepLoaded<Texture>("noise.png", synchronous: true);
    public static Texture BombardinoCroccodillo = Assets.KeepLoaded<Texture>("Textures/memes/bombardino_crocodillo.png", synchronous: false);
    public static Texture BombombiniGusini = Assets.KeepLoaded<Texture>("Textures/memes/bombombini_gusini.png", synchronous: false);
    public static Texture TralaleroTralala = Assets.KeepLoaded<Texture>("Textures/memes/tralalero_tralala.png", synchronous: false);

    public static Texture ZibraZurbraZibralini = Assets.KeepLoaded<Texture>("Textures/memes/zibra_zurbra_zibralini.png", synchronous: false);
    public static Texture ZibraZurbraZibraliniPatapim = Assets.KeepLoaded<Texture>("Textures/memes/zibra_patapim.png", synchronous: false);
    public static Texture ZibraZurbraZibraliniPatapimAssassino = Assets.KeepLoaded<Texture>("Textures/memes/zibra_patapim_assassino.png", synchronous: false);


    public static Texture BallerinaCappucina = Assets.KeepLoaded<Texture>("Textures/memes/ballerina_cappuccina.png", synchronous: false);
    public static Texture BallerinaBrrBrr = Assets.KeepLoaded<Texture>("Textures/memes/ballerina_BrrBrr.png", synchronous: false);
    public static Texture BallerinaBrrBrrLirili = Assets.KeepLoaded<Texture>("Textures/memes/ballerina_BrrBrr_Lirili.png", synchronous: false);


    public static Texture Boneca = Assets.KeepLoaded<Texture>("Textures/memes/boneca.png", synchronous: false);
    public static Texture BonecaBurbaloni = Assets.KeepLoaded<Texture>("Textures/memes/boneca_burbaloni.png", synchronous: false);
    public static Texture BonecaBurbaloniTataSahur = Assets.KeepLoaded<Texture>("Textures/memes/boneca_burbaloni_tatasahur.png", synchronous: false);

    public static Texture BrrBrrPatapim = Assets.KeepLoaded<Texture>("Textures/memes/BrrBrr.png", synchronous: false);
    public static Texture BrrBrrTungTung = Assets.KeepLoaded<Texture>("Textures/memes/BrrBrr_TungTung.png", synchronous: false);
    public static Texture BrrBrrTungTungZibraZubra = Assets.KeepLoaded<Texture>("Textures/memes/BrrBrr_TungTung_ZibraZubra.png", synchronous: false);

    public static Texture BurbaloniLulilolli = Assets.KeepLoaded<Texture>("Textures/memes/burbaloni_lulilolli.png", synchronous: false);
    public static Texture BurbaloniLulilolliBananini = Assets.KeepLoaded<Texture>("Textures/memes/burbaloni_bananini.png", synchronous: false);
    public static Texture BurbaloniLulilolliBananiniFruttoDrillo = Assets.KeepLoaded<Texture>("Textures/memes/burbaloni_bananini_fruttodrillo.png", synchronous: false);

    public static Texture CappuccinoAssassino = Assets.KeepLoaded<Texture>("Textures/memes/cappuccino_assassino.png", synchronous: false);
    public static Texture CappuccinoBallerina = Assets.KeepLoaded<Texture>("Textures/memes/cappuccino_ballerina.png", synchronous: false);
    public static Texture CappuccinoBallerinaCacto = Assets.KeepLoaded<Texture>("Textures/memes/cappuccino_ballerina_Cacto.png", synchronous: false);

    public static Texture ChimpanziniBananini = Assets.KeepLoaded<Texture>("Textures/memes/chimpanzini_bananini.png", synchronous: false);
    public static Texture ChimpanziniBananiniWatermellini = Assets.KeepLoaded<Texture>("Textures/memes/chimpanzini_Watermellini.png", synchronous: false);
    public static Texture ChimpanziniBananiniCocosino = Assets.KeepLoaded<Texture>("Textures/memes/chimpanzini_Watermellini_Cocosino.png", synchronous: false);

    public static Texture CocofantoElefanto = Assets.KeepLoaded<Texture>("Textures/memes/cocofanto_elefanto.png", synchronous: false);
    public static Texture CocofantoCocosino = Assets.KeepLoaded<Texture>("Textures/memes/cocofanto_cocosino.png", synchronous: false);
    public static Texture CocofantoCocosinoCacto = Assets.KeepLoaded<Texture>("Textures/memes/cocofanto_cocosino_cacto.png", synchronous: false);

    public static Texture CocosinoRhino = Assets.KeepLoaded<Texture>("Textures/memes/cocosino_rhino.png", synchronous: false);
    public static Texture CocosinoRhinoLirili = Assets.KeepLoaded<Texture>("Textures/memes/cocosino_rhino_lirili.png", synchronous: false);
    public static Texture CocosinoRhinoLiriliFruttoDrillo = Assets.KeepLoaded<Texture>("Textures/memes/cocosino_rhino_lirili_fruttodrillo.png", synchronous: false);

    public static Texture GlorboFruttodrillo = Assets.KeepLoaded<Texture>("Textures/memes/glorbo_fruttodrillo.png", synchronous: false);
    public static Texture GlorboFruttodrilloLulilolli = Assets.KeepLoaded<Texture>("Textures/memes/glorbo_fruttodrillo_lulilolli.png", synchronous: false);
    public static Texture GlorboFruttodrilloLulilolliCappucina = Assets.KeepLoaded<Texture>("Textures/memes/glorbo_fruttodrillo_lulilolli_cappucina.png", synchronous: false);

    public static Texture IlCactoHipopotamo = Assets.KeepLoaded<Texture>("Textures/memes/il_cacto_hipopotamo.png", synchronous: false);
    public static Texture IlCactoHipopotamoBananini = Assets.KeepLoaded<Texture>("Textures/memes/il_cacto_hipopotamo_bananini.png", synchronous: false);
    public static Texture IlCactoHipopotamoBananiniTungtung = Assets.KeepLoaded<Texture>("Textures/memes/il_cacto_hipopotamo_bananini_tungtung.png", synchronous: false);

    public static Texture LiriliLarila = Assets.KeepLoaded<Texture>("Textures/memes/lirili_larila.png", synchronous: false);
    public static Texture LiriliLarilaAmbalabu = Assets.KeepLoaded<Texture>("Textures/memes/lirili_larila_ambalabu.png", synchronous: false);
    public static Texture LiriliLarilaAmbalabuTataSahur = Assets.KeepLoaded<Texture>("Textures/memes/lirili_larila_ambalabu_tatatasahur.png", synchronous: false);

    public static Texture TataSahur = Assets.KeepLoaded<Texture>("Textures/memes/ta_ta_sahur.png", synchronous: false);
    public static Texture TataSahurLarila = Assets.KeepLoaded<Texture>("Textures/memes/ta_ta_sahur_larila.png", synchronous: false);
    public static Texture TataSahurLarilaZibraZubra = Assets.KeepLoaded<Texture>("Textures/memes/ta_ta_sahur_larila_zibrazubra.png", synchronous: false);

    public static Texture TrigrrulliniWatermellini = Assets.KeepLoaded<Texture>("Textures/memes/trigrrullini_watermellini.png", synchronous: false);
    public static Texture TrigrrulliniWatermelliniCocofanto = Assets.KeepLoaded<Texture>("Textures/memes/trigrrullini_watermellini_cocofanto.png", synchronous: false);
    public static Texture TrigrrulliniWatermelliniCocofantoCappucino = Assets.KeepLoaded<Texture>("Textures/memes/trigrrullini_watermellini_cocofanto_cappucino.png", synchronous: false);

    public static Texture TungTung = Assets.KeepLoaded<Texture>("Textures/memes/tung_tung.png", synchronous: false);
    public static Texture TungTungBananini = Assets.KeepLoaded<Texture>("Textures/memes/tung_tung_bananini.png", synchronous: false);
    public static Texture TungTungBananiniWatermeloni = Assets.KeepLoaded<Texture>("Textures/memes/tung_tung_bananini_watermeloni.png", synchronous: false);

    public static Texture KarkerkarKurkur = Assets.KeepLoaded<Texture>("Textures/memes/Karkerkar_Kurkur.png", synchronous: false);
    public static Texture KarkerkarKurkurAmbalabu = Assets.KeepLoaded<Texture>("Textures/memes/Karkerkar_Kurkur_Ambalabu.png", synchronous: false);
    public static Texture KarkerkarKurkurAmbalabuTungtung = Assets.KeepLoaded<Texture>("Textures/memes/Karkerkar_Kurkur_ambalabu_tungtung.png", synchronous: false);

    public static Texture StrawberryElephant = Assets.KeepLoaded<Texture>("Textures/memes/strawberry_elephant.png", synchronous: false);
    public static Texture StrawberryElephantAmbalabu = Assets.KeepLoaded<Texture>("Textures/memes/strawberry_elephant_ambalabu.png", synchronous: false);
    public static Texture StrawberryElephantAmbalabuGolubiro = Assets.KeepLoaded<Texture>("Textures/memes/strawberry_elephant_ambalabu_golubiro.png", synchronous: false);

    public static Texture LaGrandeCombination = Assets.KeepLoaded<Texture>("Textures/memes/lagrandecombination.png", synchronous: false);
    public static Texture Udindindin = Assets.KeepLoaded<Texture>("Textures/memes/dindin.png", synchronous: false);
    public static Texture UdindindinTricTrac = Assets.KeepLoaded<Texture>("Textures/memes/dindin_Tric_Trac.png", synchronous: false);
    public static Texture UdindindinTricTracAssassino = Assets.KeepLoaded<Texture>("Textures/memes/dindin_Tric_Trac_Assassino.png", synchronous: false);

    public static Texture NightsMonster = Assets.KeepLoaded<Texture>("Textures/memes/woodsevent/nightsmonster.png", synchronous: false);
    public static Texture NightsMonsterCannelloni = Assets.KeepLoaded<Texture>("Textures/memes/woodsevent/nightsmonster_canneloni.png", synchronous: false);
    public static Texture NightsMonsterCannelloniTroppi = Assets.KeepLoaded<Texture>("Textures/memes/woodsevent/nightsmonster_canneloni_troppi.png", synchronous: false);

    public static Texture BrrBrrBaby = Assets.KeepLoaded<Texture>("Textures/memes/woodsevent/brr_brr_patapim_baby.png", synchronous: false);
    public static Texture ChimpanziniBaby = Assets.KeepLoaded<Texture>("Textures/memes/woodsevent/chimpanzini_bananini_baby.png", synchronous: false);
    public static Texture TralaleritoBaby = Assets.KeepLoaded<Texture>("Textures/memes/woodsevent/tralalero_tralala_baby.png", synchronous: false);
    public static Texture TrippiTroppiBaby = Assets.KeepLoaded<Texture>("Textures/memes/woodsevent/trippi_troppi_baby.png", synchronous: false);

    public static Texture LuckBlock_Legendary = Assets.KeepLoaded<Texture>("Textures/memes/luck_blocks/luck_block_legendary.png", synchronous: false);
    public static Texture LuckBlock_Mythic = Assets.KeepLoaded<Texture>("Textures/memes/luck_blocks/luck_block_mythical.png", synchronous: false);
    public static Texture LuckBlock_Ethereal = Assets.KeepLoaded<Texture>("Textures/memes/luck_blocks/luck_block_ethereal.png", synchronous: false);


    public static Texture ChatBox = Assets.KeepLoaded<Texture>("ui/chat_box.png", synchronous: false);

    public static Texture QuestBubble = Assets.KeepLoaded<Texture>("ui/chat_bubble.png", synchronous: false);
    public static AudioAsset LuckyBlockTextureChangedSound = Assets.KeepLoaded<AudioAsset>("Sounds/SFX_Interact_Pop_4.wav", synchronous: false);
    public static AudioAsset LuckyBlockTextureChangedEndedSound = Assets.KeepLoaded<AudioAsset>("Sounds/SFX_Drop_Designed.wav", synchronous: false);
    public static AudioAsset CardPassSound = Assets.KeepLoaded<AudioAsset>("Sounds/SFX_Interact_Pop_2.wav", synchronous: false);
    public static AudioAsset DiceRollSound = Assets.KeepLoaded<AudioAsset>("Sounds/dice_rolling.wav", synchronous: false);
    public static AudioAsset DiceLandSound = Assets.KeepLoaded<AudioAsset>("Sounds/dice_land.wav", synchronous: false);

    public static Texture RoundedRectangle = Assets.KeepLoaded<Texture>("$AO/rounded_rectangles/16.png", synchronous: false);
    public static Texture Diamond_Seemless = Assets.KeepLoaded<Texture>("ui/diamond_seemless.png", synchronous: false);
    public static Texture Rainbow_Seemless = Assets.KeepLoaded<Texture>("ui/rainbow_seemless.png", synchronous: false);
    public static Texture BubbleGum_Seemless = Assets.KeepLoaded<Texture>("ui/bubblegum_seemless.png", synchronous: false);
    public static Texture Flames_Seemless = Assets.KeepLoaded<Texture>("ui/flames_seemless.png", synchronous: false);
    public static Texture Toxic_Seemless = Assets.KeepLoaded<Texture>("ui/toxic_seemless.png", synchronous: false);
    public static Texture Gold_Seemless = Assets.KeepLoaded<Texture>("ui/gold_seemless.png", synchronous: false);



    public static Texture DiceBase = Assets.KeepLoaded<Texture>("Textures/Dices/Dice_Basic.png", synchronous: false);
    public static Texture DiceBubbleGum = Assets.KeepLoaded<Texture>("Textures/Dices/Dice_BubbleGum.png", synchronous: false);
    public static Texture DiceRainbow = Assets.KeepLoaded<Texture>("Textures/Dices/Dice_Rainbow.png", synchronous: false);
    public static Texture DiceCrystal = Assets.KeepLoaded<Texture>("Textures/Dices/Dice_Crystal.png", synchronous: false);
    public static Texture DiceToxic = Assets.KeepLoaded<Texture>("Textures/Dices/Dice_Toxic.png", synchronous: false);
    public static Texture DiceFlame = Assets.KeepLoaded<Texture>("Textures/Dices/Dice_Lava.png", synchronous: false);
    public static Texture DiceGold = Assets.KeepLoaded<Texture>("Textures/Dices/Dice_Gold.png", synchronous: false);

    public static Texture Star = Assets.KeepLoaded<Texture>("ui/star.png", synchronous: false);
    public static Texture Luck = Assets.KeepLoaded<Texture>("ui/luck.png", synchronous: false);


    public static Texture Parts_Ballerina_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/ballerina_cappuccina/ballerina_cappuccina_Mug_Face.png", synchronous: false);
    public static Texture Parts_Ballerina_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/ballerina_cappuccina/ballerina_cappuccina_Tutu.png", synchronous: false);
    public static Texture Parts_Ballerina_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/ballerina_cappuccina/ballerina_cappuccina_Eye.png", synchronous: false);

    public static Texture Parts_BrrBrr_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/brr_brr_patapim/Brr_Brr_Patapim_Head.png", synchronous: false);
    public static Texture Parts_BrrBrr_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/brr_brr_patapim/Brr_Brr_Patapim_Leg.png", synchronous: false);
    public static Texture Parts_BrrBrr_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/brr_brr_patapim/Brr_Brr_Patapim_Arm.png", synchronous: false);

    public static Texture Parts_TungTung_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/tung_tung/Tung_Tung_Head.png", synchronous: false);
    public static Texture Parts_TungTung_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/tung_tung/Tung_Tung_Body.png", synchronous: false);
    public static Texture Parts_TungTung_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/tung_tung/Tung_Tung_Eye.png", synchronous: false);

    public static Texture Parts_BonecaAmbalabu_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/boneca_ambalabu/Boneca_Ambalabu_Frog_Head.png", synchronous: false);
    public static Texture Parts_BonecaAmbalabu_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/boneca_ambalabu/Boneca_Ambalabu_Leg.png", synchronous: false);
    public static Texture Parts_BonecaAmbalabu_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/boneca_ambalabu/Boneca_Ambalabu_Tire.png", synchronous: false);

    public static Texture Parts_BurbaloniLulilolli_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/burbaloni_lulilolli/burbaloni_lulilolli_Head.png", synchronous: false);
    public static Texture Parts_BurbaloniLulilolli_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/burbaloni_lulilolli/burbaloni_lulilolli_Leg.png", synchronous: false);
    public static Texture Parts_BurbaloniLulilolli_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/burbaloni_lulilolli/burbaloni_lulilolli_Snout.png", synchronous: false);

    public static Texture Parts_CappuccinoAssassino_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/cappuccino_assassino/Cappuccino_assassino_Eye.png", synchronous: false);
    public static Texture Parts_CappuccinoAssassino_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/cappuccino_assassino/Cappuccino_assassino_Body.png", synchronous: false);
    public static Texture Parts_CappuccinoAssassino_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/cappuccino_assassino/Cappuccino_assassino_Arm_Sword.png", synchronous: false);

    public static Texture Parts_ChimpanziniBananini_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/chimpanzini_bananini/chimpanzini_bananini_Head.png", synchronous: false);
    public static Texture Parts_ChimpanziniBananini_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/chimpanzini_bananini/chimpanzini_bananini_Ear.png", synchronous: false);
    public static Texture Parts_ChimpanziniBananini_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/chimpanzini_bananini/chimpanzini_bananini_Large_Peel.png", synchronous: false);

    public static Texture Parts_CocofantoElephanto_1 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/cocofanto_elefanto/cocofanto_elefanto_Face.png", synchronous: false);
    public static Texture Parts_CocofantoElephanto_2 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/cocofanto_elefanto/cocofanto_elefanto_Nose.png", synchronous: false);
    public static Texture Parts_CocofantoElephanto_3 = Assets.KeepLoaded<Texture>("Textures/memes/Body_Parts/cocofanto_elefanto/cocofanto_elefanto_Leg.png", synchronous: false);

    public static Texture BarZoom = Assets.KeepLoaded<Texture>("ui/barzoom.png", synchronous: false);
    public static Texture BarZoomMiddle = Assets.KeepLoaded<Texture>("ui/barzoommiddle.png", synchronous: false);

    public static Texture SpaceNebula = Assets.KeepLoaded<Texture>("Textures/space_nebula.png", synchronous: true);

    public static Texture Studs = Assets.KeepLoaded<Texture>("ui/studs.png", synchronous: false);
    public static Texture ModalSimpleGrey = Assets.KeepLoaded<Texture>("ui/modal_simple_grey1.png", synchronous: false);
    public static Texture ModalSimpleWhite = Assets.KeepLoaded<Texture>("ui/modal_simple_white1.png", synchronous: false);
    public static Texture MoneyStack = Assets.KeepLoaded<Texture>("ui/money_stack.png", synchronous: false);
    public static Texture MoneyStackLot = Assets.KeepLoaded<Texture>("ui/money_stack_lot.png", synchronous: false);
    public static Texture RayBurst1 = Assets.KeepLoaded<Texture>("ui/ray_burst.png", synchronous: false);
    public static Texture RayBurst2 = Assets.KeepLoaded<Texture>("ui/ray_burst_2.png", synchronous: false);
    public static Texture RayBurstExp = Assets.KeepLoaded<Texture>("ui/Exp_Rays_copy.png", synchronous: false);
    public static Texture CenterAura = Assets.KeepLoaded<Texture>("ui/centre_aura.png", synchronous: false);
    public static Texture LightRays = Assets.KeepLoaded<Texture>("ui/light_rays.png", synchronous: false);
    public static Texture HorizontalRays = Assets.KeepLoaded<Texture>("ui/horizontal_ray.png", synchronous: false);
    public static Texture ArrowIcon = Assets.KeepLoaded<Texture>("ui/arrow.png", synchronous: false);
    public static Texture ArrowRightIcon = Assets.KeepLoaded<Texture>("ui/arrowright.png", synchronous: false);

    public static Texture TwentyParts = Assets.KeepLoaded<Texture>("ui/twenty_parts.png", synchronous: false);
    public static Texture HundredParts = Assets.KeepLoaded<Texture>("ui/hundredparts.png", synchronous: false);


    public static AudioAsset MoneyPopSound = Assets.KeepLoaded<AudioAsset>("Sounds/SFX_UI_Click_Buy.wav", synchronous: false);
    public static AudioAsset StolenBrainrotSound = Assets.KeepLoaded<AudioAsset>("Sounds/SFX_UI_Fillup_Star_1.wav", synchronous: false);
    public static AudioAsset StolenBrainrotSoundSuccess = Assets.KeepLoaded<AudioAsset>("Sounds/SFX_UI_Countdown_SciFi_Begin.wav", synchronous: false);
    public static AudioAsset ClickSoundCute = Assets.KeepLoaded<AudioAsset>("Sounds/SFX_UI_Click_Generic_Cute.wav", synchronous: false);

    public static Texture Circle = Assets.KeepLoaded<Texture>("ui/circle.png", synchronous: false);
    public static Texture Lasers = Assets.KeepLoaded<Texture>("ui/lasers.png", synchronous: false);
    public static Texture InventoryBacking = Assets.KeepLoaded<Texture>("$AO/new/in_game/leaderboard/border_backing_generic.png", synchronous: false);
    public static Texture ItemSlotBacking = Assets.KeepLoaded<Texture>("$AO/new/in_game/inventory/inventory_v2/inventory_bubble.png", synchronous: false);
    public static Texture BoogieBombTexture = Assets.KeepLoaded<Texture>("ui/boogie_bomb.png", synchronous: false);
    public static Texture BearTrapTexture = Assets.KeepLoaded<Texture>("ui/beartrap.png", synchronous: false);

    // BRAINROT SOUNDS
    public static AudioAsset MergeSound_LuckyBlock = Assets.KeepLoaded<AudioAsset>("Sounds/luckyblock.wav", synchronous: false);
    public static AudioAsset MergeSound_Ballerina = Assets.KeepLoaded<AudioAsset>("Sounds/Balerina.wav", synchronous: false);
    public static AudioAsset MergeSound_BombardinoCrocodilo = Assets.KeepLoaded<AudioAsset>("Sounds/BombardinoCrocodilo.wav", synchronous: false);
    public static AudioAsset MergeSound_BombombiniGusini = Assets.KeepLoaded<AudioAsset>("Sounds/BombombiniGusini.wav", synchronous: false);
    public static AudioAsset MergeSound_BurbaloniLuliloli = Assets.KeepLoaded<AudioAsset>("Sounds/BurbaloniLuliloli.wav", synchronous: false);
    public static AudioAsset MergeSound_CappuccinoAssassino = Assets.KeepLoaded<AudioAsset>("Sounds/CapuccinoAssassino.wav", synchronous: false);
    public static AudioAsset MergeSound_ChimpanziniBananini = Assets.KeepLoaded<AudioAsset>("Sounds/ChimpanziniBananini.wav", synchronous: false);
    public static AudioAsset MergeSound_CocofantoElephanto = Assets.KeepLoaded<AudioAsset>("Sounds/CocofantoElephanto.wav", synchronous: false);
    public static AudioAsset MergeSoundBoneca = Assets.KeepLoaded<AudioAsset>("Sounds/BonecaAmbalabu.wav", synchronous: false);
    public static AudioAsset MergeSoundTralalero = Assets.KeepLoaded<AudioAsset>("Sounds/Tralalero.wav", synchronous: false);
    public static AudioAsset MergeSoundTungTung = Assets.KeepLoaded<AudioAsset>("Sounds/TungTung.wav", synchronous: false);
    public static AudioAsset MergeSoundGlorbo = Assets.KeepLoaded<AudioAsset>("Sounds/Glorbo.wav", synchronous: false);
    public static AudioAsset MergeSoundIlCacto = Assets.KeepLoaded<AudioAsset>("Sounds/IlCacto.wav", synchronous: false);
    public static AudioAsset MergeSoundZibra = Assets.KeepLoaded<AudioAsset>("Sounds/Zibra.wav", synchronous: false);
    public static AudioAsset MergeSoundTigrallini = Assets.KeepLoaded<AudioAsset>("Sounds/Tigrallini.wav", synchronous: false);
    public static AudioAsset MergeSoundLirilli = Assets.KeepLoaded<AudioAsset>("Sounds/Lirilli.wav", synchronous: false);
    public static AudioAsset MergeSoundTatatata = Assets.KeepLoaded<AudioAsset>("Sounds/Tatatata.wav", synchronous: false);


    public static AudioAsset MergeSoundBallerinaBrrBrr = Assets.KeepLoaded<AudioAsset>("Sounds/ballerina_brrbrr.wav", synchronous: false);
    public static AudioAsset MergeSoundBallerinaBrrBrrLirili = Assets.KeepLoaded<AudioAsset>("Sounds/ballerina_brrbrr_lirili.wav", synchronous: false);
    public static AudioAsset MergeSoundBonecaBurbaloni = Assets.KeepLoaded<AudioAsset>("Sounds/boneca_burbaloni.wav", synchronous: false);
    public static AudioAsset MergeSoundBonecaBurbaloniTataSahur = Assets.KeepLoaded<AudioAsset>("Sounds/boneca_burbaloni_tatatasahur.wav", synchronous: false);
    public static AudioAsset MergeSoundBrrBrrPatapim = Assets.KeepLoaded<AudioAsset>("Sounds/brrbrr_patapim.wav", synchronous: false);
    public static AudioAsset MergeSoundBrrBrrTungTung = Assets.KeepLoaded<AudioAsset>("Sounds/brrbrrtungtung.wav", synchronous: false);
    public static AudioAsset MergeSoundBrrBrrTungTungZibraZubra = Assets.KeepLoaded<AudioAsset>("Sounds/brrbrrtungtungzibrazubra.wav", synchronous: false);
    public static AudioAsset MergeSoundBurbaloniLulilolliBananini = Assets.KeepLoaded<AudioAsset>("Sounds/burbaloni_lulilolli_bananini.wav", synchronous: false);
    public static AudioAsset MergeSoundBurbaloniLulilolliBananiniFruttoDrillo = Assets.KeepLoaded<AudioAsset>("Sounds/burbaloni_lulilolli_bananini_fruttodrillo.wav", synchronous: false);
    public static AudioAsset MergeSoundCappuccinoBallerina = Assets.KeepLoaded<AudioAsset>("Sounds/cappuccino_ballerina.wav", synchronous: false);
    public static AudioAsset MergeSoundCappuccinoBallerinaCacto = Assets.KeepLoaded<AudioAsset>("Sounds/cappuccino_ballerina_cacto.wav", synchronous: false);
    public static AudioAsset MergeSoundChimpanziniBananiniCocosino = Assets.KeepLoaded<AudioAsset>("Sounds/chimpanzini_bananini_cocosino.wav", synchronous: false);
    public static AudioAsset MergeSoundChimpanziniBananiniWatermellini = Assets.KeepLoaded<AudioAsset>("Sounds/chimpanzini_bananini_watermellini.wav", synchronous: false);
    public static AudioAsset MergeSoundCocofantoElefantoCocosino = Assets.KeepLoaded<AudioAsset>("Sounds/cocofanto_elefanto_cocosino.wav", synchronous: false);
    public static AudioAsset MergeSoundCocofantoElefantoCocosinoCacto = Assets.KeepLoaded<AudioAsset>("Sounds/cocofanto_elefanto_cocosino_cacto.wav", synchronous: false);
    public static AudioAsset MergeSoundCocosinoRhino = Assets.KeepLoaded<AudioAsset>("Sounds/cocosino_rhino.wav", synchronous: false);
    public static AudioAsset MergeSoundCocosinoRhinoLirili = Assets.KeepLoaded<AudioAsset>("Sounds/cocosino_rhino_lirili.wav", synchronous: false);
    public static AudioAsset MergeSoundCocosinoRhinoLiriliFruttoDrillo = Assets.KeepLoaded<AudioAsset>("Sounds/cocosino_rhino_lirili_fruttodrillo.wav", synchronous: false);
    public static AudioAsset MergeSoundGlorboFruttodrilloLulilolli = Assets.KeepLoaded<AudioAsset>("Sounds/glorbo_fruttodrillo_lulilolli.wav", synchronous: false);
    public static AudioAsset MergeSoundGlorboFruttodrilloLulilolliCappucina = Assets.KeepLoaded<AudioAsset>("Sounds/glorbo_fruttodrillo_lulilolli_cappucina.wav", synchronous: false);
    public static AudioAsset MergeSoundIlCactoHipopotamoBananini = Assets.KeepLoaded<AudioAsset>("Sounds/il_cacto_hipopotamo_bananini.wav", synchronous: false);
    public static AudioAsset MergeSoundIlCactoHipopotamoBananiniTungtung = Assets.KeepLoaded<AudioAsset>("Sounds/il_cacto_hipopotamo_bananini_tungtung.wav", synchronous: false);
    public static AudioAsset MergeSoundKarkerkarKurkur = Assets.KeepLoaded<AudioAsset>("Sounds/karkerkar_kurkur.wav", synchronous: false);
    public static AudioAsset MergeSoundKarkerkarKurkurAmbalabu = Assets.KeepLoaded<AudioAsset>("Sounds/karkerkar_kurkur_ambalabu.wav", synchronous: false);
    public static AudioAsset MergeSoundKarkerkarKurkurAmbalabuTungtung = Assets.KeepLoaded<AudioAsset>("Sounds/karkerkar_kurkur_ambalabu_tungtung.wav", synchronous: false);
    public static AudioAsset MergeSoundLiriliLarilaAmbalabu = Assets.KeepLoaded<AudioAsset>("Sounds/lirili_larila_ambalabu.wav", synchronous: false);
    public static AudioAsset MergeSoundLiriliLarilaAmbalabuTataSahur = Assets.KeepLoaded<AudioAsset>("Sounds/lirili_larila_ambalabu_tata_sahur.wav", synchronous: false);
    public static AudioAsset MergeSoundTaTaSahurLarila = Assets.KeepLoaded<AudioAsset>("Sounds/ta_ta_sahur_larila.wav", synchronous: false);
    public static AudioAsset MergeSoundTaTaSahurLarilaZibraZubra = Assets.KeepLoaded<AudioAsset>("Sounds/ta_ta_sahur_larila_zibra_zubra.wav", synchronous: false);
    public static AudioAsset MergeSoundTrigrrulliniWatermelliniCocofanto = Assets.KeepLoaded<AudioAsset>("Sounds/trigrrullini_watermellini_cocofanto.wav", synchronous: false);
    public static AudioAsset MergeSoundTrigrrulliniWatermelliniCocofantoCappucino = Assets.KeepLoaded<AudioAsset>("Sounds/trigrrullini_watermellini_cocofanto_cappucino.wav", synchronous: false);
    public static AudioAsset MergeSoundTungTungBananini = Assets.KeepLoaded<AudioAsset>("Sounds/tung_tung_bananini.wav", synchronous: false);
    public static AudioAsset MergeSoundTungTungBananiniWatermeloni = Assets.KeepLoaded<AudioAsset>("Sounds/tung_tung_bananini_watermeloni.wav", synchronous: false);
    public static AudioAsset MergeSoundZibraZurbraZibraliniPatapim = Assets.KeepLoaded<AudioAsset>("Sounds/zibra_zurbra_zibralini_patapim.wav", synchronous: false);
    public static AudioAsset MergeSoundZibraZurbraZibraliniPatapimAssassino = Assets.KeepLoaded<AudioAsset>("Sounds/zibra_zurbra_zibralini_patapim_assassino.wav", synchronous: false);


    public static AudioAsset UIEvolutionSound = Assets.KeepLoaded<AudioAsset>("Sounds/ui_new_evolution.wav", synchronous: false);
    public static AudioAsset UIBuySound = Assets.KeepLoaded<AudioAsset>("Sounds/ui_buy_item_upgrade.wav", synchronous: false);

    // New merge sounds
    public static AudioAsset MergeSound_Avocadini_Antilopini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadini_Antilopini.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadini_Antilopini_Trulala = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadini_Antilopini_Trulala.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadini_Antilopini_Trulala_Toasterino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadini_Antilopini_Trulala_Toasterino.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadini_Guffo = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadini_Guffo.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadini_Guffo_BrriBri = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadini_Guffo_BrriBri.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadini_Guffo_BrriBri_Tititi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadini_Guffo_BrriBri_Tititi.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadorilla = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadorilla.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadorilla_Pipi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadorilla_Pipi.wav", synchronous: false);
    public static AudioAsset MergeSound_Avocadorilla_Pipi_Tric_Trac = Assets.KeepLoaded<AudioAsset>("Sounds/New/Avocadorilla_Pipi_Tric_Trac.wav", synchronous: false);
    public static AudioAsset MergeSound_Bambini_Crostini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bambini_Crostini.wav", synchronous: false);
    public static AudioAsset MergeSound_Bambini_Crostini_Salamini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bambini_Crostini_Salamini.wav", synchronous: false);
    public static AudioAsset MergeSound_Bambini_Crostini_Salamini_Tititi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bambini_Crostini_Salamini_Tititi.wav", synchronous: false);
    public static AudioAsset MergeSound_Bananita_Dolphinita = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bananita_Dolphinita.wav", synchronous: false);
    public static AudioAsset MergeSound_Bananita_Dolphinita_Camelo = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bananita_Dolphinita_Camelo.wav", synchronous: false);
    public static AudioAsset MergeSound_Bananita_Dolphinita_Camelo_Trulicina = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bananita_Dolphinita_Camelo_Trulicina.wav", synchronous: false);
    public static AudioAsset MergeSound_Blueberrini_Octopussini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Blueberrini_Octopussini.wav", synchronous: false);
    public static AudioAsset MergeSound_Blueberrini_Octopussini_Kurkur = Assets.KeepLoaded<AudioAsset>("Sounds/New/Blueberrini_Octopussini_Kurkur.wav", synchronous: false);
    public static AudioAsset MergeSound_Blueberrini_Octopussini_Kurkur_Orangutini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Blueberrini_Octopussini_Kurkur_Orangutini.wav", synchronous: false);
    public static AudioAsset MergeSound_Bobrito_Bandito = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bobrito_Bandito.wav", synchronous: false);
    public static AudioAsset MergeSound_Bobrito_Bandito_Tob_Tobi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bobrito_Bandito_Tob_Tobi.wav", synchronous: false);
    public static AudioAsset MergeSound_Bobrito_Bandito_Tob_Tobi_Crabracada = Assets.KeepLoaded<AudioAsset>("Sounds/New/Bobrito_Bandito_Tob_Tobi_Crabracada.wav", synchronous: false);
    public static AudioAsset MergeSound_Brri_Brri_Bicus = Assets.KeepLoaded<AudioAsset>("Sounds/New/Brri_Brri_Bicus.wav", synchronous: false);
    public static AudioAsset MergeSound_Brri_Brri_Bicus_Tigrullini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Brri_Brri_Bicus_Tigrullini.wav", synchronous: false);
    public static AudioAsset MergeSound_Brri_Brri_Bicus_Tigrullini_TungTung = Assets.KeepLoaded<AudioAsset>("Sounds/New/Brri_Brri_Bicus_Tigrullini_TungTung.wav", synchronous: false);
    public static AudioAsset MergeSound_Cavallo_Virtuoso = Assets.KeepLoaded<AudioAsset>("Sounds/New/Cavallo_Virtuoso.wav", synchronous: false);
    public static AudioAsset MergeSound_Cavallo_Virtuoso_Crabracadabra = Assets.KeepLoaded<AudioAsset>("Sounds/New/Cavallo_Virtuoso_Crabracadabra.wav", synchronous: false);
    public static AudioAsset MergeSound_Cavallo_Virtuoso_Crabracadabra_Tetete = Assets.KeepLoaded<AudioAsset>("Sounds/New/Cavallo_Virtuoso_Crabracadabra_Tetete.wav", synchronous: false);
    public static AudioAsset MergeSound_Chef_Crabracadabra = Assets.KeepLoaded<AudioAsset>("Sounds/New/Chef_Crabracadabra.wav", synchronous: false);
    public static AudioAsset MergeSound_Chef_Crabracadabra_Lirili = Assets.KeepLoaded<AudioAsset>("Sounds/New/Chef_Crabracadabra_Lirili.wav", synchronous: false);
    public static AudioAsset MergeSound_Chef_Crabracadabra_Lirili_Gangster = Assets.KeepLoaded<AudioAsset>("Sounds/New/Chef_Crabracadabra_Lirili_Gangster.wav", synchronous: false);
    public static AudioAsset MergeSound_Cocossini_Mama = Assets.KeepLoaded<AudioAsset>("Sounds/New/Cocossini_Mama.wav", synchronous: false);
    public static AudioAsset MergeSound_Cocossini_Mama_Strawberelli = Assets.KeepLoaded<AudioAsset>("Sounds/New/Cocossini_Mama_Strawberelli.wav", synchronous: false);
    public static AudioAsset MergeSound_Cocossini_Mama_Strawberelli_Assassino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Cocossini_Mama_Strawberelli_Assassino.wav", synchronous: false);
    public static AudioAsset MergeSound_Dragon_Cannelloni = Assets.KeepLoaded<AudioAsset>("Sounds/New/Dragon_Cannelloni.wav", synchronous: false);
    public static AudioAsset MergeSound_Dragon_Cannelloni_Tracotucotulu = Assets.KeepLoaded<AudioAsset>("Sounds/New/Dragon_Cannelloni_Tracotucotulu.wav", synchronous: false);
    public static AudioAsset MergeSound_Dragon_Cannelloni_Tracotucotulu_Potato = Assets.KeepLoaded<AudioAsset>("Sounds/New/Dragon_Cannelloni_Tracotucotulu_Potato.wav", synchronous: false);
    public static AudioAsset MergeSound_Fluriflura = Assets.KeepLoaded<AudioAsset>("Sounds/New/Fluriflura.wav", synchronous: false);
    public static AudioAsset MergeSound_Fluriflura_Tim = Assets.KeepLoaded<AudioAsset>("Sounds/New/Fluriflura_Tim.wav", synchronous: false);
    public static AudioAsset MergeSound_Fluriflura_Tim_Dindin = Assets.KeepLoaded<AudioAsset>("Sounds/New/Fluriflura_Tim_Dindin.wav", synchronous: false);
    public static AudioAsset MergeSound_Frigo_Camelo = Assets.KeepLoaded<AudioAsset>("Sounds/New/Frigo_Camelo.wav", synchronous: false);
    public static AudioAsset MergeSound_Frigo_Camelo_Tetetete = Assets.KeepLoaded<AudioAsset>("Sounds/New/Frigo_Camelo_Tetetete.wav", synchronous: false);
    public static AudioAsset MergeSound_Frigo_Camelo_Tetetete_Rhino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Frigo_Camelo_Tetetete_Rhino.wav", synchronous: false);
    public static AudioAsset MergeSound_Ganganzelli_Trulala = Assets.KeepLoaded<AudioAsset>("Sounds/New/Ganganzelli_Trulala.wav", synchronous: false);
    public static AudioAsset MergeSound_Ganganzelli_Trulala_Sigma = Assets.KeepLoaded<AudioAsset>("Sounds/New/Ganganzelli_Trulala_Sigma.wav", synchronous: false);
    public static AudioAsset MergeSound_Ganganzelli_Trulala_Sigma_Tric_Trac = Assets.KeepLoaded<AudioAsset>("Sounds/New/Ganganzelli_Trulala_Sigma_Tric_Trac.wav", synchronous: false);
    public static AudioAsset MergeSound_Gangster_Footera = Assets.KeepLoaded<AudioAsset>("Sounds/New/Gangster_Footera.wav", synchronous: false);
    public static AudioAsset MergeSound_Gangster_Footera_TungTung = Assets.KeepLoaded<AudioAsset>("Sounds/New/Gangster_Footera_TungTung.wav", synchronous: false);
    public static AudioAsset MergeSound_Gangster_Footera_TungTung_Kurkur = Assets.KeepLoaded<AudioAsset>("Sounds/New/Gangster_Footera_TungTung_Kurkur.wav", synchronous: false);
    public static AudioAsset MergeSound_Gorillo_Watermellondrillo = Assets.KeepLoaded<AudioAsset>("Sounds/New/Gorillo_Watermellondrillo.wav", synchronous: false);
    public static AudioAsset MergeSound_Gorillo_Watermellondrillo_Lemonchello = Assets.KeepLoaded<AudioAsset>("Sounds/New/Gorillo_Watermellondrillo_Lemonchello.wav", synchronous: false);
    public static AudioAsset MergeSound_Gorillo_Watermellondrillo_Lemonchello_Blueberrini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Gorillo_Watermellondrillo_Lemonchello_Blueberrini.wav", synchronous: false);
    public static AudioAsset MergeSound_Lerulerulerule = Assets.KeepLoaded<AudioAsset>("Sounds/New/Lerulerulerule.wav", synchronous: false);
    public static AudioAsset MergeSound_Lerulerulerule_BrrBrr = Assets.KeepLoaded<AudioAsset>("Sounds/New/Lerulerulerule_BrrBrr.wav", synchronous: false);
    public static AudioAsset MergeSound_Lerulerulerule_BrrBrr_Troppi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Lerulerulerule_BrrBrr_Troppi.wav", synchronous: false);
    public static AudioAsset MergeSound_Lionel_Cactuseli = Assets.KeepLoaded<AudioAsset>("Sounds/New/Lionel_Cactuseli.wav", synchronous: false);
    public static AudioAsset MergeSound_Lionel_Cactuseli_PipiWatermelon = Assets.KeepLoaded<AudioAsset>("Sounds/New/Lionel_Cactuseli_PipiWatermelon.wav", synchronous: false);
    public static AudioAsset MergeSound_Lionel_Cactuseli_PipiWatermelon_DiFero = Assets.KeepLoaded<AudioAsset>("Sounds/New/Lionel_Cactuseli_PipiWatermelon_DiFero.wav", synchronous: false);
    public static AudioAsset MergeSound_Orangutini_Ananasini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Orangutini_Ananasini.wav", synchronous: false);
    public static AudioAsset MergeSound_Orangutini_Ananasini_Cocosino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Orangutini_Ananasini_Cocosino.wav", synchronous: false);
    public static AudioAsset MergeSound_Orangutini_Ananasini_Cocosino_Flamingelli = Assets.KeepLoaded<AudioAsset>("Sounds/New/Orangutini_Ananasini_Cocosino_Flamingelli.wav", synchronous: false);
    public static AudioAsset MergeSound_Pandaccini_Bananini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pandaccini_Bananini.wav", synchronous: false);
    public static AudioAsset MergeSound_Pandaccini_Bananini_Potato = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pandaccini_Bananini_Potato.wav", synchronous: false);
    public static AudioAsset MergeSound_Pandaccini_Bananini_Potato_Quivioli = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pandaccini_Bananini_Potato_Quivioli.wav", synchronous: false);
    public static AudioAsset MergeSound_Penguino_Cocosino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Penguino_Cocosino.wav", synchronous: false);
    public static AudioAsset MergeSound_Penguino_Cocosino_Sigma = Assets.KeepLoaded<AudioAsset>("Sounds/New/Penguino_Cocosino_Sigma.wav", synchronous: false);
    public static AudioAsset MergeSound_Penguino_Cocosino_Sigma_Salamino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Penguino_Cocosino_Sigma_Salamino.wav", synchronous: false);
    public static AudioAsset MergeSound_PerochHello_Lemonchello = Assets.KeepLoaded<AudioAsset>("Sounds/New/Perochello_Lemonchello.wav", synchronous: false);
    public static AudioAsset MergeSound_Perochello_Lemonchello_Troppi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Perochello_Lemonchello_Troppi.wav", synchronous: false);
    public static AudioAsset MergeSound_Perochello_Lemonchello_Troppi_Ambalabu = Assets.KeepLoaded<AudioAsset>("Sounds/New/Perochello_Lemonchello_Troppi_Ambalabu.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Avocado = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Avocado.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Avocado_Toasterino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Avocado_Toasterino.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Avocado_Toasterino_Bombardino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Avocado_Toasterino_Bombardino.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Corni = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Corni.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Corni_Tim = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Corni_Tim.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Corni_Tim_Tracotucotulu = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Corni_Tim_Tracotucotulu.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Kiwi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Kiwi.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Kiwi_Tetetete = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Kiwi_Tetetete.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Kiwi_Tetetete_Tob_Tobi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Kiwi_Tetetete_Tob_Tobi.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Potato = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Potato.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Potato_Pipi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Potato_Pipi.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Potato_Pipi_Tatata = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Potato_Pipi_Tatata.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Watermelon = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Watermelon.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Watermelon_Quivioli = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Watermelon_Quivioli.wav", synchronous: false);
    public static AudioAsset MergeSound_Pipi_Watermelon_Quivioli_Trulicina = Assets.KeepLoaded<AudioAsset>("Sounds/New/Pipi_Watermelon_Quivioli_Trulicina.wav", synchronous: false);
    public static AudioAsset MergeSound_Quivioli_Ameleonni = Assets.KeepLoaded<AudioAsset>("Sounds/New/Quivioli_Ameleonni.wav", synchronous: false);
    public static AudioAsset MergeSound_Quivioli_Ameleonni_Bombardino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Quivioli_Ameleonni_Bombardino.wav", synchronous: false);
    public static AudioAsset MergeSound_Quivioli_Ameleonni_Bombardino_Octopussini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Quivioli_Ameleonni_Bombardino_Octopussini.wav", synchronous: false);
    public static AudioAsset MergeSound_Rhino_Toasterino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Rhino_Toasterino.wav", synchronous: false);
    public static AudioAsset MergeSound_Rhino_Toasterino_Gangster = Assets.KeepLoaded<AudioAsset>("Sounds/New/Rhino_Toasterino_Gangster.wav", synchronous: false);
    public static AudioAsset MergeSound_Rhino_Toasterino_Gangster_Troppi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Rhino_Toasterino_Gangster_Troppi.wav", synchronous: false);
    public static AudioAsset MergeSound_Salamino_Penguino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Salamino_Penguino.wav", synchronous: false);
    public static AudioAsset MergeSound_Salamino_Penguino_Sigma = Assets.KeepLoaded<AudioAsset>("Sounds/New/Salamino_Penguino_Sigma.wav", synchronous: false);
    public static AudioAsset MergeSound_Salamino_Penguino_Sigma_TricTrac = Assets.KeepLoaded<AudioAsset>("Sounds/New/Salamino_Penguino_Sigma_TricTrac.wav", synchronous: false);
    public static AudioAsset MergeSound_Schleemerini_Monsterini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Schleemerini_Monsterini.wav", synchronous: false);
    public static AudioAsset MergeSound_Schleemerini_Monsterini_Gangster = Assets.KeepLoaded<AudioAsset>("Sounds/New/Schleemerini_Monsterini_Gangster.wav", synchronous: false);
    public static AudioAsset MergeSound_Schleemerini_Monsterini_Gangster_Chef = Assets.KeepLoaded<AudioAsset>("Sounds/New/Schleemerini_Monsterini_Gangster_Chef.wav", synchronous: false);
    public static AudioAsset MergeSound_Sigma_Boy = Assets.KeepLoaded<AudioAsset>("Sounds/New/Sigma_Boy.wav", synchronous: false);
    public static AudioAsset MergeSound_Sigma_Boy_Strawberini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Sigma_Boy_Strawberini.wav", synchronous: false);
    public static AudioAsset MergeSound_Sigma_Boy_Strawberini_Craberini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Sigma_Boy_Strawberini_Craberini.wav", synchronous: false);
    public static AudioAsset MergeSound_Sixtee_Seven = Assets.KeepLoaded<AudioAsset>("Sounds/New/Sixtee_Seven.wav", synchronous: false);
    public static AudioAsset MergeSound_Sixtee_Seven_Cannelloni = Assets.KeepLoaded<AudioAsset>("Sounds/New/Sixtee_Seven_Cannelloni.wav", synchronous: false);
    public static AudioAsset MergeSound_Sixtee_Seven_Cannelloni_Strawberini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Sixtee_Seven_Cannelloni_Strawberini.wav", synchronous: false);
    public static AudioAsset MergeSound_Smurfcat = Assets.KeepLoaded<AudioAsset>("Sounds/New/Smurfcat.wav", synchronous: false);
    public static AudioAsset MergeSound_Smurfcat_Bandito = Assets.KeepLoaded<AudioAsset>("Sounds/New/Smurfcat_Bandito.wav", synchronous: false);
    public static AudioAsset MergeSound_Smurfcat_Bandito_assassino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Smurfcat_Bandito_assassino.wav", synchronous: false);
    public static AudioAsset MergeSound_Spijuniro_Golubiro = Assets.KeepLoaded<AudioAsset>("Sounds/New/Spijuniro_Golubiro.wav", synchronous: false);
    public static AudioAsset MergeSound_Spijuniro_Golubiro_Pipi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Spijuniro_Golubiro_Pipi.wav", synchronous: false);
    public static AudioAsset MergeSound_Spijuniro_Golubiro_Pip_Ambalabu = Assets.KeepLoaded<AudioAsset>("Sounds/New/Spijuniro_Golubiro_Pip_Ambalabu.wav", synchronous: false);
    public static AudioAsset MergeSound_SSundee_Ambalabu = Assets.KeepLoaded<AudioAsset>("Sounds/New/SSundee_Ambalabu.wav", synchronous: false);
    public static AudioAsset MergeSound_SSundee_Ambalabu_Troppa = Assets.KeepLoaded<AudioAsset>("Sounds/New/SSundee_Ambalabu_Troppa.wav", synchronous: false);
    public static AudioAsset MergeSound_SSundee_Ambalabu_Troppa_Sushi = Assets.KeepLoaded<AudioAsset>("Sounds/New/SSundee_Ambalabu_Troppa_Sushi.wav", synchronous: false);
    public static AudioAsset MergeSound_Strawberelli_Flamingelli = Assets.KeepLoaded<AudioAsset>("Sounds/New/Strawberelli_Flamingelli.wav", synchronous: false);
    public static AudioAsset MergeSound_Strawberelli_Flamingelli_Orangutini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Strawberelli_Flamingelli_Orangutini.wav", synchronous: false);
    public static AudioAsset MergeSound_Strawberelli_Flamingelli_Orangutini_Kiwi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Strawberelli_Flamingelli_Orangutini_Kiwi.wav", synchronous: false);
    public static AudioAsset MergeSound_Strawberry_Elephant = Assets.KeepLoaded<AudioAsset>("Sounds/New/Strawberry_Elephant.wav", synchronous: false);
    public static AudioAsset MergeSound_Strawberry_Elephant_Ambalabu = Assets.KeepLoaded<AudioAsset>("Sounds/New/Strawberry_Elephant_Ambalabu.wav", synchronous: false);
    public static AudioAsset MergeSound_Strawberry_Elephant_Ambalabu_Golubiro = Assets.KeepLoaded<AudioAsset>("Sounds/New/Strawberry_Elephant_Ambalabu_Golubiro.wav", synchronous: false);
    public static AudioAsset MergeSound_Svinina_Bombardino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Svinina_Bombardino.wav", synchronous: false);
    public static AudioAsset MergeSound_Svinina_Bombardino_Tititi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Svinina_Bombardino_Tititi.wav", synchronous: false);
    public static AudioAsset MergeSound_Svinina_Bombardino_Tititi_Tetetete = Assets.KeepLoaded<AudioAsset>("Sounds/New/Svinina_Bombardino_Tititi_Tetetete.wav", synchronous: false);
    public static AudioAsset MergeSound_Talpa_di_Fero = Assets.KeepLoaded<AudioAsset>("Sounds/New/Talpa_di_Fero.wav", synchronous: false);
    public static AudioAsset MergeSound_Talpa_di_Fero_Corni = Assets.KeepLoaded<AudioAsset>("Sounds/New/Talpa_di_Fero_Corni.wav", synchronous: false);
    public static AudioAsset MergeSound_Talpa_di_Fero_Corni_Watermeloni = Assets.KeepLoaded<AudioAsset>("Sounds/New/Talpa_di_Fero_Corni_Watermeloni.wav", synchronous: false);
    public static AudioAsset MergeSound_Te_Te_Te_Te_Te_Sahur = Assets.KeepLoaded<AudioAsset>("Sounds/New/Te_Te_Te_Te_Te_Sahur.wav", synchronous: false);
    public static AudioAsset MergeSound_Tetetete_Sahur_Titititi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tetetete_Sahur_Titititi.wav", synchronous: false);
    public static AudioAsset MergeSound_Tetetete_Sahur_Titititi_Dindin = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tetetete_Sahur_Titititi_Dindin.wav", synchronous: false);
    public static AudioAsset MergeSound_Ti_Ti_Ti_Sahur = Assets.KeepLoaded<AudioAsset>("Sounds/New/Ti_Ti_Ti_Sahur.wav", synchronous: false);
    public static AudioAsset MergeSound_Ti_Ti_Ti_Sahur_Luliloli = Assets.KeepLoaded<AudioAsset>("Sounds/New/Ti_Ti_Ti_Sahur_Luliloli.wav", synchronous: false);
    public static AudioAsset MergeSound_Ti_Ti_Ti_Sahur_Luliloli_Tungtung = Assets.KeepLoaded<AudioAsset>("Sounds/New/Ti_Ti_Ti_Sahur_Luliloli_Tungtung.wav", synchronous: false);
    public static AudioAsset MergeSound_Tim_Cheese = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tim_Cheese.wav", synchronous: false);
    public static AudioAsset MergeSound_Tim_Cheese_Tatatata = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tim_Cheese_Tatatata.wav", synchronous: false);
    public static AudioAsset MergeSound_Tim_Cheese_Tatatata_Gangster = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tim_Cheese_Tatatata_Gangster.wav", synchronous: false);
    public static AudioAsset MergeSound_Tob_Tobi_Tobi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tob_Tobi_Tobi.wav", synchronous: false);
    public static AudioAsset MergeSound_Tob_Tobi_Tobi_Tric_Trac = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tob_Tobi_Tobi_Tric_Trac.wav", synchronous: false);
    public static AudioAsset MergeSound_Tob_Tobi_Tobi_Tric_Trac_Avocadini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tob_Tobi_Tobi_Tric_Trac_Avocadini.wav", synchronous: false);
    public static AudioAsset MergeSound_Tracotucotulu_Delapeladustuz = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tracotucotulu_Delapeladustuz.wav", synchronous: false);
    public static AudioAsset MergeSound_Tracotucotulu_Delapeladustuz_Troppi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tracotucotulu_Delapeladustuz_Troppi.wav", synchronous: false);
    public static AudioAsset MergeSound_Tracotucotulu_Delapeladustuz_Troppi_Trulicina = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tracotucotulu_Delapeladustuz_Troppi_Trulicina.wav", synchronous: false);
    public static AudioAsset MergeSound_Tric_Trac_Baraboom = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tric_Trac_Baraboom.wav", synchronous: false);
    public static AudioAsset MergeSound_Tric_Trac_Baraboom_Gangsta = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tric_Trac_Baraboom_Gangsta.wav", synchronous: false);
    public static AudioAsset MergeSound_Tric_Trac_Baraboom_Gangsta_Watermelon = Assets.KeepLoaded<AudioAsset>("Sounds/New/Tric_Trac_Baraboom_Gangsta_Watermelon.wav", synchronous: false);
    public static AudioAsset MergeSound_Trippi_Troppi = Assets.KeepLoaded<AudioAsset>("Sounds/New/Trippi_Troppi.wav", synchronous: false);
    public static AudioAsset MergeSound_Trippi_Troppi_Smurf = Assets.KeepLoaded<AudioAsset>("Sounds/New/Trippi_Troppi_Smurf.wav", synchronous: false);
    public static AudioAsset MergeSound_Trippi_Troppi_Smurf_Lemonchello = Assets.KeepLoaded<AudioAsset>("Sounds/New/Trippi_Troppi_Smurf_Lemonchello.wav", synchronous: false);
    public static AudioAsset MergeSound_Trulimero_Trulicina = Assets.KeepLoaded<AudioAsset>("Sounds/New/Trulimero_Trulicina.wav", synchronous: false);
    public static AudioAsset MergeSound_Trulimero_Trulicina_Crostini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Trulimero_Trulicina_Crostini.wav", synchronous: false);
    public static AudioAsset MergeSound_Trulimero_Trulicina_Crostini_Orangutini = Assets.KeepLoaded<AudioAsset>("Sounds/New/Trulimero_Trulicina_Crostini_Orangutini.wav", synchronous: false);
    public static AudioAsset MergeSound_Udin_Din_Din_Dun = Assets.KeepLoaded<AudioAsset>("Sounds/New/Udin_Din_Din_Dun.wav", synchronous: false);
    public static AudioAsset MergeSound_Udin_Din_Din_Dun_Tric_Trac = Assets.KeepLoaded<AudioAsset>("Sounds/New/Udin_Din_Din_Dun_Tric_Trac.wav", synchronous: false);
    public static AudioAsset MergeSound_Udin_Din_Din_Dun_Tric_Trac_Assassino = Assets.KeepLoaded<AudioAsset>("Sounds/New/Udin_Din_Din_Dun_Tric_Trac_Assassino.wav", synchronous: false);

    public static AudioAsset MergeSound_BrrBrrBaby = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/LosBrrBrr.wav", synchronous: false);
    public static AudioAsset MergeSound_ChimpanziniBaby = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/LosChimpancinitos.wav", synchronous: false);
    public static AudioAsset MergeSound_TralaleritoBaby = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/LasTralaleritas.wav", synchronous: false);
    public static AudioAsset MergeSound_TrippiTroppiBaby = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/Lostripitopis.wav", synchronous: false);
    public static AudioAsset StormSound = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/summon_storm.wav", synchronous: false);

    public static AudioAsset MergeSound_NightsMonster = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/NightMonster.wav", synchronous: false);
    public static AudioAsset MergeSound_NightsMonsterCannelloni = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/NightMonsterCannelloni.wav", synchronous: false);
    public static AudioAsset MergeSound_NightsMonsterCannelloniTroppi = Assets.KeepLoaded<AudioAsset>("Sounds/Event_Woods/NightMonsterCannelloniTroppi.wav", synchronous: false);



    public static AudioAsset JumpscareShadeSound = Assets.KeepLoaded<AudioAsset>("Sounds/Admin/jumpscare_shade.wav", synchronous: false);





    public static Texture JailTexture = Assets.KeepLoaded<Texture>("Textures/jail.png", synchronous: false);
    public static Texture Leave = Assets.KeepLoaded<Texture>("Textures/leaf.png", synchronous: true);
    public static Texture LeafTileable = Assets.KeepLoaded<Texture>("Textures/LeafTileable.png", synchronous: true);
    public static Texture SparkIcon = Assets.KeepLoaded<Texture>("ui/spark.png", synchronous: false);
    public static Texture StealIcon = Assets.KeepLoaded<Texture>("ui/warmode.png", synchronous: false);
    public static Texture ChampDeFraise = Assets.KeepLoaded<Texture>("ui/champdefraise.png", synchronous: true);
    public static Texture DesertBG = Assets.KeepLoaded<Texture>("ui/desert.png", synchronous: true);
    public static Texture AdminPass = Assets.KeepLoaded<Texture>("ui/adminpass.png", synchronous: true);
    public static Texture ShopIcon = Assets.KeepLoaded<Texture>("ui/shopicon.png", synchronous: true);
    public static Texture BookIcon = Assets.KeepLoaded<Texture>("ui/indexbook.png", synchronous: true);
    public static Texture RebirthIcon = Assets.KeepLoaded<Texture>("ui/rebirth.png", synchronous: true);
    public static Texture BackplateShop = Assets.KeepLoaded<Texture>("ui/shop_backplate_3_ocean.png", synchronous: true);

    public static Texture BoogieBomb = Assets.KeepLoaded<Texture>("ui/boogie_bomb.png", synchronous: false);
    public static Texture Cape = Assets.KeepLoaded<Texture>("ui/cape.png", synchronous: false);
    public static Texture Clone = Assets.KeepLoaded<Texture>("ui/clone.png", synchronous: false);
    public static Texture FartPistol = Assets.KeepLoaded<Texture>("ui/fart_pistol.png", synchronous: false);
    public static Texture SwapPlayer = Assets.KeepLoaded<Texture>("ui/swap_player.png", synchronous: false);
    public static Texture Turret = Assets.KeepLoaded<Texture>("ui/turret.png", synchronous: false);
    public static Texture DreamSword = Assets.KeepLoaded<Texture>("ui/dream_sword.png", synchronous: false);
    public static Texture Taser = Assets.KeepLoaded<Texture>("ui/taser.png", synchronous: false);
    public static Texture BatTungTung = Assets.KeepLoaded<Texture>("ui/bat_tungtung.png", synchronous: false);
    public static Texture BatCappucinoAssassino = Assets.KeepLoaded<Texture>("ui/bat_assassino.png", synchronous: false);
    // new memes
    public static Texture AvocadiniAntilopiniTrulalaToasterino = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadini_Antilopini_Trulala_toasterino.png", synchronous: false);
    public static Texture AvocadiniAntilopiniTrulala = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadini_Antilopini_Trulala.png", synchronous: false);
    public static Texture AvocadiniAntilopini = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadini_Antilopini.png", synchronous: false);
    public static Texture AvocadiniGuffoBrriBriTititi = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadini_Guffo_BrriBri_Tititi.png", synchronous: false);
    public static Texture AvocadiniGuffoBrriBri = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadini_Guffo_BrriBri.png", synchronous: false);
    public static Texture AvocadiniGuffo = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadini_Guffo.png", synchronous: false);
    public static Texture AvocadorillaPipiTricTrac = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadorilla_pipi_Tric_Trac.png", synchronous: false);
    public static Texture AvocadorillaPipi = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadorilla_pipi.png", synchronous: false);
    public static Texture Avocadorilla = Assets.KeepLoaded<Texture>("Textures/memes/new/Avocadorilla.png", synchronous: false);
    public static Texture BambiniCrostiniSalaminiTititi = Assets.KeepLoaded<Texture>("Textures/memes/new/Bambini_Crostini_Salamini_Tititi.png", synchronous: false);
    public static Texture BambiniCrostiniSalamini = Assets.KeepLoaded<Texture>("Textures/memes/new/Bambini_Crostini_Salamini.png", synchronous: false);
    public static Texture BambiniCrostini = Assets.KeepLoaded<Texture>("Textures/memes/new/Bambini_Crostini.png", synchronous: false);
    public static Texture BananitaDolphinitaCameloTrulicina = Assets.KeepLoaded<Texture>("Textures/memes/new/Bananita_Dolphinita_Camelo_trulicina.png", synchronous: false);
    public static Texture BananitaDolphinitaCamelo = Assets.KeepLoaded<Texture>("Textures/memes/new/Bananita_Dolphinita_Camelo.png", synchronous: false);
    public static Texture BananitaDolphinita = Assets.KeepLoaded<Texture>("Textures/memes/new/Bananita_Dolphinita.png", synchronous: false);
    public static Texture BlueberriniOctopussiniKurkurOrangutini = Assets.KeepLoaded<Texture>("Textures/memes/new/Blueberrini_Octopussini_kurkur_Orangutini.png", synchronous: false);
    public static Texture BlueberriniOctopussiniKurkur = Assets.KeepLoaded<Texture>("Textures/memes/new/Blueberrini_Octopussini_kurkur.png", synchronous: false);
    public static Texture BlueberriniOctopussini = Assets.KeepLoaded<Texture>("Textures/memes/new/Blueberrini_Octopussini.png", synchronous: false);
    public static Texture BobritoBanditoTobTobiCrabracada = Assets.KeepLoaded<Texture>("Textures/memes/new/bobrito_bandito_tob_tobi_Crabracada.png", synchronous: false);
    public static Texture BobritoBanditoTobTobi = Assets.KeepLoaded<Texture>("Textures/memes/new/bobrito_bandito_tob_tobi.png", synchronous: false);
    public static Texture BobritoBandito = Assets.KeepLoaded<Texture>("Textures/memes/new/bobrito_bandito.png", synchronous: false);
    public static Texture BrriBrriBicusTigrulliniTungTung = Assets.KeepLoaded<Texture>("Textures/memes/new/Brri_Brri_Bicus_Tigrullini_TungTung.png", synchronous: false);
    public static Texture BrriBrriBicusTigrullini = Assets.KeepLoaded<Texture>("Textures/memes/new/Brri_Brri_Bicus_Tigrullini.png", synchronous: false);
    public static Texture BrriBrriBicus = Assets.KeepLoaded<Texture>("Textures/memes/new/Brri_Brri_Bicus.png", synchronous: false);
    public static Texture CavalloVirtuosoCrabracadabraTetete = Assets.KeepLoaded<Texture>("Textures/memes/new/Cavallo_Virtuoso_Crabracadabra_Tetete.png", synchronous: false);
    public static Texture CavalloVirtuosoCrabracadabra = Assets.KeepLoaded<Texture>("Textures/memes/new/Cavallo_Virtuoso_Crabracadabra.png", synchronous: false);
    public static Texture CavalloVirtuoso = Assets.KeepLoaded<Texture>("Textures/memes/new/Cavallo_Virtuoso.png", synchronous: false);
    public static Texture ChefCrabracadabraLiriliGangster = Assets.KeepLoaded<Texture>("Textures/memes/new/Chef_Crabracadabra_Lirili_Gangster.png", synchronous: false);
    public static Texture ChefCrabracadabraLirili = Assets.KeepLoaded<Texture>("Textures/memes/new/Chef_Crabracadabra_Lirili.png", synchronous: false);
    public static Texture ChefCrabracadabra = Assets.KeepLoaded<Texture>("Textures/memes/new/Chef_Crabracadabra.png", synchronous: false);
    public static Texture CocossiniMamaStrawberelliAssassino = Assets.KeepLoaded<Texture>("Textures/memes/new/Cocossini_mama_strawberelli_assassino.png", synchronous: false);
    public static Texture CocossiniMamaStrawberelli = Assets.KeepLoaded<Texture>("Textures/memes/new/Cocossini_mama_strawberelli.png", synchronous: false);
    public static Texture CocossiniMama = Assets.KeepLoaded<Texture>("Textures/memes/new/Cocossini_Mama.png", synchronous: false);
    public static Texture DragonCannelloniTracotucotuluPotato = Assets.KeepLoaded<Texture>("Textures/memes/new/Dragon_Cannelloni_Tracotucotulu_Potato.png", synchronous: false);
    public static Texture DragonCannelloniTracotucotulu = Assets.KeepLoaded<Texture>("Textures/memes/new/Dragon_Cannelloni_Tracotucotulu.png", synchronous: false);
    public static Texture DragonCannelloni = Assets.KeepLoaded<Texture>("Textures/memes/new/Dragon_Cannelloni.png", synchronous: false);
    public static Texture FlurifluraTimDindin = Assets.KeepLoaded<Texture>("Textures/memes/new/Fluriflura_Tim_dindin.png", synchronous: false);
    public static Texture FlurifluraTim = Assets.KeepLoaded<Texture>("Textures/memes/new/Fluriflura_Tim.png", synchronous: false);
    public static Texture Fluriflura = Assets.KeepLoaded<Texture>("Textures/memes/new/Fluriflura.png", synchronous: false);
    public static Texture FrigoCameloTeteteteRhino = Assets.KeepLoaded<Texture>("Textures/memes/new/frigoCamelo_tetetete_rhino.png", synchronous: false);
    public static Texture FrigoCameloTetetete = Assets.KeepLoaded<Texture>("Textures/memes/new/frigoCamelo_tetetete.png", synchronous: false);
    public static Texture FrigoCamelo = Assets.KeepLoaded<Texture>("Textures/memes/new/frigoCamelo.png", synchronous: false);
    public static Texture GanganzelliTrulalaSigmaTricTrac = Assets.KeepLoaded<Texture>("Textures/memes/new/Ganganzelli_Trulala_sigma_Tric_Trac.png", synchronous: false);
    public static Texture GanganzelliTrulalaSigma = Assets.KeepLoaded<Texture>("Textures/memes/new/Ganganzelli_Trulala_sigma.png", synchronous: false);
    public static Texture GanganzelliTrulala = Assets.KeepLoaded<Texture>("Textures/memes/new/Ganganzelli_Trulala.png", synchronous: false);
    public static Texture GangsterFooteraTungTungKurkur = Assets.KeepLoaded<Texture>("Textures/memes/new/GangsterFootera_TungTung_kurkur.png", synchronous: false);
    public static Texture GangsterFooteraTungTung = Assets.KeepLoaded<Texture>("Textures/memes/new/GangsterFootera_TungTung.png", synchronous: false);
    public static Texture GangsterFootera = Assets.KeepLoaded<Texture>("Textures/memes/new/GangsterFootera.png", synchronous: false);
    public static Texture GorilloLemonchelloBlueberrini = Assets.KeepLoaded<Texture>("Textures/memes/new/gorillo_lemonchello_blueberrini.png", synchronous: false);
    public static Texture GorilloLemonchello = Assets.KeepLoaded<Texture>("Textures/memes/new/gorillo_lemonchello.png", synchronous: false);
    public static Texture GorilloWatermellondrillo = Assets.KeepLoaded<Texture>("Textures/memes/new/Gorillo_Watermellondrillo.png", synchronous: false);
    public static Texture Lerulerulerule = Assets.KeepLoaded<Texture>("Textures/memes/new/Lerulerulerule.png", synchronous: false);
    public static Texture LeruleruleruleBrrBrr = Assets.KeepLoaded<Texture>("Textures/memes/new/LeruleruleruleBrrBrr.png", synchronous: false);
    public static Texture LeruleruleruleBrrBrrTroppi = Assets.KeepLoaded<Texture>("Textures/memes/new/LeruleruleruleBrrBrrTroppi.png", synchronous: false);
    public static Texture LionelCactuseliPipiWatermelonDiFero = Assets.KeepLoaded<Texture>("Textures/memes/new/Lionel_Cactuseli_PipiWatermelon_DiFero.png", synchronous: false);
    public static Texture LionelCactuseliPipiWatermelon = Assets.KeepLoaded<Texture>("Textures/memes/new/Lionel_Cactuseli_PipiWatermelon.png", synchronous: false);
    public static Texture LionelCactuseli = Assets.KeepLoaded<Texture>("Textures/memes/new/Lionel_Cactuseli.png", synchronous: false);
    public static Texture Nightsmonster = Assets.KeepLoaded<Texture>("Textures/memes/new/nightsmonster.png", synchronous: false);
    public static Texture OrangutiniAnanasiniCocosinoFlamingelli = Assets.KeepLoaded<Texture>("Textures/memes/new/Orangutini_Ananasini_Cocosino_Flamingelli.png", synchronous: false);
    public static Texture OrangutiniAnanasiniCocosino = Assets.KeepLoaded<Texture>("Textures/memes/new/Orangutini_Ananasini_Cocosino.png", synchronous: false);
    public static Texture OrangutiniAnanasini = Assets.KeepLoaded<Texture>("Textures/memes/new/Orangutini_Ananasini.png", synchronous: false);
    public static Texture PandacciniBananiniPotatoQuivioli = Assets.KeepLoaded<Texture>("Textures/memes/new/Pandaccini_Bananini_potato_Quivioli.png", synchronous: false);
    public static Texture PandacciniBananiniPotato = Assets.KeepLoaded<Texture>("Textures/memes/new/Pandaccini_Bananini_potato.png", synchronous: false);
    public static Texture PandacciniBananini = Assets.KeepLoaded<Texture>("Textures/memes/new/Pandaccini_Bananini.png", synchronous: false);
    public static Texture PenguinoCocosinoSigmaSalamino = Assets.KeepLoaded<Texture>("Textures/memes/new/Penguino_Cocosino_sigma_salamino.png", synchronous: false);
    public static Texture PenguinoCocosinoSigma = Assets.KeepLoaded<Texture>("Textures/memes/new/Penguino_Cocosino_sigma.png", synchronous: false);
    public static Texture PenguinoCocosino = Assets.KeepLoaded<Texture>("Textures/memes/new/Penguino_Cocosino.png", synchronous: false);
    public static Texture PerochelloLemonchelloTroppiAmbalabu = Assets.KeepLoaded<Texture>("Textures/memes/new/Perochello_Lemonchello_Troppi_ambalabu.png", synchronous: false);
    public static Texture PerochelloLemonchelloTroppi = Assets.KeepLoaded<Texture>("Textures/memes/new/Perochello_Lemonchello_Troppi.png", synchronous: false);
    public static Texture PerochelloLemonchello = Assets.KeepLoaded<Texture>("Textures/memes/new/Perochello_Lemonchello.png", synchronous: false);
    public static Texture PipiAvocadoToasterinoBombardino = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Avocado_Toasterino_bombardino.png", synchronous: false);
    public static Texture PipiAvocadoToasterino = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Avocado_Toasterino.png", synchronous: false);
    public static Texture PipiAvocado = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Avocado.png", synchronous: false);
    public static Texture PipiCorniTimTracotucotulu = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Corni_Tim_Tracotucotulu.png", synchronous: false);
    public static Texture PipiCorniTim = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Corni_Tim.png", synchronous: false);
    public static Texture PipiCorni = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Corni.png", synchronous: false);
    public static Texture PipiKiwiTeteteteTobTobi = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Kiwi_Tetetete_Tob_Tobi.png", synchronous: false);
    public static Texture PipiKiwiTetetete = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Kiwi_Tetetete.png", synchronous: false);
    public static Texture PipiKiwi = Assets.KeepLoaded<Texture>("Textures/memes/new/pipi_kiwi.png", synchronous: false);
    public static Texture PipiPotatoPipiTatata = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Potato_Pipi_Tatata.png", synchronous: false);
    public static Texture PipiPotatoPipi = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Potato_Pipi.png", synchronous: false);
    public static Texture PipiPotato = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Potato.png", synchronous: false);
    public static Texture PipiWatermelonQuivioliTrulicina = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Watermelon_Quivioli_Trulicina.png", synchronous: false);
    public static Texture PipiWatermelonQuivioli = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Watermelon_Quivioli.png", synchronous: false);
    public static Texture PipiWatermelon = Assets.KeepLoaded<Texture>("Textures/memes/new/Pipi_Watermelon.png", synchronous: false);
    public static Texture QuivioliAmeleonniBombardinoOctopussini = Assets.KeepLoaded<Texture>("Textures/memes/new/Quivioli_Ameleonni_Bombardino_Octopussini.png", synchronous: false);
    public static Texture QuivioliAmeleonniBombardino = Assets.KeepLoaded<Texture>("Textures/memes/new/Quivioli_Ameleonni_Bombardino.png", synchronous: false);
    public static Texture QuivioliAmeleonni = Assets.KeepLoaded<Texture>("Textures/memes/new/Quivioli_Ameleonni.png", synchronous: false);
    public static Texture RacooniJandeliniBallerinaAntilopini = Assets.KeepLoaded<Texture>("Textures/memes/new/Racooni_Jandelini_Ballerina_Antilopini.png", synchronous: false);
    public static Texture RacooniJandeliniBallerina = Assets.KeepLoaded<Texture>("Textures/memes/new/Racooni_Jandelini_Ballerina.png", synchronous: false);
    public static Texture RacooniJandelini = Assets.KeepLoaded<Texture>("Textures/memes/new/Racooni_Jandelini.png", synchronous: false);
    public static Texture RhinoToasterinoGangsterTroppi = Assets.KeepLoaded<Texture>("Textures/memes/new/Rhino_Toasterino_Gangster_Troppi.png", synchronous: false);
    public static Texture RhinoToasterinoGangster = Assets.KeepLoaded<Texture>("Textures/memes/new/Rhino_Toasterino_Gangster.png", synchronous: false);
    public static Texture RhinoToasterino = Assets.KeepLoaded<Texture>("Textures/memes/new/rhino_toasterino.png", synchronous: false);
    public static Texture SalaminoPenguinoSigmaTricTrac = Assets.KeepLoaded<Texture>("Textures/memes/new/Salamino_Penguino_sigma_TricTrac.png", synchronous: false);
    public static Texture SalaminoPenguinoSigma = Assets.KeepLoaded<Texture>("Textures/memes/new/Salamino_Penguino_sigma.png", synchronous: false);
    public static Texture SalaminoPenguino = Assets.KeepLoaded<Texture>("Textures/memes/new/Salamino_Penguino.png", synchronous: false);
    public static Texture SchleemeriniMonsteriniGangsterChef = Assets.KeepLoaded<Texture>("Textures/memes/new/Schleemerini_monsterini_Gangster_Chef.png", synchronous: false);
    public static Texture SchleemeriniMonsteriniGangster = Assets.KeepLoaded<Texture>("Textures/memes/new/Schleemerini_monsterini_Gangster.png", synchronous: false);
    public static Texture SchleemeriniMonsterini = Assets.KeepLoaded<Texture>("Textures/memes/new/schleemerini_monsterini.png", synchronous: false);
    public static Texture SigmaBoyStrawberiniCraberini = Assets.KeepLoaded<Texture>("Textures/memes/new/sigma_boy_strawberini_craberini.png", synchronous: false);
    public static Texture SigmaBoyStrawberini = Assets.KeepLoaded<Texture>("Textures/memes/new/sigma_boy_strawberini.png", synchronous: false);
    public static Texture SigmaBoy = Assets.KeepLoaded<Texture>("Textures/memes/new/sigma_boy.png", synchronous: false);
    public static Texture SixteeSevenCannelloniStrawberini = Assets.KeepLoaded<Texture>("Textures/memes/new/Sixtee_Seven_Cannelloni_Strawberini.png", synchronous: false);
    public static Texture SixteeSevenCannelloni = Assets.KeepLoaded<Texture>("Textures/memes/new/Sixtee_Seven_Cannelloni.png", synchronous: false);
    public static Texture SixteeSeven = Assets.KeepLoaded<Texture>("Textures/memes/new/Sixtee_Seven.png", synchronous: false);
    public static Texture SmurfcatBanditoAssassino = Assets.KeepLoaded<Texture>("Textures/memes/new/Smurfcat_Bandito_assassino.png", synchronous: false);
    public static Texture SmurfcatBandito = Assets.KeepLoaded<Texture>("Textures/memes/new/Smurfcat_Bandito.png", synchronous: false);
    public static Texture Smurfcat = Assets.KeepLoaded<Texture>("Textures/memes/new/smurfcat.png", synchronous: false);
    public static Texture SpijuniroGolubiroPipAmbalabu = Assets.KeepLoaded<Texture>("Textures/memes/new/Spijuniro_Golubiro_Pip_Ambalabu.png", synchronous: false);
    public static Texture SpijuniroGolubiroPipi = Assets.KeepLoaded<Texture>("Textures/memes/new/Spijuniro_Golubiro_Pipi.png", synchronous: false);
    public static Texture SpijuniroGolubiro = Assets.KeepLoaded<Texture>("Textures/memes/new/Spijuniro_Golubiro.png", synchronous: false);
    public static Texture StrawberelliFlamingelliOrangutiniKiwi = Assets.KeepLoaded<Texture>("Textures/memes/new/Strawberelli_Flamingelli_Orangutini_Kiwi.png", synchronous: false);
    public static Texture StrawberelliFlamingelliOrangutini = Assets.KeepLoaded<Texture>("Textures/memes/new/Strawberelli_Flamingelli_Orangutini.png", synchronous: false);
    public static Texture StrawberelliFlamingelli = Assets.KeepLoaded<Texture>("Textures/memes/new/Strawberelli_Flamingelli.png", synchronous: false);
    public static Texture SvininaBombardinoTititiTetetete = Assets.KeepLoaded<Texture>("Textures/memes/new/Svinina_Bombardino_Tititi_Tetetete.png", synchronous: false);
    public static Texture SvininaBombardinoTititi = Assets.KeepLoaded<Texture>("Textures/memes/new/Svinina_Bombardino_Tititi.png", synchronous: false);
    public static Texture SvininaBombardino = Assets.KeepLoaded<Texture>("Textures/memes/new/Svinina_Bombardino.png", synchronous: false);
    public static Texture TalpaDiFeroCorniWatermeloni = Assets.KeepLoaded<Texture>("Textures/memes/new/talpa_di_fero_corni_watermeloni.png", synchronous: false);
    public static Texture TalpaDiFeroCorni = Assets.KeepLoaded<Texture>("Textures/memes/new/talpa_di_fero_corni.png", synchronous: false);
    public static Texture TalpaDiFero = Assets.KeepLoaded<Texture>("Textures/memes/new/Talpa_di_Fero.png", synchronous: false);
    public static Texture TeteteteSahurTitititiDindin = Assets.KeepLoaded<Texture>("Textures/memes/new/tetetete_sahur_titititi_dindin.png", synchronous: false);
    public static Texture TeteteteSahurTitititi = Assets.KeepLoaded<Texture>("Textures/memes/new/tetetete_sahur_titititi.png", synchronous: false);
    public static Texture TeTeTeTeTeSahur = Assets.KeepLoaded<Texture>("Textures/memes/new/TeTeTeTeTeSahur.png", synchronous: false);
    public static Texture TiTiTiSahurLuliloliTungtung = Assets.KeepLoaded<Texture>("Textures/memes/new/Ti_ti_ti_Sahur_Luliloli_tungtung.png", synchronous: false);
    public static Texture TiTiTiSahurLuliloli = Assets.KeepLoaded<Texture>("Textures/memes/new/Ti_ti_ti_Sahur_Luliloli.png", synchronous: false);
    public static Texture TiTiTiSahur = Assets.KeepLoaded<Texture>("Textures/memes/new/Ti_Ti_Ti_Sahur.png", synchronous: false);
    public static Texture TimCheeseTatatataGangster = Assets.KeepLoaded<Texture>("Textures/memes/new/TimCheese_tatatata_gangster.png", synchronous: false);
    public static Texture TimCheeseTatatata = Assets.KeepLoaded<Texture>("Textures/memes/new/TimCheese_tatatata.png", synchronous: false);
    public static Texture TimCheese = Assets.KeepLoaded<Texture>("Textures/memes/new/TimCheese.png", synchronous: false);
    public static Texture TobTobiTobiTricTracAvocadini = Assets.KeepLoaded<Texture>("Textures/memes/new/tob_tobi_tobi_tric_trac_avocadini.png", synchronous: false);
    public static Texture TobTobiTobiTricTrac = Assets.KeepLoaded<Texture>("Textures/memes/new/tob_tobi_tobi_tric_trac.png", synchronous: false);
    public static Texture TobTobiTobi = Assets.KeepLoaded<Texture>("Textures/memes/new/Tob_Tobi_Tobi.png", synchronous: false);
    public static Texture TracotucotuluDelapeladustuz = Assets.KeepLoaded<Texture>("Textures/memes/new/Tracotucotulu Delapeladustuz.png", synchronous: false);
    public static Texture TracotucotuluDelapeladustuzTroppiTrulicina = Assets.KeepLoaded<Texture>("Textures/memes/new/Tracotucotulu_Delapeladustuz_Troppi_Trulicina.png", synchronous: false);
    public static Texture TracotucotuluDelapeladustuzTroppi = Assets.KeepLoaded<Texture>("Textures/memes/new/Tracotucotulu_Delapeladustuz_Troppi.png", synchronous: false);
    public static Texture TricTracBaraboomGangstaWatermelon = Assets.KeepLoaded<Texture>("Textures/memes/new/Tric_Trac_Baraboom_Gangsta_Watermelon.png", synchronous: false);
    public static Texture TricTracBaraboomGangsta = Assets.KeepLoaded<Texture>("Textures/memes/new/Tric_Trac_Baraboom_Gangsta.png", synchronous: false);
    public static Texture TricTracBaraboom = Assets.KeepLoaded<Texture>("Textures/memes/new/Tric_Trac_Baraboom.png", synchronous: false);
    public static Texture TrippiTroppi = Assets.KeepLoaded<Texture>("Textures/memes/new/trippiTroppi.png", synchronous: false);
    public static Texture TrippiTroppiSmurfLemonchello = Assets.KeepLoaded<Texture>("Textures/memes/new/TrippiTroppiSmurf_Lemonchello.png", synchronous: false);
    public static Texture TrippiTroppiSmurf = Assets.KeepLoaded<Texture>("Textures/memes/new/TrippiTroppiSmurf.png", synchronous: false);
    public static Texture TrulimeroTrulicinaCrostiniOrangutini = Assets.KeepLoaded<Texture>("Textures/memes/new/Trulimero_Trulicina_Crostini_Orangutini.png", synchronous: false);
    public static Texture TrulimeroTrulicinaCrostini = Assets.KeepLoaded<Texture>("Textures/memes/new/Trulimero_Trulicina_Crostini.png", synchronous: false);
    public static Texture TrulimeroTrulicina = Assets.KeepLoaded<Texture>("Textures/memes/new/trulimero_trulicina.png", synchronous: false);


    public static Texture SSundeeAmbalabu = Assets.KeepLoaded<Texture>("Textures/memes/SSundeeAmbalabu.png", synchronous: false);
    public static Texture SSundeeAmbalabuTroppa = Assets.KeepLoaded<Texture>("Textures/memes/SSundeeAmbalabu_Troppa.png", synchronous: false);
    public static Texture SSundeeAmbalabuTroppaSushi = Assets.KeepLoaded<Texture>("Textures/memes/SSundeeAmbalabu_Troppa_sushi.png", synchronous: false);
}
