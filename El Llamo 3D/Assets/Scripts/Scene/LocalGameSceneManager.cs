using UnityEngine;
using Random = UnityEngine.Random;

public class LocalGameSceneManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnObject
    {
        public NonPlayerIndexes type;
        public int amountToSpawn = 4;
        public Transform[] positions;
        public GameObject spawnPrefab;
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
        int spawnPointIndex = GetRandomSpawnPoint(totalPlayerSpawnPoints);
        PutObject(players);

        // we spawn guns
        PutObject(guns);

        // we spawn enemies
        PutObject(enemies);

        // we spawn health items
        PutObject(healthItems);
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

    private void PutObject(SpawnObject spawnObject)
    {
        if (spawnObject.spawnPrefab == null)
        {
            return;
        }

        int totalAmount = spawnObject.amountToSpawn;
        Debug.Log("spawning " + spawnObject.type.ToString());
        for (int i = 0; i < totalAmount; i++)
        {
            // we get a random position index to use
            int index = GetRandomSpawnPoint(totalAmount);
            // we use that index to get a random position from the array
            Vector3 position = spawnObject.positions[index].position;
            // we spawn the object using the Type of object to use
            Instantiate(spawnObject.spawnPrefab, position, Quaternion.identity);
        }
    }

    protected int GetRandomSpawnPoint(int totalPoints)
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
}
