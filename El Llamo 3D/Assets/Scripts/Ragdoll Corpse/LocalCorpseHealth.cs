using UnityEngine;

public class LocalCorpseHealth : MonoBehaviour
{
    [Header("FX")]
    [SerializeField] private GameObject bloodDamagePrefab;
    [SerializeField] private GameObject dieFXPrefab;
    [SerializeField] private GameObject explodingCorpsePrefab;
    [SerializeField] private GameObject corpsePrefab;
    [SerializeField] private int maxHealth = 30;
    [Range(0, 100)]
    [SerializeField] private int probabilityOfExplode = 30;

    protected int currentHealth;

    protected float probOfExploding;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        probOfExploding = probabilityOfExplode / 100f;
    }

    public virtual void PerformDamage(int damage, Vector3 impactPosition)
    {
        currentHealth -= damage;

        // if hp is lower than 0, set it to 0.
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            if (currentHealth != maxHealth)
            {
                MakeBlood(impactPosition);
            }
        }
    }

    protected void MakeBlood(Vector3 impactPosition)
    {
        Instantiate(bloodDamagePrefab, impactPosition, transform.rotation);
    }

    protected int GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual void Die()
    {
        // destroy corpse
        transform.parent = null;
        Destroy(gameObject);

        float randomExploding = Random.value;

        if (randomExploding <= probOfExploding)
        {
            ExplodeCorpse explodeScript = Instantiate(explodingCorpsePrefab, transform.position, transform.rotation).GetComponent<ExplodeCorpse>();
        }
        else
        {
            SpawnShieldCorpse();
        }

        Instantiate(dieFXPrefab, transform.position, Quaternion.identity);
    }

    protected virtual void SpawnShieldCorpse()
    {
        Debug.Log("Shield Corpse");
        Instantiate(corpsePrefab, transform.position, transform.rotation);
    }
}

