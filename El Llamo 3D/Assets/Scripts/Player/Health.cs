using BetoMaluje.Sikta;
using DG.Tweening;
using SWNetwork;
using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    [SerializeField] private GameObject bloodDamagePrefab;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private GameObject ragdollModel;

    public int currentHealth;

    #region Network

    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;
    private RemoteEventAgent remoteEventAgent;

    private const string HEALTH_CHANGED = "health_changed";
    private const string KILLED_EVENT = "killed";
    private const string DIE_EVENT = "die";
    private const string THROW_GUN_EVENT = "dead_throw_gun";

    #endregion

    public Action<float> OnHealthChanged = delegate { };

    private CameraShake cameraShake;

    private bool isPlayerInmune = false;

    private void Start()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        networkID = GetComponent<NetworkID>();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();
        remoteEventAgent = GetComponent<RemoteEventAgent>();
    }

    public void GiveHealth(int amount)
    {
        int newHealth = currentHealth + amount;

        if (newHealth > maxHealth)
        {
            newHealth = maxHealth;
        }

        // Apply damage and modify the "heal" SyncProperty.
        syncPropertyAgent?.Modify(HEALTH_CHANGED, newHealth);
    }

    public void PerformDamage(int damage)
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
        syncPropertyAgent?.Modify(HEALTH_CHANGED, newHealth);
    }

    public void RemoteHealthChanged()
    {
        // Update the hpSlider when player hp changes
        int newHealth = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).GetIntValue();

        bool wasPlayerDamaged = newHealth < currentHealth;
        currentHealth = newHealth;
        CalculatePercentage();

        if (wasPlayerDamaged)
        {
            // damaged performed
            Instantiate(bloodDamagePrefab, transform.position, transform.rotation);
        }
        else
        {
            // healing. Do nothing for now
        }

        if (networkID.IsMine && wasPlayerDamaged)
        {
            cameraShake.actionShakeCamera();

            if (newHealth <= 0)
            {
                // invoke the "killed" remote event when hp is 0. 
                remoteEventAgent.Invoke(KILLED_EVENT);
            }
        }
    }

    public void OnHpReady()
    {
        currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).GetIntValue();
        int version = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).version;

        // If version is 0, you can call the Modify() method on the SyncPropertyAgent to initialize player's hp to maxHp.
        if (version == 0)
        {
            syncPropertyAgent.Modify(HEALTH_CHANGED, maxHealth);
            currentHealth = maxHealth;
        }

        CalculatePercentage();
    }

    private void CalculatePercentage()
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
            Instantiate(dieBloodPrefab, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

            CreateRagdoll();

            yield return new WaitForSeconds(1f);

            RepositionPlayer();

            /*
            // Only the source player GameObject should be respawned. 
            // SceneSpawner will handle the remote duplicate creation for the respawned player.
            if (networkID.IsMine)
            {
                GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();

                // Call the DelayedRespawnPlayer() method you just added to the GameSceneManager.cs script. 
                gameSceneManager.DelayedRespawnPlayer();

                // Ask the SceneSpawner to destroy the gameObject. 
                // SceneSpawner will destroy the local Player and its remote duplicates.
                NetworkClient.Instance.LastSpawner.DestroyGameObject(gameObject);
            }
            */
        }
    }

    private void ThrowGun()
    {
        Debug.Log("die throw gun");
        PlayerGrab playerGrab = GetComponent<PlayerGrab>();
        Grabable gun = playerGrab.GetActiveHand().GetComponentInChildren<Grabable>();
        if (gun != null)
        {
            gun.StartThrow(10f, Camera.main.transform.forward);
        }
    }

    public void RemoteThrowGun(SWNetworkMessage msg)
    {
        Gun gunTarget = transform.GetComponentInChildren<Gun>();
        if (gunTarget != null)
        {
            Debug.Log("remote throwing gun: ");
            Vector3 direction = msg.PopVector3();
            gunTarget.Throw(400f, direction);
        }
    }

    private void CreateRagdoll()
    {
        NetworkClient.Instance.LastSpawner.SpawnForNonPlayer((int)NonPlayerIndexes.Ragdoll_Corpse, transform.position, Quaternion.identity);
        /*
        SWNetworkMessage msg = new SWNetworkMessage();
        msg.Push(transform.position);
        remoteEventAgent.Invoke(DIE_EVENT, msg);
        */
    }

    public void RemoteCreateRagdoll(SWNetworkMessage msg)
    {
        Debug.Log("remote ragdoll: ");
        Vector3 position = msg.PopVector3();
        Instantiate(ragdollModel, position, Quaternion.identity);
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

        syncPropertyAgent.Modify(HEALTH_CHANGED, maxHealth);
        currentHealth = maxHealth;
        CalculatePercentage();

        isPlayerInmune = false;

        Debug.Log("Player reset!");
    }
}
