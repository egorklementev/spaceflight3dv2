using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	public Animator settingsAnim;
	[Space(10)]
	
	public Button[] menuButtons;
	public Slider soundSlider;
	public Slider musicSlider;

	private bool settingsShown = false;

	void Start()
	{
		soundSlider.onValueChanged.AddListener (delegate {UpdateSoundVolume();});
		musicSlider.onValueChanged.AddListener (delegate {UpdateMusicVolume();});
		soundSlider.value = GameDataManager.instance.generalData.soundVolume;
		musicSlider.value = GameDataManager.instance.generalData.musicVolume;
	}

	void Awake()
    {
        MusicManager.instance.Play("Menu theme");
    }

	public void TriggerSettings() {
		settingsShown = !settingsShown;

		settingsAnim.SetBool("Shown", settingsShown);

		foreach (Button b in menuButtons) {
			b.interactable = !settingsShown;
		}
	}

	private void UpdateSoundVolume() {
		GameDataManager.instance.generalData.soundVolume = soundSlider.value;
	}

	private void UpdateMusicVolume() {
		GameDataManager.instance.generalData.musicVolume = musicSlider.value;
		MusicManager.instance.UpdateVolume(musicSlider.value);
	}
}
