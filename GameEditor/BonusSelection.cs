using UnityEngine;
using System.Collections;

public class BonusSelection : MonoBehaviour {

    public MessageText mt;
    public EditorParams pu;
    public GameObject[] bonusMeshes;
    public float scaleSpeed = 1f;

    [HideInInspector]
    public GameObject[] bonusGrid;
    [HideInInspector]
    public int currentSelected = -1;
    [HideInInspector]
    public bool isProcessing = false;

    private float normalScale;
    private readonly float boundParam = 0.7f;    

    public void CreatePanel(int[] bonuses, GameObject anchor)
    {
        bonusGrid = new GameObject[bonuses.Length + 2];

        anchor.transform.position = transform.position;

        float height = 2f * pu.mainCamera.orthographicSize;
        float width = height * pu.mainCamera.aspect;
        float gemSize = Mathf.Min(
            boundParam * height / (bonusGrid.Length + bonusGrid.Length * 0.2f),
            0.065f * width);
        
        for (int i = 0; i < 2; i++)
        {
            Vector3 pos = anchor.transform.position;
            pos.y += i * 1.25f * gemSize;

            bonusGrid[i] = Instantiate(bonusMeshes[i], anchor.transform);
            bonusGrid[i].transform.localScale = new Vector3(gemSize, gemSize, gemSize);
            bonusGrid[i].transform.position = pos;
        }

        int iter = 2;
        foreach (int i in bonuses)
        {
            Vector3 position = anchor.transform.position;
            position.y += iter * 1.25f * gemSize;

            bonusGrid[iter] = Instantiate(bonusMeshes[i], anchor.transform);
            bonusGrid[iter].transform.localScale = new Vector3(gemSize, gemSize, gemSize);            
            bonusGrid[iter].transform.position = position;

            iter++;
        }
        
        normalScale = gemSize;
    }

    public void ReCreatePanel(int[] bonuses, GameObject anchor)
    {
        foreach (GameObject obj in bonusGrid)
        {
            Destroy(obj);
        }

        CreatePanel(bonuses, anchor);
    }

    public void SelectBonus(int i)
    {
        if (currentSelected == -1)
        {
            StartCoroutine(ScaleGem(bonusGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
            switch(i)
            {
                case 0:
                    mt.DisplayMessage(pu.bonusNames[0], MessageText.ScreenPosition.TOP);
                    break;
                case 1:
                    mt.DisplayMessage(pu.bonusNames[1], MessageText.ScreenPosition.TOP);
                    break;
                default:
                    mt.DisplayMessage(pu.bonusNames[pu.permittedBonuses[i - 2]], MessageText.ScreenPosition.TOP);
                    break;
            }
            
        }
        else if (i != currentSelected)
        {
            StartCoroutine(ScaleGem(bonusGrid[currentSelected], new Vector3(normalScale, normalScale, normalScale), scaleSpeed));
            StartCoroutine(ScaleGem(bonusGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
            switch (i)
            {
                case 0:
                    mt.DisplayMessage(pu.bonusNames[0], MessageText.ScreenPosition.TOP);
                    break;
                case 1:
                    mt.DisplayMessage(pu.bonusNames[1], MessageText.ScreenPosition.TOP);
                    break;
                default:
                    mt.DisplayMessage(pu.bonusNames[pu.permittedBonuses[i - 2]], MessageText.ScreenPosition.TOP);
                    break;
            }
        } else
        {
            StartCoroutine(ScaleGem(bonusGrid[currentSelected], new Vector3(normalScale, normalScale, normalScale), scaleSpeed));
            currentSelected = -1;
        }
    }
    
    private IEnumerator ScaleGem(GameObject gem, Vector3 finalScale, float speed)
    {
        isProcessing = true;
        Vector3 currentScale = gem.transform.localScale;
        for (float t = 0; t < 1f; t += Time.deltaTime * speed)
        {
            gem.transform.localScale = Vector3.Lerp(currentScale, finalScale, t);
            yield return new WaitForEndOfFrame();
        }
        isProcessing = false;
    }

}
