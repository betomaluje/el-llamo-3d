using UnityEngine;
using SWNetwork;

public class GameSceneManager : MonoBehaviour
{
    public void OnSpawnerReady(bool alreadySetup, SceneSpawner sceneSpawner)
    {
        Debug.Log("OnSpawnerReady " + alreadySetup);

        // Check alreadySetup to see if the scene has been set up before. 
        // If it is true, it means the player disconnected and reconnected to the game. 
        // In this case, we should not spawn a new Player GameObject for the player.
        if (!alreadySetup)
        {
            /*
            // If alreadySetup is false, it means the player just started the game. 
            // We randomly select a SpawnPoint and ask the SceneSpawner to spawn a Player GameObject. 
            // we have 1 playerPrefabs so playerPrefabIndex is 0.
            // We have 2 spawnPoints so we generated a random int between 0 to 1.
            int spawnPointIndex = Random.Range(0, 1);
            sceneSpawner.SpawnForPlayer(0, spawnPointIndex);
            */

            if (sceneSpawner.IsHost)
            {
                sceneSpawner.SpawnForPlayer(0, 0);
            } else
            {
                sceneSpawner.SpawnForPlayer(0, 1);
            }

            // Tell the spawner that we have finished setting up the scene. 
            // alreadySetup will be true when SceneSpawn becomes ready next time.
            sceneSpawner.PlayerFinishedSceneSetup();
        }
    }
}
