using UnityEngine;
using UnityEngine.UI;

public class FactAnimationController : MonoBehaviour {

    public GameObject[] factPrefabs;
    public Transform dataFilesParent;

    private Button[] dataFiles;
    private Animator factAnim;
    private int showedFactId = 0;

    void Awake()
    {
        factAnim = GetComponent<Animator>();
        dataFiles = new Button[GameDataManager.instance.generalData.unlockedFacts];
        for (int i = 0; i < dataFiles.Length; i++)
        {
            dataFiles[i] = dataFilesParent.Find("Data file (" + i.ToString() + ")").GetComponent<Button>();
        }
    }

    public void ShowFact(int factId)
    {
        foreach (GameObject obj in factPrefabs)
        {
            obj.SetActive(false);
        }
        factPrefabs[factId].SetActive(true);        
        showedFactId = factId;
        factAnim.Play("Fact show");

        // Disable datafile buttons
        for (int i = 0; i < dataFiles.Length; i++)
        {
            dataFiles[i].interactable = false;
        }
    }

    public void HideFact()
    {
        factAnim.Play("Fact hide");
    }

    public void DisableFact()
    {
        factPrefabs[showedFactId].SetActive(false);
        showedFactId = -1;

        // Enable datafile buttons
        for (int i = 0; i < dataFiles.Length; i++)
        {
            dataFiles[i].interactable = true;
        }
    }

}
