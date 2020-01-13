using UnityEngine;

public class DataFile : MonoBehaviour {

    public int factId = 0;
    public FactAnimationController faController;

    public void ShowFact()
    {
        faController.ShowFact(factId);
    }

}
