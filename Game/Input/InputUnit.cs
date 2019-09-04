using UnityEngine;

/// <summary>
/// A class listening to the user input and transmitting it to the other units
/// </summary>
public class InputUnit : MonoBehaviour {

    [Header("Units' refs")]
    public LogicUnit lu;
    public GraphicsUnit gu;
    public ParamUnit pu;

    [HideInInspector]
    public bool wasSwap = false;

    private bool isGameOver = false; // Local variable to make sure that no input can be processed after game is over   
    
    private void Update()
    {
        if (gu.WorkingObjs == 0 && !isGameOver)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (lu.TwoSelected())
                {
                    gu.SwapGems(lu.gemSO, lu.gemST);
                    lu.SwapGems(lu.gemSO, lu.gemST);

                    gu.ResetSelection();
                    lu.ResetSelection();

                    wasSwap = true;
                }
                else
                {
                    gu.ResetSelection();
                    lu.ResetSelection();
                }
            }
            if (Input.GetMouseButton(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (hit)
                {
                    if (hitInfo.transform.gameObject.tag == "Gem")
                    {
                        gu.SelectGem(hitInfo.transform.gameObject);
                    }
                }
            }
            else
            {
                if (lu.OneSelected())
                {
                    gu.ResetSelection();
                    lu.ResetSelection();
                }
            }
        }

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
