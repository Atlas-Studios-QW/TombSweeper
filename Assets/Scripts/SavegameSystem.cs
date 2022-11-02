using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavegameSystem : MonoBehaviour
{

    [Header("Save Items")]
    public GameObject HexagonParent;
    public GameObject ScriptHolder;

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

    public void SaveGame()
    {
        
    }

    public void LoadGame()
    {

    }
}
