using UnityEngine;

public class SFButton : MonoBehaviour {

	public void PlayClickSound() {
		MusicManager.instance.PlaySound("Click sound");
	}

}
