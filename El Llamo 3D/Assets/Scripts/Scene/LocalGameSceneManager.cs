using Llamo.Level;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LocalGameSceneManager : MonoBehaviour
{
    [Serializable]
    public class SpawnObject
    {
        public NonPlayerIndexes type;
        public Transform[] positions;
        public int amountToSpawn = 4;
        public GameObject spawnPrefab;
        public GameObject spawnSFX;

        private Randomizer randomizer;

        public int AmountToSpawn()
        {
            if (positions.Length == 0)
            {
                return 0;
            }

            if (amountToSpawn > positions.Length)
            {
                amountToSpawn = positions.Length;
            }

            if (randomizer == null)
            {
                randomizer = new Randomizer(amountToSpawn);
            }

            return amountToSpawn;
        }

        public Transform GetRandomSpawnPoint()
        {
            return positions[randomizer.SelectNoRepeat()];
        }

        public void ResetSpawnPositions()
        {
            randomizer.ClearBanned();
        }
    }

    [Serializable]
    public enum SpawnObjectKeys
    {
        Player,
        Gun,
        Enemies,
        Turrets,
        HealthItems,
        PosessObjects,
    }

    [Serializable]
    public class InspectorSpawnObject
    {
        public SpawnObjectKeys key;
        public bool shouldWaitForReady = false;
        public SpawnObject spawnObject;
    }

    [SerializeField] protected InspectorSpawnObject[] spawnObjects;

    private ILevelStateManager levelStateManager;

    private void Awake()
    {
        levelStateManager = FindObjectOfType<ILevelStateManager>();
    }

    private void OnEnable()
    {
        if (levelStateManager != null)
        {
            levelStateManager.OnLevelStateChange += OnRoundStarted;
        }
    }

    private void OnDisable()
    {
        if (levelStateManager != null)
        {
            levelStateManager.OnLevelStateChange -= OnRoundStarted;
        }
    }

    protected virtual void SpawnFirstObjects()
    {
        foreach (InspectorSpawnObject spawnObject in spawnObjects)
        {
            if (!spawnObject.shouldWaitForReady)
            {
                SpawnObject spawn = spawnObject.spawnObject;
                PutObject(spawn, spawn.AmountToSpawn());
            }
        }
    }

    protected virtual void SpawnLevelReadyObjects()
    {
        foreach (InspectorSpawnObject spawnObject in spawnObjects)
        {
            if (spawnObject.shouldWaitForReady)
            {
                SpawnObject spawn = spawnObject.spawnObject;
                PutObject(spawn, spawn.AmountToSpawn());
                spawn.ResetSpawnPositions();
            }
        }
    }

    protected virtual void OnRoundStarted(GameState gameState)
    {
        if (gameState == GameState.started)
        {
            SpawnLevelReadyObjects();
        }
    }

    public virtual void Start()
    {
        SpawnFirstObjects();
    }

    private void PutObject(SpawnObject spawnObject, int totalAmount)
    {
        PutObject(spawnObject, totalAmount, false);
    }

    private void PutObjectWithSFX(SpawnObject spawnObject, int totalAmount)
    {
        PutObject(spawnObject, totalAmount, true);
    }

    private void PutObject(SpawnObject spawnObject, int totalAmount, bool withSFX)
    {
        if (spawnObject.spawnPrefab == null)
        {
            Debug.LogWarning("Trying to spawn " + spawnObject.type.ToString() + " with a null game object");
            return;
        }

        Debug.Log("Spawning " + spawnObject.type.ToString() + " for " + totalAmount);

        for (int i = 0; i < totalAmount; i++)
        {
            // we get a random position index to use
            Transform index = GetRandomSpawnPoint(spawnObject);
            // we use that index to get a random position from the array
            Vector3 position = index.position;

            if (withSFX)
            {
                SpawnObjectSfx(spawnObject.spawnSFX, position);
            }

            // we spawn the object using the Type of object to use
            Instantiate(spawnObject.spawnPrefab, position, Quaternion.identity);
        }
    }

    protected void SpawnObjectSfx(GameObject sfx, Vector3 position)
    {
        if (sfx != null)
        {
            Instantiate(sfx, position, Quaternion.identity);
        }
    }

    public virtual void SpawnEnemy(int amount)
    {
        PutObjectWithSFX(FindRandomEnemy(), amount);
    }

    private InspectorSpawnObject[] GetAllSpawnObjectForKey(SpawnObjectKeys key)
    {
        InspectorSpawnObject[] spawnObjectsOfType = Array.FindAll(spawnObjects, inspectorSpawnObject =>
            inspectorSpawnObject.key == key && inspectorSpawnObject.spawnObject != null
        );

        return spawnObjectsOfType;
    }

    protected SpawnObject FindRandomEnemy()
    {
        return FindRandomSpawnObject(SpawnObjectKeys.Enemies);
    }

    protected SpawnObject FindRandomSpawnObject(SpawnObjectKeys key)
    {
        InspectorSpawnObject[] allSpawnObjects = GetAllSpawnObjectForKey(key);

        if (allSpawnObjects == null || allSpawnObjects.Length <= 0)
        {
            Debug.LogWarning("SpawnObjects of type " + key.ToString() + " not found");
            return null;
        }

        int index = Random.Range(0, allSpawnObjects.Length);

        return allSpawnObjects[index].spawnObject;
    }

    protected Transform GetRandomSpawnPoint(SpawnObject spawnObject)
    {
        return spawnObject.GetRandomSpawnPoint();
    }

    protected int GetRandomSpawnPoint(int totalPoints)
    {
        if (totalPoints <= 0)
        {
            totalPoints = 0;
        }
        return Random.Range(0, totalPoints);

    }
}
