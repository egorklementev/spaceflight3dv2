using UnityEngine;
using System.Collections;

public class ColorSelection : MonoBehaviour {

    public GameObject gemMesh;
    public float scaleSpeed = 1f;

    [HideInInspector]
    public GameObject[] colorGrid;
    [HideInInspector]
    public int currentSelected = -1;
    [HideInInspector]
    public bool isProcessing = false;

    private float normalScale;

    public void CreatePanel(float gemSize, int[] colors)
    {
        colorGrid = new GameObject[colors.Length];

        float param = 1.25f;
        transform.localScale *= 45f * param;
        int iter = 0;
        foreach (int i in colors)
        {
            Vector3 position = transform.position;
            position.y += iter * 1.25f * gemSize * param;

            colorGrid[iter] = Instantiate(gemMesh, transform);
            colorGrid[iter].transform.localScale = new Vector3(gemSize, gemSize, gemSize);
            colorGrid[iter].GetComponent<Renderer>().material.color = EditorGraphics.colors[i];
            colorGrid[iter].transform.position = position;

            iter++;
        }

        normalScale = gemSize;
    }

    public void ReCreatePanel(float gemSize, int[] colors)
    {
        foreach (GameObject obj in colorGrid)
        {
            Destroy(obj);
        }

        colorGrid = new GameObject[colors.Length];
        
        int iter = 0;
        foreach (int i in colors)
        {
            Vector3 position = transform.position;
            position.y += iter * 1.25f * gemSize;

            colorGrid[iter] = Instantiate(gemMesh, transform);
            colorGrid[iter].transform.localScale = new Vector3(gemSize, gemSize, gemSize);
            colorGrid[iter].GetComponent<Renderer>().material.color = EditorGraphics.colors[i];
            colorGrid[iter].transform.position = position;

            iter++;
        }

        normalScale = .5f * gemSize;
    }

    public void SelectColor(int i)
    {
        if (currentSelected == -1)
        {
            StartCoroutine(ScaleGem(colorGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
        } else if (i != currentSelected)
        {
            StartCoroutine(ScaleGem(colorGrid[currentSelected], new Vector3(normalScale, normalScale, normalScale), scaleSpeed));
            StartCoroutine(ScaleGem(colorGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
        } else
        {
            StartCoroutine(ScaleGem(colorGrid[currentSelected], new Vector3(normalScale, normalScale, normalScale), scaleSpeed));
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
