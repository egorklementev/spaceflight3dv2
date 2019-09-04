using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour {

    public string localeKey = "key";
    public bool useKeyAsValue = false;

    private void Awake()
    {
        if (useKeyAsValue)
        {
            GetComponent<TextMeshProUGUI>().text = localeKey;
        } else
        {
            GetComponent<TextMeshProUGUI>().text = LocalizationManager.instance.GetLocalizedValue(localeKey);
        }
    }

}
