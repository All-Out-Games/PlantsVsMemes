using AO;

public partial class PlacedCollectible : Component
{
    public SyncVar<string> CollectibleType = new SyncVar<string>("peashooter");
    
    public int Width = 1;
    public int Height = 1;
    public float MoneyPerSecond = 1.0f;
    public MyPlayer OwnerPlayer;
    public Plot OwnerPlot;
    public int GridX = 0;
    public int GridY = 0;

    float lastIncomeTime = 0;
    float incomeInterval = 2.0f;
    Spine_Animator spineAnimator;

    public override void Awake()
    {
        spineAnimator = Entity.GetComponent<Spine_Animator>();

        if (Network.IsServer)
        {
            lastIncomeTime = Time.TimeSinceStartup;
        }
    }

    public override void Update()
    {
        if (Network.IsServer)
        {
            ServerUpdate();
        }
    }

    void ServerUpdate()
    {
        // if (!OwnerPlayer.Alive() || !OwnerPlot.Alive())
        // {
        //     Entity.Destroy();
        //     return;
        // }

        // if (Time.TimeSinceStartup - lastIncomeTime >= incomeInterval)
        // {
        //     int moneyAmount = (int)(MoneyPerSecond * incomeInterval);
        //     // OwnerPlayer.AddMoney(moneyAmount);
        //     CallClient_ShowMoneyPopup(moneyAmount);
        //     lastIncomeTime = Time.TimeSinceStartup;
        // }
    }

    [ClientRpc]
    public void ShowMoneyPopup(int amount)
    {
        // Log.Info($"+{amount} money from {CollectibleType.Value}");
    }
}

