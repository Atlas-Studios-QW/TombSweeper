using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class SavegameSystem : MonoBehaviour
{
    private string SavegameFolder = "";
    private void Start()
    {
        SavegameFolder = Application.persistentDataPath + "/Savegames";
        if (!Directory.Exists(SavegameFolder)) { 
            Directory.CreateDirectory(SavegameFolder);
            File.Create(SavegameFolder + "/Save1.txt");
            File.Create(SavegameFolder + "/Save2.txt");
            File.Create(SavegameFolder + "/Save3.txt");
        }
        if (PlayerPrefs.GetInt("AutoSaveTime") > 0)
        {
            StartCoroutine(AutoSave(PlayerPrefs.GetInt("AutoSaveTime") * 60));
        }
    }

    public bool CheckSavegame(int SaveGameNumber)
    {
        if (!File.Exists(SavegameFolder + $"/Save{SaveGameNumber}.txt"))
        {
            File.Create(SavegameFolder + $"/Save{SaveGameNumber}.txt");
        }
        string fileContent = File.ReadAllText(SavegameFolder + $"/Save{SaveGameNumber}.txt");
        if (fileContent.ToCharArray().Length > 0)
        { return true; }
        else { return false; }
    }

    IEnumerator AutoSave(int Interval)
    {
        yield return new WaitForSeconds(Interval);
        SaveGame();
    }

    public void SaveGame()
    {
        if (PlayerPrefs.GetInt("LatestSaveGame") > 3)
        {
            PlayerPrefs.SetInt("LatestSaveGame", PlayerPrefs.GetInt("LatestSaveGame") - 3);
        }
        SavegameFolder = Application.persistentDataPath + "/Savegames";
        LevelBuilder Script = GetComponent<LevelBuilder>();
        SaveGameData SaveGame = Script.SaveGame;

        int SaveGameNumber = PlayerPrefs.GetInt("LatestSaveGame");
        CheckSavegame(SaveGameNumber);
        string fileContent = Encryption.Encrypt(JsonUtility.ToJson(SaveGame));
        print("SAVING: " + fileContent);
        File.WriteAllText(SavegameFolder + $"/Save{SaveGameNumber}.txt", fileContent);
        Script.Alert("Saved Game!");
    }

    public void LoadGame()
    {
        SavegameFolder = Application.persistentDataPath + "/Savegames";
        int SaveGameNumber = PlayerPrefs.GetInt("LatestSaveGame");
        string ReadJson = Encryption.Decrypt(File.ReadAllText(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
        SaveGameData ReadData = JsonUtility.FromJson<SaveGameData>(ReadJson);

        LevelBuilder Script = GetComponent<LevelBuilder>();

        Script.SaveGame.collectedItems = ReadData.collectedItems;

        foreach (Item Item in ReadData.collectedItems)
        {
            Script.Alert("Item");
        }
        Script.Alert("Coin");

        Script.SaveGame = ReadData;

        Script.Player.transform.position = ReadData.playerPos;

        GameObject RoomParent = Script.HexParent;
        int RoomID = 0;
        bool SkippedRoom = false;
        foreach (Room Room in ReadData.rooms)
        {
            if (SkippedRoom)
            {
                GameObject NewRoom = Instantiate(Script.Hexagon, Room.location, new Quaternion(0, 0, 0, 0), RoomParent.transform);
                NewRoom.name = "Hexagon" + RoomID;
                if (Room.Entered)
                {
                    NewRoom.transform.Find("Canvas").Find("BombAmount").GetComponent<TextMeshProUGUI>().text = "" + Room.BombsFound;
                    NewRoom.transform.Find("Canvas").Find("Marker").gameObject.SetActive(false);
                }
            }
            else
            {
                SkippedRoom = true;
            }
            RoomID++;
        }
    }

    public void FixedValues(int NewDifficulty, int TotalDeaths)
    {
        int SaveGameNumber = PlayerPrefs.GetInt("LatestSaveGame");
        if (SaveGameNumber < 4)
        {
            SavegameFolder = Application.persistentDataPath + "/Savegames";
            string ReadJson = Encryption.Decrypt(File.ReadAllText(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
            SaveGameData ReadData = JsonUtility.FromJson<SaveGameData>(ReadJson);

            ReadData.intData.difficulty = NewDifficulty;
            ReadData.intData.totalDeaths = TotalDeaths;

            CheckSavegame(SaveGameNumber);
            File.WriteAllText(SavegameFolder + $"/Save{SaveGameNumber}.txt", Encryption.Encrypt(JsonUtility.ToJson(ReadData)));
        }
    }

    public void ResetSavegames()
    {
        File.Delete(SavegameFolder + "/Save1.txt");
        File.Delete(SavegameFolder + "/Save2.txt");
        File.Delete(SavegameFolder + "/Save3.txt");
        File.Create(SavegameFolder + "/Save1.txt");
        File.Create(SavegameFolder + "/Save2.txt");
        File.Create(SavegameFolder + "/Save3.txt");
    }
}
