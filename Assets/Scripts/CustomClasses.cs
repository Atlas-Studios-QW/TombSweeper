using System.Collections;
using System.Collections.Generic;
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

    public Room(Vector2 Location, ContainState Contains, int bombsFound, bool entered)
    {
        location = Location;
        contains = Contains;
        BombsFound = bombsFound;
        Entered = entered;
    }
}

[System.Serializable]
public class SaveGameData
{
    public List<Room> rooms = new List<Room>();
    public List<Vector2> keyHex = new List<Vector2>();
    public List<Item> collectedItems = new List<Item>();
    public List<int> intData = new List<int>();
    public bool enableSight;
    public bool hasCompass;
    public Vector2 playerPos;

    public SaveGameData(List<Room> Rooms, List<Vector2> KeyHex, List<Item> CollectedItems, List<int> IntData, bool EnableSight, bool HasCompass, Vector2 PlayerPos)
    {
        rooms = Rooms;
        keyHex = KeyHex;
        collectedItems = CollectedItems;
        intData = IntData;
        enableSight = EnableSight;
        hasCompass = HasCompass;
        playerPos = PlayerPos;
    }
}