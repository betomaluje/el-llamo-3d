using DG.Tweening;
using SWNetwork;
using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    [SerializeField] private int maxHealth = 100;

    public int currentHealth;

    #region Network

    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;
    const string HEALTH = "Hp";

    #endregion

    public Action<float> OnHealthChanged = delegate { };

    private void Start()
    {
        networkID = GetComponent<NetworkID>();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();
    }

    public void PerformDamage(int damage)
    {
        ModifyHealth(-damage);
    }

    private void ModifyHealth(int amount)
    {
        Debug.Log("Got hit: old currentHealth= " + currentHealth);

        if (currentHealth > 0)
        {
            currentHealth += amount;

            // if hp is lower than 0, set it to 0.
            if (currentHealth < 0)
            {
                currentHealth = 0;
                Die();
            }
            else
            {
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
            }
        }

        Debug.Log("Got hit: new currentHealth= " + currentHealth);

        // Apply damage and modify the "hp" SyncProperty.
        syncPropertyAgent?.Modify(HEALTH, currentHealth);
    }

    public void OnHpChanged()
    {
        // Update the hpSlider when player hp changes
        currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH).GetIntValue();
        Debug.Log("hp changed: " + currentHealth);
        float healthPercentage = currentHealth / (float)maxHealth;
        OnHealthChanged(healthPercentage);
    }

    public void OnHpReady()
    {
        Debug.Log("OnHpPropertyReady");

        // Get the current value of the "hp" SyncProperty.
        currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH).GetIntValue();

        // Check if the local player has ownership of the GameObject. 
        // Source GameObject can modify the "hp" SyncProperty.
        // Remote duplicates should only be able to read the "hp" SyncProperty.
        if (networkID.IsMine)
        {
            int version = syncPropertyAgent.GetPropertyWithName(HEALTH).version;

            if (version != 0)
            {
                // You can check the version of a SyncProperty to see if it has been initialized. 
                // If version is not 0, it means the SyncProperty has been modified before. 
                // Probably the player got disconnected from the game. 
                // Set hpSlider's value to currentHP to restore player's hp.                
                CalculatePercentage();
            }
            else
            {
                // If version is 0, you can call the Modify() method on the SyncPropertyAgent to initialize player's hp to maxHp.
                syncPropertyAgent.Modify(HEALTH, maxHealth);
                currentHealth = maxHealth;
                CalculatePercentage();
            }
        }
        else
        {
            CalculatePercentage();
        }
    }

    private void CalculatePercentage()
    {
        float healthPercentage = currentHealth / (float)maxHealth;
        OnHealthChanged(healthPercentage);
    }

    private void Die()
    {
        Instantiate(dieBloodPrefab, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

        ThrowGun();

        Vector3 currentRotation = transform.position;
        currentRotation.z = UnityEngine.Random.Range(0, 2) == 0 ? -90 : 90;
        currentRotation.x = 0;

        float deadY = 0.5f;

        transform.DOMoveY(deadY, 1f);

        transform.DORotate(currentRotation, 1f).SetUpdate(true);

        PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();
        if (playerAnimations != null)
        {
            playerAnimations.DieAnim();
        }

        StartCoroutine(Reset());
    }

    private void ThrowGun()
    {
        Gun gunTarget = transform.GetComponentInChildren<Gun>();
        if (gunTarget != null)
        {
            Debug.Log("Dead! Throwing gun");
            gunTarget.Throw(400f);
        }
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);

        transform.rotation = Quaternion.identity;
        Vector3 currentPos = transform.position;

        currentPos.y = 1;

        transform.position = currentPos;

        currentHealth = maxHealth;
        ModifyHealth(currentHealth);

        Debug.Log("Player reset!");
    }
}
