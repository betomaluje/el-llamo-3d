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

    [Space]
    [Header("Ragdoll Debug")]
    [SerializeField] private KeyCode ragdoll;
    [SerializeField] protected Transform ragdollPosition;
    [SerializeField] private GameObject ragdollCorpse;

    protected int totalPlayerSpawnPoints;

    private bool spawningRagdoll = false;

    private void Start()
    {
        totalPlayerSpawnPoints = players.positions.Length;

        PutObject(players, players.AmountToSpawn());

        // we spawn guns
        PutObject(guns, guns.AmountToSpawn());

        // we spawn enemies
        PutObject(enemies, enemies.AmountToSpawn());

        // we spawn health items
        PutObject(healthItems, healthItems.AmountToSpawn());
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
                SoundManager.instance.Play("Portal");
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
