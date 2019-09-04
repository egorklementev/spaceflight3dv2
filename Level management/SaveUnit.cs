using UnityEngine;
using System.IO;

/// <summary>
/// A class that loads from and saves to files levels using given parameter and logic units and slot number
/// </summary>
public static class SaveUnit {

    public static void SaveLevel(EditorParams pu, EditorLogic lu, int slotNumber)
    {
        string filepath = Application.streamingAssetsPath + "/Levels/Editor/level_" + slotNumber.ToString() + ".json";
        
        LevelData ld = new LevelData(pu, lu);

        string jsonData = JsonUtility.ToJson(ld);
        
        File.WriteAllText(filepath, jsonData);
    }

    public static LevelData LoadLevel(int slotNumber)
    {
        string dataAsJson;
        string filepath = Application.streamingAssetsPath + "/Levels/Editor/level_" + slotNumber.ToString() + ".json";        
            
        try
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    dataAsJson = File.ReadAllText(filepath);
                    break;
                case RuntimePlatform.Android:
                    WWW reader = new WWW(filepath);
                    while (!reader.isDone) { }
                    dataAsJson = reader.text;
                    break;
                default:
                    dataAsJson = File.ReadAllText(filepath);
                    break;
            }
        }
        catch (System.Exception)
        {
            throw;
        }

        LevelData ld = JsonUtility.FromJson<LevelData>(dataAsJson);

        return ld;        
    }

}
