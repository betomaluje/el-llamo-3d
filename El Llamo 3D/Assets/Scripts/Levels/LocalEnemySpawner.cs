using UnityEngine;

public class LocalEnemySpawner : MonoBehaviour
{
    [SerializeField] private float timeBetweenSpawns = 10f;
    [SerializeField] private int amountToSpawn = 1;
    [SerializeField] private int maxExtraPerTime = 10;

    private LocalGameSceneManager sceneManager;

    private int currentAmount = 0;
    private float timeout = 2;

    private void Start()
    {
        sceneManager = GetComponent<LocalGameSceneManager>();
    }

    private void Update()
    {
        if (timeout > 0)
        {
            // Reduces the timeout by the time passed since the last frame
            timeout -= Time.deltaTime;

            return;
        }

        if (currentAmount < maxExtraPerTime)
        {
            // Spawn object once
            sceneManager.SpawnEnemy(1);
            currentAmount++;
        }

        // Reset timer
        timeout = timeBetweenSpawns;
    }

    public void DecreaseEnemyAmount()
    {
        currentAmount--;

        if (currentAmount < 0)
        {
            currentAmount = 0;
        }
    }
}
