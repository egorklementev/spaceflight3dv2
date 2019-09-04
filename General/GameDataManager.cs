using UnityEngine;
using System.IO;

public class GameDataManager : MonoBehaviour {

    public GameData generalData;
    public RocketData[] rocketData;
    [Space(10)]

    public int resPerStorageUpg = 50;
    [Space(10)]

    public int rocketNumber = 6;
    public int maxRocketUpgradeLevel = 5;
    [Space(10)]

    [Header("Upgrade params")]
    public int resPerEngineUpg = 50;
    public float buffPerEngineUpg = 5f;
    [Space(5)]

    public int resPerSheathingUpg = 60;
    public float buffPerSheathingUpg = 2.5f;
    [Space(5)]

    public int resPerFrameUpg = 70;
    public float buffPerFrameUpg = 3.5f;
    [Space(10)]

    public static GameDataManager instance;

    private float autosaveTimer;

    private void Awake()
    {
        rocketData = new RocketData[rocketNumber];

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);

        autosaveTimer = generalData.autosaveTimer;

        generalData = LoadDataFile<GameData>("/game_data.json");

        for (int i = 0; i < rocketNumber; i++)
        {
            rocketData[i] = LoadDataFile<RocketData>("/RocketData/rocket_" + (i + 1).ToString() + ".json");
        }
    }

    private void Update()
    {
        // DEBUG
        DebugResourcesIncreaser();

        // Autosaving
        if (autosaveTimer < 0f)
        {
            SaveToDataFile("/game_data.json", generalData);

            for (int i = 0; i < rocketNumber; i++)
            {
                SaveToDataFile("/RocketData/rocket_" + (i + 1).ToString() + ".json", rocketData[i]);
            }

            autosaveTimer = generalData.autosaveTimer;            
        }
        autosaveTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Loads a data from the specified file
    /// </summary>
    /// <typeparam name="T">Type of the object to be returned</typeparam>
    /// <param name="fileName">Path to the data file starting from streaming assets</param>
    /// <returns>Data object loaded from the streaming assets</returns>
    public T LoadDataFile<T>(string fileName)
    {
        string jsonData;
        string filePath = Application.streamingAssetsPath + fileName;

        try
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    jsonData = File.ReadAllText(filePath);
                    break;
                case RuntimePlatform.Android:
                    WWW reader = new WWW(filePath);
                    while (!reader.isDone) { }
                    jsonData = reader.text;
                    break;
                default:
                    jsonData = File.ReadAllText(filePath);
                    break;
            }
        }
        catch (System.Exception)
        {
            throw;
        }

        T dataObj = JsonUtility.FromJson<T>(jsonData);

        return dataObj;
    }

    /// <summary>
    /// Saves data object to the file in the streaming assets
    /// </summary>
    /// <typeparam name="T">Type of the object to be saved</typeparam>
    /// <param name="fileName">Path to the file starting from the streaming assets</param>
    /// <param name="objToSave">Object to be saved</param>
    public void SaveToDataFile<T>(string fileName, T objToSave)
    {
        string filePath = Application.streamingAssetsPath + fileName;

        string jsonData = JsonUtility.ToJson(objToSave);

        File.WriteAllText(filePath, jsonData);
    }
    
    /// <summary>
    /// Function for addition of energy in the data. In case if old_energy + energy is bigger than max_energy, energy amount will be assign to the max_energy.
    /// </summary>
    /// <param name="energy">Number of energy to be added</param>
    /// <returns>False if old_energy + energy exceeds max_energy, true otherwise</returns>
    public bool AddEnergy(int energy)
    {
        if (generalData.energy + energy <= generalData.energyUpgrade * resPerStorageUpg)
        {
            generalData.energy += energy;
            return true;
        }
        else
        {
            generalData.energy = generalData.energyUpgrade * resPerStorageUpg;
            return false;
        }
    }

    /// <summary>
    /// Function for consuming of energy in the data. In case if old_energy - energy is less than zero, nothing happens.
    /// </summary>
    /// <param name="energy">Number of energy to be consumed</param>
    /// <returns>False if old_energy - energy is less than zero, true otherwise</returns>
    public bool ConsumeEnergy(int energy)
    {
        if (generalData.energy - energy >= 0)
        {
            generalData.energy -= energy;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Function for addition of metal in the data. In case if old_metal + metal is bigger than max_metal, energy amount will be assign to the max_metal.
    /// </summary>
    /// <param name="metal">Number of metal to be added</param>
    /// <returns>False if old_metal + metal exceeds max_metal, true otherwise</returns>
    public bool AddMetal(int metal)
    {
        if (generalData.metal + metal <= generalData.metalUpgrade * resPerStorageUpg)
        {
            generalData.metal += metal;
            return true;
        }
        else
        {
            generalData.metal = generalData.metalUpgrade * resPerStorageUpg;
            return false;
        }
    }

    /// <summary>
    /// Function for consuming of metal in the data. In case if old_metal - metal is less than zero, nothing happens.
    /// </summary>
    /// <param name="metal">Number of metal to be consumed</param>
    /// <returns>False if old_metal - metal is less than zero, true otherwise</returns>
    public bool ConsumeMetal(int metal)
    {
        if (generalData.metal - metal >= 0)
        {
            generalData.metal -= metal;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Function for addition of fuel in the data. In case if old_fuel + fuel is bigger than max_fuel, fuel amount will be assign to the max_fuel.
    /// </summary>
    /// <param name="fuel">Number of fuel to be added</param>
    /// <returns>False if old_fuel + fuel exceeds max_fuel, true otherwise</returns>
    public bool AddFuel(int fuel)
    {
        if (generalData.fuel + fuel <= generalData.fuelUpgrade * resPerStorageUpg)
        {
            generalData.fuel += fuel;
            return true;
        }
        else
        {
            generalData.fuel = generalData.fuelUpgrade * resPerStorageUpg;
            return false;
        }
    }

    /// <summary>
    /// Function for consuming of fuel in the data. In case if old_fuel - fuel is less than zero, nothing happens.
    /// </summary>
    /// <param name="fuel">Number of fuel to be consumed</param>
    /// <returns>False if old_fuel - fuel is less than zero, true otherwise</returns>
    public bool ConsumeFuel(int fuel)
    {
        if (generalData.fuel - fuel >= 0)
        {
            generalData.fuel -= fuel;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private void DebugResourcesIncreaser()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            int addition = generalData.energyUpgrade * resPerStorageUpg / 4;
            AddEnergy(addition);
            addition = generalData.metalUpgrade * resPerStorageUpg / 4;
            AddMetal(addition);
            addition = generalData.fuelUpgrade * resPerStorageUpg / 4;
            AddFuel(addition);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            generalData.energyUpgrade++;
            generalData.metalUpgrade++;
            generalData.fuelUpgrade++;
        }
    }

}
