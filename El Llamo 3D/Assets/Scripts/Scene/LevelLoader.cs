using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(SceneNumbers sceneNumber) {
        StartCoroutine(LoadScene((int)sceneNumber));
    }

    private IEnumerator LoadScene(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while(!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("progress: " + progress);

            yield return null;
        }
    }
}
