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

    const string HEALTH = "Hp";
    const string KILLED_EVENT = "killed";
    const string DIE_EVENT = "die";
    const string THROW_GUN_EVENT = "dead_throw_gun";

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

    public void PerformDamage(int damage)
    {
        if (isPlayerInmune)
        {
            return;
        }

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
        CalculatePercentage();

        Instantiate(bloodDamagePrefab, transform.position, transform.rotation);

        if (networkID.IsMine)
        {
            cameraShake.actionShakeCamera();

            if (currentHealth <= 0)
            {
                // invoke the "killed" remote event when hp is 0. 
                remoteEventAgent.Invoke(KILLED_EVENT);
            }
        }
    }

    public void OnHpReady()
    {
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
        SWNetworkMessage msg = new SWNetworkMessage();
        msg.Push(Camera.main.transform.forward);
        remoteEventAgent.Invoke(THROW_GUN_EVENT, msg);
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
        NetworkClient.Instance.LastSpawner.SpawnForNonPlayer(NonPlayerIndexes.Ragdoll_Corpse, transform.position, Quaternion.identity);
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

        transform.DORotate(currentRotation, 1f).SetUpdate(true);

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
        transform.DORotate(currentRotation, .25f).SetUpdate(true);

        syncPropertyAgent.Modify(HEALTH, maxHealth);
        currentHealth = maxHealth;
        CalculatePercentage();

        isPlayerInmune = false;

        Debug.Log("Player reset!");
    }
}
