using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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

public class LevelBuilder : MonoBehaviour
{
    [Header("-----Prefabs")]
    public GameObject Room;
    public GameObject IconPrefab;

    [Header("-----Objects")]
    public GameObject Player;
    public GameObject RoomParent;
    public GameObject AlertBox;
    public GameObject CoinBox;
    public GameObject ItemBox;
    public GameObject ItemDetails;
    public GameObject ObjectiveBox;
    public GameObject PausePanel;
    public GameObject DeathPanel;

    [Header("-----Sprites")]
    public Sprite CoinImg;
    public Sprite ItemImg;
    public Sprite KeyImg;

    [Header("-----Items")]
    public GameObject IconParent;
    public List<Item> Items = new List<Item>();
    public List<Sprite> KeySprites = new List<Sprite>();
    public GameObject CompassMarker;

    [Header("-----Settings")]
    public int BombChance = 25;
    public int CoinChance = 5;
    public int ItemChance = 1;

    [Header("-----Dev settings")]
    public bool EnableBombSight = false;
    public float[] HexMovement = { 3.9f, 4.5f };

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
    private List<int> CoinRooms = new List<int>();
    private List<int> ItemRooms = new List<int>();
    private List<int> KeyRoomsID = new List<int>();
    private List<Vector2> KeyRooms = new List<Vector2>(); 

    private List<Item> CollectedItems = new List<Item>();

    private int CurrentMove = 0;
    private int CurrentRoomID = 1;
    private int TotalCoins = 0;
    private int CounterFixObjective = 0;
    private int ItemTotal = 0;
    private int KeyTotal = 0;
    private bool EnableSight = false;
    private bool HasCompass = false;

    private void Start()
    {
        LoadNewRooms();
        ChoosePositions();
        Alert("Objective:\nFind the key pieces [0/3]");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel.SetActive(!PausePanel.activeSelf);
        }

