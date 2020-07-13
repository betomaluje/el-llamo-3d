﻿using Llamo.Level;
using SWNetwork;
using UnityEngine;

public class GameSceneManager : LocalGameSceneManager
{
    private SceneSpawner sceneSpawner;

    public void OnSpawnerReady(bool alreadySetup, SceneSpawner sceneSpawner)
    {
        Debug.Log("OnSpawnerReady " + alreadySetup);

        // Check alreadySetup to see if the scene has been set up before. 
        // If it is true, it means the player disconnected and reconnected to the game. 
        // In this case, we should not spawn a new Player GameObject for the player.
        if (!alreadySetup)
        {
            this.sceneSpawner = sceneSpawner;

            // If alreadySetup is false, it means the player just started the game. 
            // We randomly select a SpawnPoint and ask the SceneSpawner to spawn a Player GameObject. 
            // we have 1 playerPrefabs so playerPrefabIndex is 0.
            // We have 2 spawnPoints so we generated a random int between 0 to 1.
            totalPlayerSpawnPoints = sceneSpawner.NumberOfSpawnPoints;

            int spawnPointIndex = GetRandomSpawnPoint(totalPlayerSpawnPoints);
            sceneSpawner.SpawnForPlayer(PlayerIndexes.Player_1, spawnPointIndex);

            // we spawn health items
            PutObject(sceneSpawner, healthItems, healthItems.AmountToSpawn(), false);

            // Tell the spawner that we have finished setting up the scene. 
            // alreadySetup will be true when SceneSpawn becomes ready next time.
            sceneSpawner.PlayerFinishedSceneSetup();
        }
    }

    protected override void OnRoundStarted(GameState gameState)
    {
        if (gameState == GameState.started && sceneSpawner != null)
        {
            PutObject(sceneSpawner, turrets, turrets.AmountToSpawn(), false);

            PutObject(sceneSpawner, enemies, enemies.AmountToSpawn(), false);

            PutObject(sceneSpawner, guns, guns.AmountToSpawn(), false);

            // we spawn posess Objects
            PutObject(sceneSpawner, posessObjects, posessObjects.AmountToSpawn(), false);
        }
    }

    public override void SpawnEnemy(int amount)
    {
        Vector3 position = GetRandomSpawnPoint(enemies).position;
        if (enemies.spawnSFX != null)
        {
            SpawnObjectSfx(enemies.spawnSFX, position);
        }
        NetworkClient.Instance.LastSpawner.SpawnForNonPlayer((int)NonPlayerIndexes.Enemy_Business, position, Quaternion.identity);
    }

    private void PutObject(SceneSpawner sceneSpawner, SpawnObject spawnObject, int totalAmount, bool withSFX)
    {
        for (int i = 0; i < totalAmount; i++)
        {
            // we get a random position of that specific spawn object
            Vector3 position = GetRandomSpawnPoint(spawnObject).position;

            if (withSFX)
            {
                SpawnObjectSfx(spawnObject.spawnSFX, position);
            }

            // we spawn the object using the Type of object to use            
            sceneSpawner.SpawnForNonPlayer((int)spawnObject.type, position, Quaternion.identity);
        }
    }
}
