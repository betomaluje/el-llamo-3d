using SWNetwork;

public class MouseLook : LocalMouseLook
{
    private NetworkID networkID;

    protected override void Start()
    {
        base.Start();
        networkID = GetComponent<NetworkID>();
    }

    protected override void Update()
    {
        if (networkID.IsMine)
        {
            base.Update();
        }
    }

    protected override void LateUpdate()
    {
        if (networkID.IsMine)
        {
            base.LateUpdate();
        }
    }
}
