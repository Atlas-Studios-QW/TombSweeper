using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject Room;

    [Header("Objects")]
    public GameObject Player;
    public GameObject RoomParent;
    public GameObject AlertBox;
    public GameObject CoinCounter;

    [Header("Settings")]
    public float[] HexMovement = { 3.9f, 4.5f };
    public int BombChance = 25;
    public int CoinChance = 5;
    public List<Item> Items = new List<Item>();

    [HideInInspector]
    public List<float[]> PositionCalc = new List<float[]> {
        new float[] {0.0f, 1.0f},
        new float[] {1.0f, 0.5f},
        new float[] {1.0f, -0.5f},
        new float[] {0.0f, -1.0f},
        new float[] {-1.0f, -0.5f},
        new float[] {-1.0f, 0.5f}
    };

    private List<Vector2> Rooms = new List<Vector2> {new Vector2(0,0)};
    private List<int> BombRooms = new List<int>();
    private List<int> ItemRooms = new List<int>();

    private int CurrentRoomID = 1;
    private int TotalCoins = 0;

    private void Start()
    {
        LoadNewRooms();
    }

    public void LoadNewRooms()
    {
        Vector2 PlayerPos = Player.transform.position;
        
        int CurrentRoom = 0;
        bool Dead = false;

        if (Rooms.IndexOf(PlayerPos) == -1)
        {
            print("Current position: " + PlayerPos);
            float MinDistance = Mathf.Infinity;
            foreach (Vector2 Room in Rooms)
            {
                float Distance = Vector2.Distance(Room, PlayerPos);
                if (Distance < MinDistance)
                {
                    MinDistance = Distance;
                    CurrentRoom = Rooms.IndexOf(Room);
                }
            }
            print("Current fixed room: " + CurrentRoom);
        }
        else
        {
            CurrentRoom = Rooms.IndexOf(PlayerPos);
            print("Current room: " + CurrentRoom);
        }

        GameObject RoomObject = GameObject.Find("Room" + CurrentRoom);
        RoomObject.transform.Find("Canvas").Find("Marker").gameObject.SetActive(false);

        if (BombRooms.Contains(CurrentRoom))
        {
            EndGame();
            Alert("BOOM");
            Dead = true;
        }
        else if (ItemRooms.Contains(CurrentRoom))
        {
            ItemRooms.RemoveAt(ItemRooms.IndexOf(CurrentRoom));
            TotalCoins++;
            CoinCounter.GetComponent<TextMeshProUGUI>().text = "Coins: " + TotalCoins;
            Alert("Found a coin!");
        }

        int NewBombs = 0;
        int BombsFound = 0;

        if (!Dead)
        {
            foreach (float[] PosChange in PositionCalc)
            {
                Vector2 NewRoomPosition = new Vector2(Mathf.Round((PlayerPos.x + (PosChange[0] * HexMovement[0])) * 100) / 100, Mathf.Round((PlayerPos.y + (PosChange[1] * HexMovement[1])) * 100) / 100);

                if (!Rooms.Contains(NewRoomPosition))
                {
                    GameObject NewRoom = Instantiate(Room, NewRoomPosition, new Quaternion(0,0,0,0), RoomParent.transform);
                    Rooms.Add(NewRoomPosition);
                    NewRoom.name = "Room" + CurrentRoomID;

                    if (Random.Range(0,100) > (100 - BombChance) && BombsFound != 3)
                    {
                        NewBombs++;
                        BombsFound++;
                        BombRooms.Add(CurrentRoomID);
                        //NewRoom.GetComponent<SpriteRenderer>().material.color = new Color(255, 0, 0);
                        print("Bomb Added: " + CurrentRoomID);
                    }
                    else if (Random.Range(0,100) > (100 - CoinChance))
                    {
                        ItemRooms.Add(CurrentRoomID);
                        //NewRoom.GetComponent<SpriteRenderer>().material.color = new Color(0,255,0);
                    }

                    CurrentRoomID++;
                }
                else
                {
                    int ExistingRoom = Rooms.IndexOf(NewRoomPosition);

                    print("Checking: " + ExistingRoom + " ---- At: " + NewRoomPosition);
                    if (BombRooms.Contains(ExistingRoom))
                    {
                        print("Found: " + ExistingRoom);
                        BombsFound++;
                    }
                }
            }

            GameObject.Find("Room" + CurrentRoom).transform.Find("Canvas").Find("BombAmount").GetComponent<TextMeshProUGUI>().text = BombsFound.ToString();

            print("Found: " + BombsFound);

        }

        print("Next move ======================================================");
    }

    public void Alert(string Message)
    {
        StartCoroutine(Alerter(Message));
    }

    private IEnumerator Alerter(string Message)
    {
        if (Message == "Coin")
        {

        } 
        else
        {
            AlertBox.GetComponent<TextMeshProUGUI>().text = Message;
            yield return new WaitForSeconds(1);
            AlertBox.GetComponent<TextMeshProUGUI>().text = "";
        }

    }

    private void EndGame()
    {
        SceneManager.LoadScene("Menu");
    }
}

public class Item
{
    public string Name;
    public int Chance;
    public Sprite Image;
    public Item(string name, int chance, Sprite image)
    {
        Name = name;
        Chance = chance;
        Image = image;
    }
}