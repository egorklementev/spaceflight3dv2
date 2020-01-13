using UnityEngine;

public class FactAnimationController : MonoBehaviour {

    public GameObject[] factPrefabs;    

    private Animator factAnim;
    private int showedFactId = 0;

    void Awake()
    {
        factAnim = GetComponent<Animator>();
    }

    public void ShowFact(int factId)
    {
        factPrefabs[factId].SetActive(true);
        showedFactId = factId;
        factAnim.Play("Fact show");
    }

    public void HideFact()
    {
        factAnim.Play("Fact hide");
    }

    public void DisableFact()
    {
        factPrefabs[showedFactId].SetActive(false);
        showedFactId = -1;
    }

}
