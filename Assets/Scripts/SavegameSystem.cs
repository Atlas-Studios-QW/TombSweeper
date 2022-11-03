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
    }

    public bool CheckSavegame(int SaveGameNumber)
    {
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

    public void SaveGame()
    {
        SavegameFolder = Application.persistentDataPath + "/Savegames";
        LevelBuilder Script = GetComponent<LevelBuilder>();
        SaveGameData SaveGame = new SaveGameData(
            Script.Rooms,
            Script.KeyHex,
            Script.CollectedItems,
            new List<int> { 
                Script.CurrentMove,
                Script.CurrentHexID,
                Script.TotalCoins,
                Script.CounterFixObjective,
                Script.ItemTotal,
                Script.KeyTotal
            },
            Script.EnableSight,
            Script.HasCompass,
            Script.PlayerPos
        );

        int SaveGameNumber = PlayerPrefs.GetInt("LatestSaveGame");
        StreamReader reader = new StreamReader(File.OpenRead(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
        string fileContent = reader.ReadToEnd();
        reader.Close();
        fileContent = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(SaveGame)));
        StreamWriter writer = new StreamWriter(File.OpenWrite(SavegameFolder + $"/Save{SaveGameNumber}.txt"));
        writer.Write(fileContent);
        writer.Close();
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
        Script.Rooms = ReadData.rooms;
        Script.KeyHex = ReadData.keyHex;
        Script.CurrentMove = ReadData.intData[0];
        Script.CurrentHexID = ReadData.intData[1];
        Script.CounterFixObjective = ReadData.intData[3];
        Script.KeyTotal = ReadData.intData[5];
        Script.EnableSight = ReadData.enableSight;
        Script.HasCompass = ReadData.hasCompass;
        Script.PlayerPos = ReadData.playerPos;


        Script.CollectedItems = ReadData.collectedItems;
        foreach (Item Item in ReadData.collectedItems)
        {
            Script.Alert("Item");
        }
        Script.ItemTotal = ReadData.intData[4];

        Script.TotalCoins = ReadData.intData[2];
        Script.Alert("Coin");

        Script.Player.transform.position = Script.PlayerPos;

        GameObject RoomParent = Script.HexParent;
        int RoomID = 0;
        bool SkippedRoom = false;
        foreach (Room Room in ReadData.rooms)
        {
            print(Room.BombsFound);
            if (SkippedRoom)
            {
                GameObject NewRoom = Instantiate(Script.Hexagon, Room.location, new Quaternion(0, 0, 0, 0), RoomParent.transform);
                NewRoom.name = "Hexagon" + RoomID;
                if (Room.Entered)
                {
                    print("Test: " + Room.BombsFound);
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
