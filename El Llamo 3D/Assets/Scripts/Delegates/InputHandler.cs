using SWNetwork;

public class InputHandler : LocalInputHandler
{
    #region Network

    private NetworkID networkID;

    #endregion

    private void Start()
    {
        networkID = GetComponent<NetworkID>();
    }

    protected override void Update()
    {
        // only the owner should be able to sync the aim and shooting of the mouse
        if (!networkID.IsMine)
        {
            return;
        }

        base.Update();
    }
}
