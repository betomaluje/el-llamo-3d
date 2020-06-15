using SWNetwork;

public class PlayerNetworkSetup : PlayerSetup
{
    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;

    protected override void Awake()
    {
        networkID = GetComponent<NetworkID>();

        if (networkID.IsMine)
        {
            crosshair = FindObjectOfType<Crosshair>();
            syncPropertyAgent = GetComponent<SyncPropertyAgent>();

            SetupPlayer();
        }
    }
    protected override void SetupPlayer()
    {
        LocalPlayerGrab playerGrab = GetComponentInChildren<LocalPlayerGrab>(true);
        playerGrab.SetupPlayer();

        crosshair.SetupPlayer(GetComponent<LocalInputHandler>());

        string name = NetworkClient.Instance.PlayerId.Split('-')[1];
        syncPropertyAgent?.Modify(PlayerName.NICKNAME_PROPERTY, name);
    }
}
