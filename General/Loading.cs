using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour {

    public static int levelToLoad = 1;

    public Slider loadingBar;

    private void Awake()
    {
        StartCoroutine(LoadAsynchronously(levelToLoad));
    }

    private IEnumerator LoadAsynchronously(int levelToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            Debug.Log("Loading: " + progress);
            loadingBar.value = progress;

            yield return null;
        }

    }



}
