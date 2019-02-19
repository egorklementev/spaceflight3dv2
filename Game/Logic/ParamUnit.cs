using UnityEngine;
using System.Collections.Generic;

public class ParamUnit : MonoBehaviour {

    [Header("Grid params")]
    public Vector2 gridSize;    
    public float gemOffsetParam = .3f;
    public float gemMoveTime = 1f;
    public float gemScaleSpeed = 1f;
    public float destructionForce = 10f;
    public float dPartsLifetime = 3f;
    public Vector2 screenBound;
    [Space(10)]

    [Header("Gameplay params")]
    [Range(3, 8)]
    public int colorsAvailable = 5;
    [Range(3, 5)]
    public int sequenceSize = 3;
    public bool randomizeColors = false;
    [Space(10)]

    [Header("Bonus params")]
    [Range(0, 100)]
    public int bonusesPercentage;
    public int bonusesNumber;
    public float meteorMoveSpeed = 1f;
    public float meteorOffset = 10f;
    [Space(10)]

    [Header("Units' refs")]
    public GraphicsUnit gu;
    public LogicUnit lu;

    [HideInInspector]
    public float gemOffset;
    [HideInInspector]
    public float gemSize;

    private int[] colorVector;

    private void Start()
    {
        #region Color vector for gem colors
        if (randomizeColors)
        {
            colorVector = new int[colorsAvailable];
            List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < colorsAvailable; i++)
            {
                int index = Random.Range(0, list.Count);
                colorVector[i] = list[index];
                list.RemoveAt(index);
            }
        } else
        {
            colorVector = new int[colorsAvailable];
            for (int i = 0; i < colorsAvailable; i++)
            {
                colorVector[i] = i;
            }
        }
        #endregion

        // Size of the gems sides
        float pixelsInUnit = Screen.height / 10f; // Size of the camera is 5
        gemSize = Mathf.Min(
            screenBound.x * Screen.width / (gridSize.x + (gridSize.x - 1) * gemOffsetParam),
            screenBound.y * Screen.height / (gridSize.y + (gridSize.y - 1) * gemOffsetParam)
            );
        gemSize /= pixelsInUnit;
        gemOffset = gemOffsetParam * gemSize;

        gu.gameObject.SetActive(true);
        lu.gameObject.SetActive(true);
    }
        
    public int GetRandomColor()
    {
        return colorVector[Random.Range(0, colorsAvailable)];
    }

    public int GetRandomBonus()
    {
        return Random.Range(0, 100) < bonusesPercentage ? 
            Random.Range(1, bonusesNumber + 1) : -1;
    }

}
