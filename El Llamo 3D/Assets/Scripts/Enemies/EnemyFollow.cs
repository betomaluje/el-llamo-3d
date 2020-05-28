using SWNetwork;

public class EnemyFollow : LocalEnemyFollow
{
    private NetworkID networkID;

    protected override void Start()
    {
        base.Start();
        networkID = GetComponent<NetworkID>();
    }

    protected override void Update()
    {
        agent.enabled = networkID.IsMine;

        if (networkID.IsMine)
        {
            base.Update();
        }
    }
}
