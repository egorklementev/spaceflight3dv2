using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {

    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    private static bool isReady = false;
    private readonly string missingTextString = "null";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            Debug.Log("LOCALIZATION INITIALIZATION");

            if (!GetIsReady())
                LoadLocalizedText("lang" + Application.systemLanguage + ".json");
            //LoadLocalizedText("langEnglish.json");

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);        
    }

    public void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();

        string filePath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "Localization"), fileName);

        try
        {
            string dataAsJson;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    dataAsJson = File.ReadAllText(filePath);
                    break;
                case RuntimePlatform.Android:
                    WWW reader = new WWW(filePath);
                    while (!reader.isDone) { }
                    dataAsJson = reader.text;
                    break;
                default:
                    dataAsJson = File.ReadAllText(filePath);
                    break;
            }

            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            throw;
        }

        isReady = true;
    }

    public string GetLocalizedValue(string key)
    {
        while(!GetIsReady()) { }

        string result = missingTextString;
        if(localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }
        return result;
    }

    public bool GetIsReady()
    {
        return isReady;
    }
}
