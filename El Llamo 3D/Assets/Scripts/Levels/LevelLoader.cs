using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Llamo.Level
{
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
                float progress = Mathf.Clamp01(operation.progress / 0.99f);

                Debug.Log("Scene progress: " + progress.ToString("#0.##%"));

                yield return null;
            }
        }

        public void LoadLevel(Level level)
        {
            // we save the game settings
            GameSettings.instance.SetLevel(level);

            LoadLevel(level.levelNumber);
        }

        public void ExitGame()
        {
            Debug.Log("Exiting game");
            Application.Quit();
        }
    }
}
