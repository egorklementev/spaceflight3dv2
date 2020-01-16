using UnityEngine;

public class MenuManager : MonoBehaviour {

	void Awake()
    {
        MusicManager.instance.Play("Menu theme");
    }

}
