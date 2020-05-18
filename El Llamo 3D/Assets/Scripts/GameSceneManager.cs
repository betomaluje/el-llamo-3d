using SWNetwork;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameSceneManager : MonoBehaviour
{
    [Header("Guns")]
    [SerializeField] private int amountOfGuns = 4;
    [SerializeField] private Transform[] gunsPositions;

    [Space]
    [Header("Enemies")]
    [SerializeField] private int amountOfEnemies = 4;
    [SerializeField] private Transform[] enemiesPositions;

    [Space]
    [Header("Ragdoll Debug")]
    [SerializeField] private KeyCode ragdoll;
    [SerializeField] private Transform ragdollPosition;
    [SerializeField] private GameObject ragdollCorpse;

    private int totalPlayerSpawnPoints = 0;
    private int totalGunsSpawnPoints = 0;
    private int totalEnemiesSpawnPoints = 0;

    private bool spawningRagdoll = false;

    private void Update()
    {
        if (Input.GetKeyDown(ragdoll))
        {
            spawningRagdoll = !spawningRagdoll;

            if (spawningRagdoll)
            {
                if (GameSettings.instance.usingNetwork)
                {
                    NetworkClient.Instance.LastSpawner.SpawnForNonPlayer(NonPlayerIndexes.Ragdoll_Corpse, ragdollPosition.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(ragdollCorpse, ragdollPosition.position, Quaternion.identity);
                }
            }
        }
    }

    public void OnSpawnerReady(bool alreadySetup, SceneSpawner sceneSpawner)
    {
        Debug.Log("OnSpawnerReady " + alreadySetup);

        // Check alreadySetup to see if the scene has been set up before. 
        // If it is true, it means the player disconnected and reconnected to the game. 
        // In this case, we should not spawn a new Player GameObject for the player.
        if (!alreadySetup)
        {
            // If alreadySetup is false, it means the player just started the game. 
            // We randomly select a SpawnPoint and ask the SceneSpawner to spawn a Player GameObject. 
            // we have 1 playerPrefabs so playerPrefabIndex is 0.
            // We have 2 spawnPoints so we generated a random int between 0 to 1.
            totalPlayerSpawnPoints = sceneSpawner.NumberOfSpawnPoints;

            int spawnPointIndex = GetRandomSpawnPoint(totalPlayerSpawnPoints);
            sceneSpawner.SpawnForPlayer(PlayerIndexes.Player_1, spawnPointIndex);

            // we get the possible total guns to spawn
            totalGunsSpawnPoints = gunsPositions.Length;

            SpawnGuns(sceneSpawner);

            // we get the possible total enemies to spawn
            totalEnemiesSpawnPoints = enemiesPositions.Length;

            SpawnEnemies(sceneSpawner);

            // Tell the spawner that we have finished setting up the scene. 
            // alreadySetup will be true when SceneSpawn becomes ready next time.
            sceneSpawner.PlayerFinishedSceneSetup();
        }
    }

    private void SpawnEnemies(SceneSpawner sceneSpawner)
    {
        for (int i = 0; i < amountOfEnemies; i++)
        {
            int enemySpawnPointIndex = GetRandomSpawnPoint(totalEnemiesSpawnPoints);
            Vector3 position = enemiesPositions[enemySpawnPointIndex].position;
            sceneSpawner.SpawnForNonPlayer(NonPlayerIndexes.Enemy_Business, position, Quaternion.identity);
        }
    }

    private void SpawnGuns(SceneSpawner sceneSpawner)
    {
        for (int i = 0; i < amountOfGuns; i++)
        {
            int gunSpawnPointIndex = GetRandomSpawnPoint(totalGunsSpawnPoints);
            Vector3 position = gunsPositions[gunSpawnPointIndex].position;
            sceneSpawner.SpawnForNonPlayer(NonPlayerIndexes.Gun, position, Quaternion.identity);
        }
    }

    private int GetRandomSpawnPoint(int totalPoints)
    {
        if (totalPoints == 0)
        {
            return 0;
        }

        if (totalPoints <= 0)
        {
            totalPoints = 2;
        }
        return Random.Range(0, totalPoints - 1);
    }

    /**
     * Called by the Health script anytime a player dies
     */
    public void DelayedRespawnPlayer()
    {
        StartCoroutine(RespawnPlayer(1f));
    }

    IEnumerator RespawnPlayer(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Respawn the player at a random SpawnPoint
        int spawnPointIndex = GetRandomSpawnPoint(totalPlayerSpawnPoints);
        NetworkClient.Instance.LastSpawner.SpawnForPlayer(PlayerIndexes.Player_1, spawnPointIndex);
    }
}
