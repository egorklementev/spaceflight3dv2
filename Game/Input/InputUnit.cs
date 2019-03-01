using UnityEngine;

public class InputUnit : MonoBehaviour {

    [Header("Units' refs")]
    public LogicUnit lu;
    public GraphicsUnit gu;
    public ParamUnit pu;

    [HideInInspector]
    public bool wasSwap = false;

    private void Start()
    {
    }

    private void Update()
    {
        if (gu.WorkingObjs == 0)
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
}
