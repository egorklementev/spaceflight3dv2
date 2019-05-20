using UnityEngine;

public class EditorInput : MonoBehaviour
{

    [Header("Units' refs")]
    public EditorLogic lu;
    public EditorGraphics gu;
    public EditorParams pu;

    [HideInInspector]
    public bool wasSwap = false;

    private bool isGameOver = false;

    private void Start() { }

    private void Update()
    {   

        // TODO: implement input feature

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void SetGameOver()
    {
        isGameOver = true;
    }

}