        string ItemName = IsPointerOverUIElement();
        if (ItemName != null)
        {
            Item CurrentItem = null;
            foreach (Item item in Items)
            {
                if (item.name == ItemName)
                {
                    CurrentItem = item;
                }
            }

            ItemDetails.transform.position = Input.mousePosition;
            ItemDetails.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = ItemName;
            ItemDetails.transform.Find("Details").GetComponent<TextMeshProUGUI>().text = CurrentItem.description;
        }
        else
        {
            ItemDetails.transform.position = new Vector2(-1000,0);
        }
    }

    public void ResumeGame()
    {
        PausePanel.SetActive(false);
    }

    public void ChoosePositions()
    {
        for (int i = 1; i < 5; i++)
        {
            float NewKeyLocationX = Random.Range(-10 * i, 10 * i) * HexMovement[0];
            float NewKeyLocationY = Random.Range(-10 * i, 10 * i) * HexMovement[1];
            KeyRooms.Add(new Vector2(NewKeyLocationX, NewKeyLocationY));
            //print(new Vector2(NewKeyLocationX, NewKeyLocationY));
        }
    }

    public void LoadNewRooms()
    {
        Vector2 PlayerPos = Player.transform.position;
        
        int CurrentRoom = 0;
        bool Dead = false;

        if (HasCompass && CurrentMove % 3 == 0)
        {
            StartCoroutine(CompassBlink());
        }

        if (Rooms.IndexOf(PlayerPos) == -1)
        {
            //print("Current position: " + PlayerPos);
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
            //print("Current fixed room: " + CurrentRoom);
        }
        else
        {
            CurrentRoom = Rooms.IndexOf(PlayerPos);
            //print("Current room: " + CurrentRoom);
        }

        GameObject RoomObject = GameObject.Find("Room" + CurrentRoom);
        RoomObject.transform.Find("Canvas").Find("Marker").gameObject.SetActive(false);

        if (BombRooms.Contains(CurrentRoom))
        {
            EndGame();
            Dead = true;
        }
        else if (KeyRoomsID.Contains(CurrentRoom))
        {
            GameObject.Find("Room" + CurrentRoom).transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            KeyRoomsID.RemoveAt(KeyRoomsID.IndexOf(CurrentRoom));
            KeyTotal++;
            if (KeyTotal == 3)
            {
                Alert($"Objective:\nFind the exit");
            }
            else
            {
                Alert($"Objective:\nFind the key pieces [{KeyTotal}/3]");
            }
        }
        else if (CoinRooms.Contains(CurrentRoom))
        {
            GameObject.Find("Room" + CurrentRoom).transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(0,0,0,0);
            CoinRooms.RemoveAt(CoinRooms.IndexOf(CurrentRoom));
            TotalCoins++;
            Alert("Coin");
        }
        else if (ItemRooms.Contains(CurrentRoom))
        {
            GameObject.Find("Room" + CurrentRoom).transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            ItemRooms.RemoveAt(ItemRooms.IndexOf(CurrentRoom));
            if (ItemTotal < 3)
            {
                CollectedItems.Add(Items[ItemTotal]);
                Alert("Item");
                Alert("You collected an item!");

                if (ItemTotal == 1)
                {
                    EnableSight = true;
                }
                else if (ItemTotal == 2)
                {
                    HasCompass = true;
                }
                else if (ItemTotal == 3)
                {
                    
                }
            }
        }

        int NewBombs = 0;
        int BombsFound = 0;
        bool KeyRoomFound = false;

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

                    bool SpawnBomb = false;

                    if (CurrentRoomID <= 6) { SpawnBomb = false; }
                    else { SpawnBomb = Random.Range(0, 100) > (100 - BombChance); }

                    float KeyMinDistance = Mathf.Infinity;
                    foreach (Vector2 KeyRoom in KeyRooms)
                    {
                        float Distance = Vector2.Distance(KeyRoom, PlayerPos);
                        if (Distance < KeyMinDistance)
                        {
                            KeyMinDistance = Distance;
                        }
                    }
                    //print(KeyMinDistance);

                    if (KeyMinDistance < 3.0f && !KeyRoomFound)
                    {
                        KeyRoomFound = true;
                        NewRoom.transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                        NewRoom.transform.Find("Canvas").Find("Icon").GetComponent<Image>().sprite = KeyImg;
                        KeyRoomsID.Add(CurrentRoomID);
                    }
                    else if (SpawnBomb && BombsFound != 3)
                    {
                        NewBombs++;
                        BombsFound++;
                        BombRooms.Add(CurrentRoomID);
                        if (EnableBombSight)
                        {
                            NewRoom.GetComponent<SpriteRenderer>().material.color = new Color(255, 0, 0);
                        }
                        //print("Bomb Added: " + CurrentRoomID);
                    }
                    else if (Random.Range(0,100) > (100 - CoinChance))
                    {
                        CoinRooms.Add(CurrentRoomID);
                        if (EnableSight)
                        {
                            NewRoom.transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                            NewRoom.transform.Find("Canvas").Find("Icon").GetComponent<Image>().sprite = CoinImg;
                        }
                    }
                    else if (Random.Range(0, 100) > (100 - ItemChance) && ItemTotal < 3)
                    {
                        ItemRooms.Add(CurrentRoomID);
                        if (EnableSight)
                        {
                            NewRoom.transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                            NewRoom.transform.Find("Canvas").Find("Icon").GetComponent<Image>().sprite = ItemImg;
                        }
                    }

                    CurrentRoomID++;
                }
                else
                {
                    int ExistingRoom = Rooms.IndexOf(NewRoomPosition);

                    //print("Checking: " + ExistingRoom + " ---- At: " + NewRoomPosition);
                    if (BombRooms.Contains(ExistingRoom))
                    {
                        //print("Found: " + ExistingRoom);
                        BombsFound++;
                    }
                }
            }

            GameObject.Find("Room" + CurrentRoom).transform.Find("Canvas").Find("BombAmount").GetComponent<TextMeshProUGUI>().text = BombsFound.ToString();

            //print("Found: " + BombsFound);

        }

        //print("Next move ======================================================");

        CurrentMove++;
    }

    public void Alert(string Message)
    {
        StartCoroutine(Alerter(Message));
    }

    private IEnumerator CompassBlink()
    {
        CompassMarker.transform.rotation = new Quaternion(0,0,,0);

        Image Marker = CompassMarker.transform.Find("Marker").GetComponent<Image>();
        for (int i = 0; i < 4; i++)
        {
            while (Marker.color.a < 1.0f)
            {
                Marker.color += new Color(0, 0, 0, 0.025f);
                yield return null;
            }
            while (Marker.color.a > 0.0f)
            {
                Marker.color -= new Color(0, 0, 0, 0.025f);
                yield return null;
            }
        }
    }

    private IEnumerator Alerter(string Message)
    {
        Transform UIItems = CoinBox.transform.Find("Padding");

        if (Message.Contains("Objective:"))
        {
            ObjectiveBox.GetComponent<TextMeshProUGUI>().text = Message;
            CounterFixObjective++;
            while (ObjectiveBox.transform.position.x < 590)
            {
                ObjectiveBox.transform.position += new Vector3(20, 0, 0);
                yield return null;
            }

            yield return new WaitForSeconds(2);
            CounterFixObjective--;

            if (CounterFixObjective == 0)
            {
                while (ObjectiveBox.transform.position.x > - 610)
                {
                    ObjectiveBox.transform.position -= new Vector3(20, 0, 0);
                    yield return null;
                }
            }
        }
        else if (Message == "Coin")
        {
            UIItems.Find("CoinCounter").GetComponent<TextMeshProUGUI>().text = "<sprite index=0> " + TotalCoins.ToString("000");
        }
        else if (Message == "Item")
        {
            if (ItemTotal == 0)
            {
                ItemBox.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 50);
            }
            ItemBox.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 150);
            GameObject NewIcon = Instantiate(IconPrefab, IconParent.transform);
            NewIcon.transform.position += new Vector3(0, 150 * ItemTotal, 0);
            NewIcon.transform.Find("Icon").GetComponent<Image>().sprite = CollectedItems[ItemTotal].icon;
            NewIcon.transform.Find("Icon").name = CollectedItems[ItemTotal].name;
            ItemTotal++;
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
        DeathPanel.SetActive(true);
    }

    public string IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private string IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.tag == "ItemUI")
                return curRaysastResult.gameObject.name;
        }
        return null;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}