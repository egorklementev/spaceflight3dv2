using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour {

    private float speed = 1f;

    public void FadeToLevel(int levelIndex)
    {
        Loading.levelToLoad = levelIndex;
        GetComponent<Animator>().SetTrigger("FadeTrigger");
        GetComponent<Animator>().SetFloat("Speed", speed);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
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
