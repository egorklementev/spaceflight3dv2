using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject logo;
    public GameObject logoAnchor;

    private void Awake()
    {
        logo.transform.position = logoAnchor.transform.position;
    }

    public void QuitTheGame()
    {
        Application.Quit();
    }

}
