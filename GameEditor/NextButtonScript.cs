using UnityEngine;

public class NextButtonScript : MonoBehaviour {

    public EditorUIUnit ui;

    public void DisablePreOptions()
    {
        ui.HidePreOptions();
        ui.HideLoadingOptions();
        ui.HideSaveOptions();
    }
}
