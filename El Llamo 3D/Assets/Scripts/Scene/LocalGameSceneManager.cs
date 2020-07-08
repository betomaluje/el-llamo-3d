using Llamo.Level;
using UnityEngine;
using Random = UnityEngine.Random;

public class LocalGameSceneManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnObject
    {
        public NonPlayerIndexes type;
        public Transform[] positions;
        public int amountToSpawn = 4;
        public GameObject spawnPrefab;
        public GameObject spawnSFX;

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

            return amountToSpawn;
        }
    }

    [SerializeField] protected SpawnObject players;

    [SerializeField] protected SpawnObject guns;

    [SerializeField] protected SpawnObject enemies;

    [SerializeField] protected SpawnObject healthItems;

    [SerializeField] protected SpawnObject posessObjects;

    [Space]
    [Header("Ragdoll Debug")]
    [SerializeField] private KeyCode ragdoll;
    [SerializeField] protected Transform ragdollPosition;
    [SerializeField] private GameObject ragdollCorpse;

    protected int totalPlayerSpawnPoints;

    private bool spawningRagdoll = false;

    private ILevelStateManager levelStateManager;

    private void Awake()
    {
        levelStateManager = FindObjectOfType<ILevelStateManager>();
    }

    private void OnEnable()
    {
        levelStateManager.OnLevelStateChange += OnRoundStarted;
    }

    private void OnDisable()
    {
        levelStateManager.OnLevelStateChange -= OnRoundStarted;
    }

    protected virtual void OnRoundStarted(GameState gameState)
    {
        if (gameState == GameState.started)
        {
            PutObject(enemies, enemies.AmountToSpawn());
        }
    }

    private void Start()
    {
        totalPlayerSpawnPoints = players.positions.Length;

        PutObject(players, players.AmountToSpawn());

        // we spawn guns
        PutObject(guns, guns.AmountToSpawn());

        // we spawn enemies
        //PutObject(enemies, enemies.AmountToSpawn());

        // we spawn health items
        PutObject(healthItems, healthItems.AmountToSpawn());

        // we spawn posess Objects
        PutObject(posessObjects, posessObjects.AmountToSpawn());
    }

    private void Update()
    {
        if (Input.GetKeyDown(ragdoll))
        {
            spawningRagdoll = !spawningRagdoll;

            if (spawningRagdoll)
            {
                AddRagdollCorpse();
            }
        }
    }

    protected virtual void AddRagdollCorpse()
    {
        Instantiate(ragdollCorpse, ragdollPosition.position, Quaternion.identity);
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
            return;
        }

        for (int i = 0; i < totalAmount; i++)
        {
            // we get a random position index to use
            Transform index = GetRandomSpawnPoint(spawnObject.positions);
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
        PutObjectWithSFX(enemies, amount);
    }

    protected Transform GetRandomSpawnPoint(Transform[] positions)
    {
        int index = Random.Range(0, positions.Length);
        return positions[index];
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
