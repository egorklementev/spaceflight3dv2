[System.Serializable]
public class GameData {

    // The number of the resources that a player owns
    public int metal; 
    public int fuel;
    public int energy;

    // The number of the resources upgrade level
    public int metalUpgrade;
    public int fuelUpgrade; 
    public int energyUpgrade; 

    public float autosaveTimer; // The time interval in which the game make autosavings

    public int planetsReached; // The number of the planets that are available to the player
    public int currentLevel; // The number of the next level of the current planet

    public int selectedRocket; // Rocket that is selected for the flight
    
}
