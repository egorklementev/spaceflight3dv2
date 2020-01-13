using UnityEngine;

public class ComboText : MonoBehaviour {
    
    public void DisableOnAnimationEnd()
    {
        GetComponent<Animator>().SetBool("ShowComboMsg", false);
    }

}
