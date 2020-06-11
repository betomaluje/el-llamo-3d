using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class LocalHealth : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    public GameObject bloodDamagePrefab;
    public int maxHealth = 100;

    [SerializeField] private Transform cameraParticleTransform;
    [SerializeField] private GameObject[] healParticles;

    [SerializeField] private GameObject ragdollModel;

    [HideInInspector]
    public int currentHealth;

    public Action<float> OnHealthChanged = delegate { };

    protected CameraShake cameraShake;

    protected bool isPlayerInmune = false;

    protected virtual void Awake()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        currentHealth = maxHealth;

        CalculatePercentage();
    }

    public virtual void GiveHealth(int amount)
    {
        int newHealth = currentHealth + amount;

        if (newHealth > maxHealth)
        {
            newHealth = maxHealth;
        }

        AddHealSFX();

        // Apply damage and modify the "heal" SyncProperty.
        HealthChanged(newHealth);
    }

    protected void AddHealSFX()
    {
        SoundManager.instance.Play("Heal");

        foreach (GameObject particle in healParticles)
        {
            Instantiate(particle, cameraParticleTransform.position, Quaternion.identity);
        }
    }

    public virtual void PerformDamage(int damage)
    {
        if (isPlayerInmune)
        {
            return;
        }

        //currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).GetIntValue();
        int newHealth = currentHealth - damage;

        // if hp is lower than 0, set it to 0.
        if (newHealth < 0)
        {
            newHealth = 0;
        }

        // Apply damage and modify the "damage" SyncProperty.
        HealthChanged(newHealth);
    }

    public void HealthChanged(int newHealth)
    {
        bool wasPlayerDamaged = newHealth < currentHealth;
        currentHealth = newHealth;
        CalculatePercentage();

        // we only instantiate blood when it's damaged, not healing
        if (wasPlayerDamaged)
        {
            // damaged performed
            AddDamageSFX();
        }

        if (wasPlayerDamaged)
        {
            cameraShake.actionShakeCamera();

            if (newHealth <= 0)
            {
                // invoke the "killed" remote event when hp is 0. 
                Die();
            }
        }
    }

    protected void AddDamageSFX()
    {
        Instantiate(bloodDamagePrefab, cameraParticleTransform.position, transform.rotation);
    }

    protected void CalculatePercentage()
    {
        float healthPercentage = currentHealth / (float)maxHealth;
        OnHealthChanged(healthPercentage);
    }

    public void Die()
    {
        StartCoroutine(PerformDie());
    }

    private IEnumerator PerformDie()
    {
        if (!isPlayerInmune)
        {
            isPlayerInmune = true;

            ThrowGun();
            Instantiate(dieBloodPrefab, transform.position, Quaternion.identity);

            CreateRagdoll();

            yield return new WaitForSeconds(1f);

            RepositionPlayer();
        }
    }

    private void ThrowGun()
    {
        GrabController grabController = GetComponent<GrabController>();
        if (grabController != null)
        {
            grabController.RemoveAllGrabables();
        }
    }

    protected virtual void CreateRagdoll()
    {
        Instantiate(ragdollModel, transform.position, transform.rotation);
    }

    private void RepositionPlayer()
    {
        Quaternion originalRotation = transform.rotation;
        Vector3 currentRotation = transform.position;
        currentRotation.z = UnityEngine.Random.Range(0, 2) == 0 ? -90 : 90;
        currentRotation.x = 0;

        float deadY = 0.5f;

        transform.DOMoveY(deadY, 1f);

        transform.DORotate(currentRotation, 1f);

        PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();
        if (playerAnimations != null)
        {
            playerAnimations.DieAnim();
        }

        StartCoroutine(Reset(originalRotation));
    }

    private IEnumerator Reset(Quaternion originalRotation)
    {
        PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();
        if (playerAnimations != null)
        {
            playerAnimations.Revive();
        }

        yield return new WaitForSeconds(1.5f);

        Vector3 currentRotation = originalRotation.eulerAngles;

        float resetY = 8;

        transform.DOMoveY(resetY, 1f);
        transform.DORotate(currentRotation, .25f);

        currentHealth = maxHealth;
        CalculatePercentage();

        isPlayerInmune = false;

        Debug.Log("Player reset!");
    }
}
