using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSwitcher : MonoBehaviour {

	public void SwitchFade()
    {
        foreach(FadeManager fm in GetComponentsInChildren<FadeManager>())
        {
            fm.SwitchFade();
        }
    }
	
}
