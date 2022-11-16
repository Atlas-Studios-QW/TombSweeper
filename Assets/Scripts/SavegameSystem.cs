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
        StreamReader reader = new StreamReader(File.OpenRead(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
        string fileContent = reader.ReadToEnd();
        reader.Close();
        if (fileContent.ToCharArray().Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
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
        StreamReader reader = new StreamReader(File.OpenRead(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
        string fileContent = reader.ReadToEnd();
        reader.Close();
        fileContent = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(SaveGame)));
        StreamWriter writer = new StreamWriter(File.OpenWrite(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
        writer.Write(fileContent);
        writer.Close();
        Script.Alert("Saved Game!");
    }

    public void LoadGame()
    {
        SavegameFolder = Application.persistentDataPath + "/Savegames";
        int SaveGameNumber = PlayerPrefs.GetInt("LatestSaveGame");
        StreamReader reader = new StreamReader(File.OpenRead(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
        string ReadJson = reader.ReadToEnd();
        SaveGameData ReadData = JsonUtility.FromJson<SaveGameData>(System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(ReadJson)));

        reader.Close();

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
}
