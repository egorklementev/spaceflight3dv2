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
                    obj.GetComponent<AudioSource>().Play();               
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
                obj.GetComponent<AudioSource>().Play();
                break;
            }            
        }
    }
}
