//for Cityscapes, copyright Cole Hilscher 2025

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem{
    //handles the saving and loading of the player's stored game data. References SaveData.cs

    static private string path = Application.persistentDataPath + "/save.fbg";

    public static void SaveGame() {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void LoadGame() {
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0) {
                stream.Close();
                FirstTimePlayingEver();
            }
            else {
                SaveData data = formatter.Deserialize(stream) as SaveData;
                stream.Close();
                data.LoadData();
            }
        }
        else
            FirstTimePlayingEver();
    }

    private static void FirstTimePlayingEver()
    {
        Debug.Log("First time playing ever!");
        //if this is the first time that the player has opened the game, load the default values for some staticVariables elements
        StaticVariables.highestBeatenStage = StaticVariables.GetStage(1, 0);
        StaticVariables.playerName = "REBECCA";
        StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Normal;
        SaveGame();
        LoadGame();
    }
}
