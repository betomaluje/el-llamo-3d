using SWNetwork;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameSceneManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnObject
    {
        public NonPlayerIndexes type;
        public int amountToSpawn = 4;
        public Transform[] positions;
    }

    [SerializeField] private SpawnObject guns;

    [SerializeField] private SpawnObject enemies;

    [SerializeField] private SpawnObject healthItems;

    [Space]
    [Header("Ragdoll Debug")]
    [SerializeField] private KeyCode ragdoll;
    [SerializeField] private Transform ragdollPosition;
    [SerializeField] private GameObject ragdollCorpse;

    private bool spawningRagdoll = false;

    private int totalPlayerSpawnPoints;

    private void Update()
    {
        if (Input.GetKeyDown(ragdoll))
        {
            spawningRagdoll = !spawningRagdoll;

            if (spawningRagdoll)
            {
                if (GameSettings.instance.usingNetwork)
                {
                    NetworkClient.Instance.LastSpawner.SpawnForNonPlayer((int)NonPlayerIndexes.Ragdoll_Corpse, ragdollPosition.position, Quaternion.identity);
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

            // we spawn guns
            PutObject(sceneSpawner, guns);

            // we spawn enemies
            PutObject(sceneSpawner, enemies);

            // we spawn health items
            PutObject(sceneSpawner, healthItems);

            // Tell the spawner that we have finished setting up the scene. 
            // alreadySetup will be true when SceneSpawn becomes ready next time.
            sceneSpawner.PlayerFinishedSceneSetup();
        }
    }

    private void PutObject(SceneSpawner sceneSpawner, SpawnObject spawnObject)
    {
        int totalAmount = spawnObject.amountToSpawn;
        Debug.Log("spawning " + spawnObject.type.ToString());
        for (int i = 0; i < totalAmount; i++)
        {
            // we get a random position index to use
            int index = GetRandomSpawnPoint(totalAmount);
            // we use that index to get a random position from the array
            Vector3 position = spawnObject.positions[index].position;
            // we spawn the object using the Type of object to use
            sceneSpawner.SpawnForNonPlayer((int)spawnObject.type, position, Quaternion.identity);
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
