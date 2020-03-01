using System.Collections;
using System;
using UnityEngine;
using DG.Tweening;
using SWNetwork;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    private Rigidbody rb;
    private Collider col;

    #region Network

    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;

    #endregion

    public Action<float> OnHealthChanged = delegate {  };

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        networkID = GetComponent<NetworkID>();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();
    }    

    public void PerformDamage(int damage)
    {
        ModifyHealth(-damage);
    }

    private void ModifyHealth(int amount)
    {
        currentHealth += amount;

        float healthPercentage = (float)currentHealth / (float)maxHealth;
        OnHealthChanged(healthPercentage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void OnHPReady()
    {
        Debug.Log("OnHPPropertyReady");

        // Get the current value of the "hp" SyncProperty.
        currentHealth = syncPropertyAgent.GetPropertyWithName("hp").GetIntValue();

        // Check if the local player has ownership of the GameObject. 
        // Source GameObject can modify the "hp" SyncProperty.
        // Remote duplicates should only be able to read the "hp" SyncProperty.
        if (networkID.IsMine)
        {
            int version = syncPropertyAgent.GetPropertyWithName("hp").version;

            if (version != 0)
            {
                // You can check the version of a SyncProperty to see if it has been initialized. 
                // If version is not 0, it means the SyncProperty has been modified before. 
                // Probably the player got disconnected from the game. 
                // Set hpSlider's value to currentHP to restore player's hp.                
                float healthPercentage = (float)currentHealth / (float)maxHealth;
                OnHealthChanged(healthPercentage);
            }
            else
            {
                // If version is 0, you can call the Modify() method on the SyncPropertyAgent to initialize player's hp to maxHp.
                syncPropertyAgent.Modify("hp", maxHealth);
                OnHealthChanged(1);
            }
        }
        else
        {
            float healthPercentage = (float)currentHealth / (float)maxHealth;
            OnHealthChanged(healthPercentage);
        }
    }

    private void Die()
    {
        Instantiate(dieBloodPrefab, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

        Vector3 currentRotation = transform.position;
        currentRotation.z = UnityEngine.Random.Range(0, 2) == 0 ? -90 : 90;
        currentRotation.x = 0;
        
        float deadY = 0.5f;

        transform.DOMoveY(deadY, 1f);

        Sequence s = DOTween.Sequence();
        s.Append(transform.DORotate(currentRotation, 1f)).SetUpdate(true);
        s.AppendCallback(() => MakeUntouchable());

        PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();
        if (playerAnimations != null)
        {
            playerAnimations.DieAnim();
        }

        StartCoroutine(Reset());
    }

    private void MakeUntouchable()
    {
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        col.isTrigger = true;
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        
        if (col != null)
        {
            col.isTrigger = false;
        }        

        transform.rotation = Quaternion.identity;
        Vector3 currentPos = transform.position;

        currentPos.y = 1;

        transform.position = currentPos;

        currentHealth = maxHealth;
        ModifyHealth(currentHealth);

        Debug.Log("Player reset!");
    }
}
