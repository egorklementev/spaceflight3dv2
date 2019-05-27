using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour {
    
    public void FadeToLevel(int levelIndex)
    {
        Loading.levelToLoad = levelIndex;
        GetComponent<Animator>().SetTrigger("FadeTrigger");
    }

    public void SwitchFade()
    {
        GetComponent<Animator>().SetBool("IsFaded", !GetComponent<Animator>().GetBool("IsFaded"));
    }

    public void OnFadeComplete()
    {        
        SceneManager.LoadScene(0); // Loading screen        
    }
    
}
