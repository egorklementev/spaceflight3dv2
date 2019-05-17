using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveUnit {

    public static void SaveLevel(ParamUnit pu, LogicUnit lu)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedLevel.edf";

        FileStream fs = new FileStream(path, FileMode.Create);

        LevelData ld = new LevelData(pu, lu);

        bf.Serialize(fs, ld);
        fs.Close();
    }

    public static LevelData LoadLevel()
    {
        string path = Application.persistentDataPath + "/savedLevel.edf";
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
