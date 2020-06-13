using SWNetwork;
using UnityEngine;

public class CorpseHealth : LocalCorpseHealth
{
    private NetworkID networkID;
    private RemoteEventAgent remoteEventAgent;

    const string HEALTH = "Hp";
    const string KILLED_EVENT = "killed";

    protected override void Start()
    {
        networkID = GetComponent<NetworkID>();
        remoteEventAgent = GetComponent<RemoteEventAgent>();
        base.Start();
    }

    public override void PerformDamage(int damage, Vector3 impactPosition)
    {
        currentHealth -= damage;

        // if hp is lower than 0, set it to 0.
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Apply damage and modify the "hp" SyncProperty.
        SWNetworkMessage msg = new SWNetworkMessage();
        // current health
        msg.Push((float)currentHealth);
        // blood position
        msg.Push(impactPosition);
        remoteEventAgent.Invoke(HEALTH, msg);
    }

    /**
     * Called from the RemoteEventAgent on the editor
     */
    public void OnHPChanged(SWNetworkMessage msg)
    {
        currentHealth = (int)msg.PopFloat();

        if (currentHealth != maxHealth)
        {
            CalculatePercentage();

            Vector3 impactPosition = msg.PopVector3();

            AddDamageSFX(impactPosition);
        }

        if (networkID.IsMine && currentHealth <= 0)
        {
            // invoke the "killed" remote event when hp is 0. 
            remoteEventAgent.Invoke(KILLED_EVENT);
        }
    }

    public override void Die()
    {
        base.Die();
        networkID.Destroy();
    }

    protected override void SpawnShieldCorpse()
    {
        Debug.Log("Network Shield Corpse");
        NetworkClient.Instance.LastSpawner.SpawnForNonPlayer((int)NonPlayerIndexes.Ragdoll_Corpse, transform.position, transform.rotation);
    }
}

