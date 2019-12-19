using UnityEngine;

public class FinalMessage : MonoBehaviour {

    public FadeManager fm;

    public void ToTheResultScreen()
    {
        if(GameDataManager.instance.generalData.selectedRocket == -1)
        {
            fm.SetLevel(3); // To the editor screen
        }
        else
        {
            fm.SetLevel(7); // To the result screen
        }
    }

}
