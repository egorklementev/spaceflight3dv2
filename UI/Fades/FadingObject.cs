using UnityEngine;
using UnityEngine.SceneManagement;

public class FadingObject : MonoBehaviour {

    public float time = 1f; // Time in seconds to fade in/out
    public float delayTime = 1f; // Delay in seconds to appear later

    private void Awake()
    {
        SetAnimationValues(transform);
    }

    private void OnEnable()
    {
        SetAnimationValues(transform);
    }

    private void SetAnimationValues(Transform parent)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Speed", 1f / time);
            if (delayTime != 0f)
                animator.SetFloat("Delay", 1f / delayTime);
        }

        if (parent.childCount > 0)
        {
            foreach (Transform child in parent.transform)
            {
                FadingObject fadeComponent = GetComponent<FadingObject>();
                if (fadeComponent != null)
                {
                    fadeComponent.SetAnimationValues(child);
                }
            }
        }
    }    

    /// <summary>
    /// For the regular objects
    /// </summary>
    public void DisableOnComplete()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// For the transition objects
    /// </summary>
    public void ToTheLoadingScreen()
    {
        SceneManager.LoadScene(0);
    }
    
}
