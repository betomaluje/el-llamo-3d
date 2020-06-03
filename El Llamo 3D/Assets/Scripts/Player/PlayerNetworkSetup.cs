using SWNetwork;

public class PlayerNetworkSetup : PlayerSetup
{
    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;

    protected override void Awake()
    {
        base.Awake();
        networkID = GetComponent<NetworkID>();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();

        if (networkID.IsMine)
        {
            SetupPlayer();
        }
    }
    protected override void SetupPlayer()
    {
        base.SetupPlayer();
        string name = NetworkClient.Instance.PlayerId.Split('-')[1];
        syncPropertyAgent?.Modify(PlayerName.NICKNAME_PROPERTY, name);
    }
}
