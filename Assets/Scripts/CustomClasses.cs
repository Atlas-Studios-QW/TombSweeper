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

public enum ContainState { Empty, Coin, Item, Key, Bomb }

public class Room
{
    public Vector2 location;
    public ContainState contains;
    public int BombsFound;

    public Room(Vector2 Location, ContainState Contains, int bombsFound)
    {
        location = Location;
        contains = Contains;
        BombsFound = bombsFound;
    }
}