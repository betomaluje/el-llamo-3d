using SWNetwork;
using UnityEngine;

public class CorpseHealth : MonoBehaviour
{
    [SerializeField] private GameObject bloodDamagePrefab;
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    #region Network

    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;
    private RemoteEventAgent remoteEventAgent;

    const string HEALTH = "Hp";
    const string KILLED_EVENT = "killed";

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        networkID = GetComponent<NetworkID>();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();
        remoteEventAgent = GetComponent<RemoteEventAgent>();
    }

    public void PerformDamage(int damage)
    {
        currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH).GetIntValue();

        currentHealth -= damage;

        // if hp is lower than 0, set it to 0.
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Apply damage and modify the "hp" SyncProperty.
        syncPropertyAgent?.Modify(HEALTH, currentHealth);
    }

    public void OnHpChanged()
    {
        // Update the hpSlider when player hp changes
        currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH).GetIntValue();

        Debug.Log("remote hp: " + currentHealth);

        if (currentHealth != maxHealth)
        {
            Instantiate(bloodDamagePrefab, transform.position, transform.rotation);
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
        Debug.Log("corpse dead");

        transform.parent = null;
        Destroy(gameObject);
    }
}

