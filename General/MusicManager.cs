using UnityEngine;

public class MusicManager : MonoBehaviour {

    public GameObject[] audioSources;

    public static MusicManager instance;

	void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Play(string musicName)
    {
        foreach (GameObject obj in audioSources)
        {
            if (obj.name.Equals(musicName))
            {
                if (!obj.GetComponent<AudioSource>().isPlaying)
                {
					AudioSource asrc = obj.GetComponent<AudioSource>();
					asrc.volume = GameDataManager.instance.generalData.musicVolume;
					asrc.Play();
                }
            }
            else
            {
                obj.GetComponent<AudioSource>().Stop();
            }
        }
    }

    public void Stop(string musicName)
    {
        foreach (GameObject obj in audioSources)
        {
            if (obj.name.Equals(musicName))
            {
                obj.GetComponent<AudioSource>().Stop();
                break;
            }
        }
    }

    public void PlaySound(string soundName)
    {
        foreach (GameObject obj in audioSources)
        {
            if (obj.name.Equals(soundName))
            {
				AudioSource asrc = obj.GetComponent<AudioSource>();
				asrc.volume = GameDataManager.instance.generalData.soundVolume;
				asrc.Play();
                break;
            }            
        }
    }

	public void UpdateVolume(float volume)
	{
        foreach (GameObject obj in audioSources)
        {
			if (obj.GetComponent<AudioSource>().isPlaying)
			{	
				obj.GetComponent<AudioSource>().volume = volume;
			}
        }
	}
}
