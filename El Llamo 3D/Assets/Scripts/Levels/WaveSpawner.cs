using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private float spawnRate = 1.0f;
    [SerializeField] private float timeBetweenWaves = 3.0f;
    [SerializeField] private int enemyCount;

    int waveCount = 1;
    bool waveIsDone = true;

    private LocalGameSceneManager sceneManager;

    private void Start()
    {
        sceneManager = GetComponent<LocalGameSceneManager>();
    }

    void Update()
    {
        if (waveCountText != null)
        {
            waveCountText.text = "Wave: " + waveCount.ToString();
        }

        if (waveIsDone)
        {
            StartCoroutine(waveSpawner());
        }
    }

    IEnumerator waveSpawner()
    {
        waveIsDone = false;

        Debug.Log("Wave " + waveCount.ToString() + ". Spawning " + enemyCount + " enemies!");

        for (int i = 0; i < enemyCount; i++)
        {
            sceneManager.SpawnEnemy(1);

            yield return new WaitForSeconds(spawnRate);
        }

        spawnRate -= 0.1f;
        enemyCount += 3;
        waveCount += 1;

        yield return new WaitForSeconds(timeBetweenWaves);

        waveIsDone = true;
    }
}
