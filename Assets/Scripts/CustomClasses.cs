using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public string description;
    public Sprite icon;

    public Item(string Name, string Description, Sprite Icon)
    {
        name = Name;
        description = Description;
        icon = Icon;
    }
}

[System.Serializable]
public enum ContainState { Empty, Coin, Item, Key, Bomb }

[System.Serializable]
public class Room
{
    public Vector2 location;
    public ContainState contains;
    public int BombsFound;
    public bool Entered;

    public Room()
    {
        location = new Vector2();
        contains = ContainState.Empty;
        BombsFound = 0;
        Entered = false;
    }

    public Room(Vector2 Location, ContainState Contains, int bombsFound, bool entered)
    {
        location = Location;
        contains = Contains;
        BombsFound = bombsFound;
        Entered = entered;
    }
}

[System.Serializable]
public class IntData
{
    public int currentMove;
    public int currentHexID;
    public int totalCoins;
    public int counterFixObjective;
    public int itemTotal;
    public int itemFix;
    public int keyTotal;
    public int keyFix;
    public int difficulty;
    public int totalDeaths;

    public IntData()
    {
        currentMove = 0;
        currentHexID = 1;
        totalCoins = 0;
        counterFixObjective = 0;
        itemTotal = 0;
        itemFix = 0;
        keyTotal = 0;
        keyFix = 0;
        difficulty = 0;
        totalDeaths = 0;
    }
    public IntData(int CurrentMove, int CurrentHexID, int TotalCoins, int CounterFixObjective, int ItemTotal, int ItemFix, int KeyTotal, int KeyFix, int Difficulty, int TotalDeaths)
    {
        currentMove = CurrentMove;
        currentHexID = CurrentHexID;
        totalCoins = TotalCoins;
        counterFixObjective = CounterFixObjective;
        itemTotal = ItemTotal;
        itemFix = ItemFix;
        keyTotal = KeyTotal;
        keyFix = KeyFix;
        difficulty = Difficulty;
        totalDeaths = TotalDeaths;
    }
}

[System.Serializable]
public class SaveGameData
{
    public List<Room> rooms = new List<Room>();
    public List<Vector2> keyHex = new List<Vector2>();
    public List<Item> collectedItems = new List<Item>();
    public IntData intData;
    public bool enableSight;
    public bool hasCompass;
    public bool hasDetonator;
    public Vector2 playerPos;

    public SaveGameData()
    {
        rooms = new List<Room>();
        keyHex = new List<Vector2>();
        collectedItems = new List<Item>();
        intData = new IntData();
        enableSight = false;
        hasCompass = false;
        hasDetonator = false;
        playerPos = new Vector2(0,0);
    }

    public SaveGameData(List<Room> Rooms, List<Vector2> KeyHex, List<Item> CollectedItems, IntData IntData, bool EnableSight, bool HasCompass, bool HasDetonator, Vector2 PlayerPos)
    {
        rooms = Rooms;
        keyHex = KeyHex;
        collectedItems = CollectedItems;
        intData = IntData;
        enableSight = EnableSight;
        hasCompass = HasCompass;
        hasDetonator = HasDetonator;
        playerPos = PlayerPos;
    }
}