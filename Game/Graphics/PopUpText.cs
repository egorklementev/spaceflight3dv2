using UnityEngine;

public class PopUpText : MonoBehaviour {

    void DisableOnAnimationEnd()
    {
        gameObject.SetActive(false);
        Destroy(gameObject.transform.parent.gameObject);
        Destroy(gameObject);
    }

}
