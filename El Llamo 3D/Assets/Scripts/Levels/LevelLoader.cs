using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(Level.LevelNumber levelNumber)
    {
        StartCoroutine(LoadScene((int)levelNumber));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            yield return null;
        }
    }

    public void LoadLevel(Level level)
    {
        // we save the game settings
        GameSettings.instance.gameType = level.gameType;
        GameSettings.instance.usingNetwork = level.usesNetwork;

        LoadLevel(level.levelNumber);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game");
        Application.Quit();
    }
}
