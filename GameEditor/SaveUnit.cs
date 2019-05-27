using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveUnit {

    public static void SaveLevel(EditorParams pu, EditorLogic lu, int slotNumber)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedLevel" + slotNumber.ToString() + ".edf";

        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

        LevelData ld = new LevelData(pu, lu);

        bf.Serialize(fs, ld);
        fs.Close();
    }

    public static LevelData LoadLevel(int slotNumber)
    {
        string path = Application.persistentDataPath + "/savedLevel" + slotNumber.ToString() + ".edf";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);

            LevelData ld = bf.Deserialize(fs) as LevelData;
            fs.Close();

            return ld;
        } else
        {
            Debug.LogError("File \"" + path + "\" does not exists!");
            return null;
        }
    }

}
