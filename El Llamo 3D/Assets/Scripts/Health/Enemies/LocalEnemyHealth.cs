using System;
using UnityEngine;

namespace Llamo.Health
{
    public class LocalEnemyHealth : MonoBehaviour, IHealth
    {
        [Header("FX")]
        [SerializeField] private GameObject bloodDamagePrefab;
        [SerializeField] private GameObject dieFXPrefab;
        [SerializeField] private GameObject explodingCorpsePrefab;
        [SerializeField] private GameObject corpsePrefab;
        [SerializeField] protected int maxHealth = 30;
        [Range(0, 100)]
        [SerializeField] private int probabilityOfExplode = 30;

        public Action<float> OnHealthChanged = delegate { };

        protected int currentHealth;

        protected float probOfExploding;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            currentHealth = maxHealth;
            probOfExploding = probabilityOfExplode / 100f;

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

            BroadcastEnemyDead();

            float randomExploding = UnityEngine.Random.value;

            if (randomExploding <= probOfExploding)
            {
                Instantiate(explodingCorpsePrefab, transform.position, transform.rotation).GetComponent<ExplodeCorpse>();
            }
            else
            {
                CreateRagdoll();
            }

            Instantiate(dieFXPrefab, transform.position, Quaternion.identity);
        }

        protected void BroadcastEnemyDead()
        {
            FindObjectOfType<LocalEnemySpawner>().DecreaseEnemyAmount();
        }

        protected virtual void CreateRagdoll()
        {
            Instantiate(corpsePrefab, transform.position, transform.rotation);
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
}