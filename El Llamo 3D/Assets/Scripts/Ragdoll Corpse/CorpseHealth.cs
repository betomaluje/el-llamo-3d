using SWNetwork;
using UnityEngine;

public class CorpseHealth : MonoBehaviour
{
    [SerializeField] private GameObject bloodDamagePrefab;
    [SerializeField] private GameObject explodingCorpsePrefab;
    [SerializeField] private int maxHealth = 30;

    private int currentHealth;

    #region Network

    private NetworkID networkID;
    private RemoteEventAgent remoteEventAgent;

    const string HEALTH = "Hp";
    const string KILLED_EVENT = "killed";

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        networkID = GetComponent<NetworkID>();
        remoteEventAgent = GetComponent<RemoteEventAgent>();

        currentHealth = maxHealth;
    }

    public void PerformDamage(int damage, Vector3 impactPosition)
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
     * Called from the SyncPropertyAgent on the editor
     */
    public void OnHPChanged(SWNetworkMessage msg)
    {
        currentHealth = (int)msg.PopFloat();

        if (currentHealth != maxHealth)
        {
            Vector3 impactPosition = msg.PopVector3();

            Instantiate(bloodDamagePrefab, impactPosition, transform.rotation);
        }

        if (networkID.IsMine && currentHealth <= 0)
        {
            // invoke the "killed" remote event when hp is 0. 
            remoteEventAgent.Invoke(KILLED_EVENT);
        }
    }

    public void Die()
    {
        // destroy corpse
        transform.parent = null;

        ExplodeCorpse explodeScript = Instantiate(explodingCorpsePrefab, transform.position, transform.rotation).GetComponent<ExplodeCorpse>();
        Destroy(gameObject);
        networkID.Destroy();
    }
}

