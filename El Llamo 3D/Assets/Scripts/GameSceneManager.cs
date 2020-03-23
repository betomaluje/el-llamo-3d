﻿using SWNetwork;
using System.Collections;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private int totalSpawnPoints = 0;

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
            totalSpawnPoints = sceneSpawner.NumberOfSpawnPoints;

            int spawnPointIndex = GetSpawnPoint();

            Debug.Log("spawnPointIndex: " + spawnPointIndex);

            sceneSpawner.SpawnForPlayer(0, spawnPointIndex);

            // Tell the spawner that we have finished setting up the scene. 
            // alreadySetup will be true when SceneSpawn becomes ready next time.
            sceneSpawner.PlayerFinishedSceneSetup();
        }
    }

    private int GetSpawnPoint()
    {
        if (totalSpawnPoints <= 0)
        {
            totalSpawnPoints = 2;
        }
        return Random.Range(0, totalSpawnPoints - 1); ;
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
        NetworkClient.Instance.LastSpawner.SpawnForPlayer(0, spawnPointIndex);
    }
}
