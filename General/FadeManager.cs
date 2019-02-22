using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour {

    public bool isLoadingFade = false;

    public static int levelToLoad = 1;

    private void Awake()
    {
        if (isLoadingFade)
        {
            StartCoroutine(LoadAsynchronously(levelToLoad));
        }
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        GetComponent<Animator>().SetTrigger("FadeTrigger");
    }

    public void OnFadeComplete()
    {
        if (!isLoadingFade)
        {
            SceneManager.LoadScene(0); // Loading screen
        }
    }
    
    private IEnumerator LoadAsynchronously(int levelToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);

        while(!operation.isDone)
        {
            //float progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

    }
}
