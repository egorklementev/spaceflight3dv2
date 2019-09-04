using UnityEngine;
using System.Collections.Generic;

public class FadeManager : MonoBehaviour {
   
    public GameObject transitionFade;
    [Space(10)]
    public GameObject[] fadeGroups;

    private Dictionary<string, GameObject> dict;

    private void Awake()
    {
        dict = new Dictionary<string, GameObject>();

        for (int i = 0; i < fadeGroups.Length; i++)
        {
            dict.Add(fadeGroups[i].name, fadeGroups[i]);
        }
    }

    public void SetLevel(int levelIndex)
    {
        Loading.levelToLoad = levelIndex;
        transitionFade.SetActive(true);
        transitionFade.GetComponent<Animator>().Play("Transition showing");
    }

    public void ShowGroup(string groupName)
    {
        ActivateChildrenRecursively(dict[groupName].transform);

        foreach (Animator childAnim in dict[groupName].GetComponentsInChildren<Animator>())
        {
            childAnim.Play("Delay");
        }
    }

    public void HideGroup(string groupName)
    {
        foreach (Animator childAnim in dict[groupName].GetComponentsInChildren<Animator>())
        {
            childAnim.Play("Hiding");
        }
    }

    private void ActivateChildrenRecursively(Transform parent)
    {
        parent.gameObject.SetActive(true);

        if (parent.childCount > 0)
        {
            foreach (Transform child in parent.transform)
            {
                ActivateChildrenRecursively(child);
            }
        }
    }

}
