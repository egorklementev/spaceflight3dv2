using UnityEngine;

public class EditorInput : MonoBehaviour
{

    [Header("Units' refs")]
    public EditorLogic lu;
    public EditorGraphics gu;
    public EditorParams pu;

    [HideInInspector]
    public bool wasSwap = false;        

    private void Update()
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

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                if (hitInfo.transform.gameObject.tag == "Gem")
                {
                    gu.SelectColor(hitInfo.transform.gameObject);
                } else if (hitInfo.transform.gameObject.tag == "BonusBar")
                {
                    gu.SelectBonus(hitInfo.transform.gameObject);
                }
            }

        }
        if (!gu.IsWorking())
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (hit)
                {
                    if ((!lu.coloringMode && !lu.bonusingMode) && (hitInfo.transform.gameObject.tag == "Gem" || hitInfo.transform.gameObject.tag == "Untouchable"))
                    {
                        gu.SelectGem(hitInfo.transform.gameObject);
                    }
                    else if ((lu.coloringMode) && (hitInfo.transform.gameObject.tag == "Gem" || hitInfo.transform.gameObject.tag == "Untouchable"))
                    {
                        gu.ColorGem(hitInfo.transform.gameObject);
                    }
                    else if ((lu.bonusingMode) && (hitInfo.transform.gameObject.tag == "Gem" || hitInfo.transform.gameObject.tag == "Untouchable"))
                    {
                        gu.BonusGem(hitInfo.transform.gameObject);
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
    }    

}
