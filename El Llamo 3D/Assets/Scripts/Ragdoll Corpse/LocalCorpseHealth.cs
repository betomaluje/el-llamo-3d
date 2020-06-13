using System;
using UnityEngine;

public class LocalCorpseHealth : MonoBehaviour, IHealth
{
    [Header("FX")]
    [SerializeField] private GameObject bloodDamagePrefab;
    [SerializeField] private GameObject dieFXPrefab;
    [SerializeField] private GameObject explodingCorpsePrefab;
    [SerializeField] protected int maxHealth = 30;

    public Action<float> OnHealthChanged = delegate { };

    protected int currentHealth;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;

        CalculatePercentage();
    }

    protected void CalculatePercentage()
    {
        float healthPercentage = currentHealth / (float)maxHealth;
        OnHealthChanged(healthPercentage);
    }

    public virtual void Die()
    {
        // destroy corpse
        transform.parent = null;
        Destroy(gameObject);

        Instantiate(explodingCorpsePrefab, transform.position, transform.rotation).GetComponent<ExplodeCorpse>();

        Instantiate(dieFXPrefab, transform.position, Quaternion.identity);
    }

    #region IHealth

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
                CalculatePercentage();
                AddDamageSFX(impactPosition);
            }
        }
    }

    public void GiveHealth(int health, Vector3 impactPosition)
    {
        // do nothing
    }

    public void AddDamageSFX(Vector3 impactPosition)
    {
        Instantiate(bloodDamagePrefab, impactPosition, transform.rotation);
    }

    public void AddHealSFX(Vector3 impactPosition)
    {
        // do nothing
    }

    #endregion
}

