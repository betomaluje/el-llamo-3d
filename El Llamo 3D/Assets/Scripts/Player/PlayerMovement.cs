using SWNetwork;

public class PlayerMovement : LocalPlayerMovement
{
    private NetworkID networkID;

    protected override void Update()
    {
        if (networkID.IsMine)
        {
            base.Update();
        }
    }

    protected override void Start()
    {
        base.Start();
        networkID = GetComponent<NetworkID>();
    }

}
