using AO;

public partial class PurchasingManager : System<PurchasingManager>
{
    // Product IDs for area purchases (these should match your Sparks store configuration)
    public const string BuyArea1ProductId = "68f0194cc6d4a2cdff2ceeb4";
    public const string BuyArea2ProductId = "68f019553764dc586ee4df83";
    public const string BuyArea3ProductId = "68f0195c3764dc586ee4df86";

    public override void Awake()
    {
        if (Network.IsServer)
        {
            Purchasing.SetPurchaseHandler(SparksPurchaseHandler);
        }
    }

    private bool SparksPurchaseHandler(Player _player, string productId)
    {
        var player = (MyPlayer)_player;
        
        if (player == null || !player.Entity.Alive())
            return false;
        
        Log.Info($"[PurchasingManager] Player {player.Name} purchasing: {productId}");
        
        // Handle area unlocks
        if (productId == BuyArea1ProductId)
        {
            player.UnlockArea(1);
            player.CallClient_ShowMessage("Area 1 unlocked!", new RPCOptions() { Target = player });
            return true;
        }
        else if (productId == BuyArea2ProductId)
        {
            player.UnlockArea(2);
            player.CallClient_ShowMessage("Area 2 unlocked!", new RPCOptions() { Target = player });
            return true;
        }
        else if (productId == BuyArea3ProductId)
        {
            player.UnlockArea(3);
            player.CallClient_ShowMessage("Area 3 unlocked!", new RPCOptions() { Target = player });
            return true;
        }
        
        return false;
    }
}