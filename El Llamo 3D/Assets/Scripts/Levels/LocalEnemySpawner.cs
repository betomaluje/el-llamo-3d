using Llamo.Level;
using UnityEngine;

public class LocalEnemySpawner : MonoBehaviour
{
    [SerializeField] private float timeBetweenSpawns = 10f;
    [SerializeField] private int amountToSpawn = 1;
    [SerializeField] private int maxExtraPerTime = 10;

    private LocalGameSceneManager sceneManager;

    private int currentAmount = 0;
    private float timeout = 2;

    private bool hasRoundStarted = false;
    private ILevelStateManager levelStateManager;

    private void Awake()
    {
        levelStateManager = FindObjectOfType<ILevelStateManager>();
        sceneManager = GetComponent<LocalGameSceneManager>();

        // if we don't have a levelStateManager it means that we can spawn enemies
        hasRoundStarted = levelStateManager == null;
    }

    private void OnEnable()
    {
        levelStateManager.OnLevelStateChange += OnRoundStarted;
    }

    private void OnDisable()
    {
        levelStateManager.OnLevelStateChange -= OnRoundStarted;
    }

    private void OnRoundStarted(GameState gameState)
    {
        hasRoundStarted = true;
    }

    private void Update()
    {
        if (!hasRoundStarted || amountToSpawn == 0)
        {
            return;
        }

        if (timeout > 0)
        {
            // Reduces the timeout by the time passed since the last frame
            timeout -= Time.deltaTime;

            return;
        }

        if (currentAmount < maxExtraPerTime)
        {
            // Spawn object once
            sceneManager.SpawnEnemy(amountToSpawn);
            currentAmount += amountToSpawn;
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
