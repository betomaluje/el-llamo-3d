using UnityEngine;

public class LocalEnemySpawner : MonoBehaviour
{
    [SerializeField] private float timeBetweenSpawns = 10f;
    [SerializeField] private int amountToSpawn = 1;

    private LocalGameSceneManager sceneManager;

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

        // Spawn object once
        sceneManager.SpawnEnemy(1);

        // Reset timer
        timeout = timeBetweenSpawns;
    }
}
