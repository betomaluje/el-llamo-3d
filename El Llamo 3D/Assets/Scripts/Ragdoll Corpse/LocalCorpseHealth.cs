using UnityEngine;

public class LocalCorpseHealth : MonoBehaviour
{
    [SerializeField] private GameObject bloodDamagePrefab;
    [SerializeField] private GameObject explodingCorpsePrefab;
    [SerializeField] private int maxHealth = 30;

    protected int currentHealth;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
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

        ExplodeCorpse explodeScript = Instantiate(explodingCorpsePrefab, transform.position, transform.rotation).GetComponent<ExplodeCorpse>();
        Destroy(gameObject);
    }
}

