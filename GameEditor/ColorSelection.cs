using UnityEngine;
using System.Collections;

public class ColorSelection : MonoBehaviour {

    public MessageText mt;
    public EditorParams pu;
    public GameObject gemMesh;
    public float scaleSpeed = 1f;

    [HideInInspector]
    public GameObject[] colorGrid;
    [HideInInspector]
    public int currentSelected = -1;
    [HideInInspector]
    public bool isProcessing = false;

    private float normalScale;
    private readonly float boundParam = 0.7f;    

    public void CreatePanel(int[] colors, GameObject anchor)
    {
        colorGrid = new GameObject[colors.Length];

        anchor.transform.position = transform.position;

        float height = 2f * pu.mainCamera.orthographicSize;
        float width = height * pu.mainCamera.aspect;
        float gemSize = Mathf.Min(
            boundParam * height / (colors.Length + colors.Length * 0.2f),
            0.065f * width);
        
        int iter = 0;
        foreach (int i in colors)
        {
            Vector3 position = anchor.transform.position;
            position.y += iter * 1.25f * gemSize;

            colorGrid[iter] = Instantiate(gemMesh, anchor.transform);
            colorGrid[iter].transform.localScale = new Vector3(gemSize, gemSize, gemSize);
            colorGrid[iter].GetComponent<Renderer>().material.color = EditorGraphics.colors[i];
            colorGrid[iter].transform.position = position;

            iter++;
        }

        normalScale = gemSize;
    }

    public void ReCreatePanel(int[] colors, GameObject anchor)
    {
        foreach (GameObject obj in colorGrid)
        {
            Destroy(obj);
        }

        CreatePanel(colors, anchor);
    }

    public void SelectColor(int i)
    {
        if (currentSelected == -1)
        {
            StartCoroutine(ScaleGem(colorGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
            mt.DisplayMessage(pu.colorNames[pu.colorVector[i]], MessageText.ScreenPosition.TOP);
        }
        else if (i != currentSelected)
        {
            StartCoroutine(ScaleGem(colorGrid[currentSelected], new Vector3(normalScale, normalScale, normalScale), scaleSpeed));
            StartCoroutine(ScaleGem(colorGrid[i], new Vector3(1.25f * normalScale, 1.25f * normalScale, 1.25f * normalScale), scaleSpeed));
            currentSelected = i;
            mt.DisplayMessage(pu.colorNames[pu.colorVector[i]], MessageText.ScreenPosition.TOP);
        }
        else
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
