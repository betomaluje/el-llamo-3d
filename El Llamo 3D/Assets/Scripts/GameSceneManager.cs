using SWNetwork;
using System.Collections;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private int amountOfGuns = 4;
    [SerializeField] private Transform[] nonPlayerPositions;

    [Header("Ragdoll Debug")]
    [SerializeField] private KeyCode ragdoll;
    [SerializeField] private Transform ragdollPosition;

    private int totaPlayerSpawnPoints = 0;
    private int totaNonPlayerSpawnPoints = 0;

    private bool spawningRagdoll = false;

    private void Update()
    {
        if (Input.GetKeyDown(ragdoll))
        {
            spawningRagdoll = !spawningRagdoll;

            if (spawningRagdoll)
            {
                NetworkClient.Instance.LastSpawner.SpawnForNonPlayer(NonPlayerIndexes.Ragdoll_Corpse, ragdollPosition.position, Quaternion.identity);
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
            totaPlayerSpawnPoints = sceneSpawner.NumberOfSpawnPoints;

            int spawnPointIndex = GetSpawnPoint();
            sceneSpawner.SpawnForPlayer(PlayerIndexes.Player_1, spawnPointIndex);

            totaNonPlayerSpawnPoints = nonPlayerPositions.Length;

            SpawnGuns(sceneSpawner);

            // Tell the spawner that we have finished setting up the scene. 
            // alreadySetup will be true when SceneSpawn becomes ready next time.
            sceneSpawner.PlayerFinishedSceneSetup();
        }
    }

    private void SpawnGuns(SceneSpawner sceneSpawner)
    {
        for (int i = 0; i < amountOfGuns; i++)
        {
            int gunSpawnPointIndex = GetNonPlayerSpawnPoint();
            Vector3 position = nonPlayerPositions[gunSpawnPointIndex].position;
            sceneSpawner.SpawnForNonPlayer(NonPlayerIndexes.Gun, position, Quaternion.identity);
        }
    }

    private int GetSpawnPoint()
    {
        if (totaPlayerSpawnPoints <= 0)
        {
            totaPlayerSpawnPoints = 2;
        }
        return Random.Range(0, totaPlayerSpawnPoints - 1);
    }

    private int GetNonPlayerSpawnPoint()
    {
        if (totaNonPlayerSpawnPoints <= 0)
        {
            totaNonPlayerSpawnPoints = 2;
        }
        return Random.Range(0, totaNonPlayerSpawnPoints - 1);
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
        int spawnPointIndex = GetSpawnPoint();
        NetworkClient.Instance.LastSpawner.SpawnForPlayer(PlayerIndexes.Player_1, spawnPointIndex);
    }
}
