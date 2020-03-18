using DG.Tweening;
using SWNetwork;
using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private GameObject ragdollModel;

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
        currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH).GetIntValue();

        Debug.Log("Got hit: old currentHealth= " + currentHealth);

        currentHealth -= damage;

        // if hp is lower than 0, set it to 0.
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Die();
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
        CalculatePercentage();
    }

    public void OnHpReady()
    {
        Debug.Log("OnHpPropertyReady");

        currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH).GetIntValue();
        int version = syncPropertyAgent.GetPropertyWithName(HEALTH).version;

        // If version is 0, you can call the Modify() method on the SyncPropertyAgent to initialize player's hp to maxHp.
        if (version == 0)
        {
            syncPropertyAgent.Modify(HEALTH, maxHealth);
            currentHealth = maxHealth;
        }

        CalculatePercentage();
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

        CreateRagdoll();

        RepositionPlayer();
    }

    private void CreateRagdoll() 
    {
        Instantiate(ragdollModel, transform.position, Quaternion.identity);
    }

    private void RepositionPlayer() 
    {
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
        PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();
        if (playerAnimations != null)
        {
            playerAnimations.Revive();
        }

        yield return new WaitForSeconds(1.5f);

        Vector3 currentRotation = Vector3.zero;

        float resetY = 8;

        transform.DOMoveY(resetY, 1f);
        transform.DORotate(currentRotation, 1f).SetUpdate(true);

        syncPropertyAgent.Modify(HEALTH, maxHealth);
        currentHealth = maxHealth;
        CalculatePercentage();

        Debug.Log("Player reset!");
    }
}
