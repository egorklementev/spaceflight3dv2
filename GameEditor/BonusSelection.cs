using UnityEngine;
using System.Collections;

public class BonusSelection : MonoBehaviour {

    public GameObject[] bonusMeshes;
    public float scaleSpeed = 1f;

    [HideInInspector]
    public GameObject[] bonusGrid;
    [HideInInspector]
    public int currentSelected = -1;
    [HideInInspector]
    public bool isProcessing = false;

    private float normalScale;

    public void CreatePanel(float gemSize, int[] bonuses)
    {
        bonusGrid = new GameObject[bonuses.Length];

        float param = 1.3f;
        transform.localScale *= 45f * param;
        int iter = 0;
        foreach (int i in bonuses)
        {
            Vector3 position = transform.position;
            position.y += iter * 1.25f * gemSize * param;

            bonusGrid[iter] = Instantiate(bonusMeshes[i - 1], transform);
            bonusGrid[iter].transform.localScale = new Vector3(gemSize, gemSize, gemSize);            
            bonusGrid[iter].transform.position = position;

            iter++;
        }

        normalScale = gemSize;
    }

    public void ReCreatePanel(float gemSize, int[] bonuses)
    {
        foreach (GameObject obj in bonusGrid)
        {
            Destroy(obj);
        }

        bonusGrid = new GameObject[bonuses.Length];
        
        int iter = 0;
        foreach (int i in bonuses)
        {
            Vector3 position = transform.position;
            position.y += iter * 1.25f * gemSize;

            bonusGrid[iter] = Instantiate(bonusMeshes[i], transform);
            bonusGrid[iter].transform.localScale = new Vector3(gemSize, gemSize, gemSize);
            bonusGrid[iter].GetComponent<Renderer>().material.color = EditorGraphics.colors[i];
            bonusGrid[iter].transform.position = position;

            iter++;
        }

        normalScale = .5f * gemSize;
    }

    public void SelectBonus(int i)
    {
        if (currentSelected == -1)
        {
            StartCoroutine(ScaleGem(bonusGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
        } else if (i != currentSelected)
        {
            StartCoroutine(ScaleGem(bonusGrid[currentSelected], new Vector3(normalScale, normalScale, normalScale), scaleSpeed));
            StartCoroutine(ScaleGem(bonusGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
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
