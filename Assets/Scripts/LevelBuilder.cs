using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using JetBrains.Annotations;

public class LevelBuilder : MonoBehaviour
{
    [Header("-----Prefabs")]
    public GameObject Hexagon;
    public GameObject IconPrefab;

    [Header("-----Objects")]
    public GameObject Player;
    public GameObject HexParent;
    public GameObject AlertBox;
    public GameObject CoinBox;
    public GameObject ItemBox;
    public GameObject ItemDetails;
    public GameObject ObjectiveBox;
    public GameObject PausePanel;
    public GameObject DeathPanel;
    public GameObject WinPanel;
    public GameObject ParticlesParent;
    public GameObject MainCamera;

    [Header("-----Particles")]
    public GameObject Explosion;

    [Header("-----Sprites")]
    public Sprite CoinImg;
    public Sprite ItemImg;
    public Sprite BombImg;
    public List<Sprite> KeySprites = new List<Sprite>();

    [Header("-----Items")]
    public GameObject IconParent;
    public List<Item> Items = new List<Item>();
    public GameObject CompassMarker;

    [Header("-----Settings")]
    public float CameraSpeed = 8;
    public int BombDifficulty = 30;
    public int CoinChance = 8;
    public int ItemChance = 4;

    [Header("-----Dev settings")]
    public bool EnableBombSight = false;
    public float[] HexMovement = { 3.9f, 4.5f };

    [HideInInspector]
    public SaveGameData SaveGame = new SaveGameData();
    public List<float[]> PositionCalc = new List<float[]> {
        new float[] {0.0f, 1.0f},
        new float[] {1.0f, 0.5f},
        new float[] {1.0f, -0.5f},
        new float[] {0.0f, -1.0f},
        new float[] {-1.0f, -0.5f},
        new float[] {-1.0f, 0.5f}
    };
    private bool CanUseDetonator = true;

    private void Start()
    {
        SaveGame.intData.difficulty = BombDifficulty;

        SaveGame.rooms.Add(new Room(new Vector2(0, 0), ContainState.Empty, 0, true));
        if (PlayerPrefs.GetInt("LatestSaveGame") < 4)
        {
            GetComponent<SavegameSystem>().LoadGame();
            print("Lowered Difficulty: " + SaveGame.intData.difficulty);
        }
        ChoosePositions();
        LoadNewHex();
        if (SaveGame.intData.keyTotal < 3)
        {
            Alert($"Objective:\nFind the key pieces [{SaveGame.intData.keyTotal}/3]");
        }
        else
        {
            Alert($"Objective:\nFind the exit");
        }
    }

    private void Update()
    {
        MainCamera.transform.position = Vector3.MoveTowards(MainCamera.transform.position, Player.transform.position + new Vector3(0, -3, -10), CameraSpeed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.Escape) && !DeathPanel.activeInHierarchy)
        {
            PausePanel.SetActive(!PausePanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            UseDetonator();
        }

        GameObject HoveredItem = IsPointerOverUIElement("ItemUI");
        if (HoveredItem != null)
        {
            string ItemName = HoveredItem.name;
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
            SaveGame.keyHex.Add(new Vector2(NewKeyLocationX, NewKeyLocationY));
            //print(new Vector2(NewKeyLocationX, NewKeyLocationY));
        }
    }

    public void LoadNewHex()
    {
        SaveGame.playerPos = Player.transform.position;

        bool addDifficulty = Random.Range(0, 100) > (100 - (BombDifficulty / 5));
        if (addDifficulty && SaveGame.intData.difficulty < BombDifficulty * 1.8 && SaveGame.intData.difficulty < 80)
        {
            SaveGame.intData.difficulty++;
            print("New Difficulty: "+SaveGame.intData.difficulty);
        }

        Room CurrentRoom = null;
        bool Dead = false;

        if (SaveGame.hasCompass && SaveGame.intData.currentMove % 8 == 0)
        {
            StartCoroutine(CompassBlink());
        }

        foreach (Room Room in SaveGame.rooms)
        {    
            if (Room.location == SaveGame.playerPos)
            {
                CurrentRoom = Room;   
            }    
        }            
        
        if (CurrentRoom == null)
        {
            float MinDistance = Mathf.Infinity;
            foreach (Room Room in SaveGame.rooms)
            {
                float Distance = Vector2.Distance(Room.location, SaveGame.playerPos);
                if (Distance < MinDistance)
                {
                    MinDistance = Distance;
                    CurrentRoom = Room;
                    //print("Dist: " + Distance);
                }
            }
        }


        CurrentRoom.Entered = true;
        GameObject HexObject = GameObject.Find("Hexagon" + SaveGame.rooms.IndexOf(CurrentRoom));
        HexObject.transform.Find("Canvas").Find("Marker").gameObject.SetActive(false);

        if (CurrentRoom.contains == ContainState.Bomb)
        {
            EndGame();
            Dead = true;
        }
        else if (CurrentRoom.contains == ContainState.Key)
        {
            CurrentRoom.contains = ContainState.Empty;
            SaveGame.intData.keyTotal++;
            if (SaveGame.intData.keyTotal == 4)
            {
                WinPanel.SetActive(true);
            }
            else if (SaveGame.intData.keyTotal == 3)
            {
                Alert($"Objective:\nFind the exit");
            }
            else
            {
                Alert($"Objective:\nFind the key pieces [{SaveGame.intData.keyTotal}/3]");
            }
        }
        else if (CurrentRoom.contains == ContainState.Coin)
        {
            CurrentRoom.contains = ContainState.Empty;
            SaveGame.intData.totalCoins++;
            Alert("Coin");
        }
        else if (CurrentRoom.contains == ContainState.Item)
        {
            CurrentRoom.contains = ContainState.Empty;
            if (SaveGame.intData.itemTotal < 3)
            {
                SaveGame.collectedItems.Add(Items[SaveGame.intData.itemTotal]);
                Alert("Item");
                Alert("You collected an item!");

                if (SaveGame.intData.itemTotal == 1)
                {
                    SaveGame.enableSight = true;
                }
                else if (SaveGame.intData.itemTotal == 2)
                {
                    SaveGame.hasCompass = true;
                }
                else if (SaveGame.intData.itemTotal == 3)
                {
                    SaveGame.hasDetonator = true;
                }
            }
        }

        GameObject.Find("Hexagon" + SaveGame.rooms.IndexOf(CurrentRoom)).transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(0, 0, 0, 0);

        int NewBombs = 0;
        int BombsFound = 0;
        bool KeyHexFound = false;
        bool ItemAdded = true;

        if (!Dead)
        {
            foreach (float[] PosChange in PositionCalc)
            {
                Vector2 NewHexPosition = new Vector2(Mathf.Round((SaveGame.playerPos.x + (PosChange[0] * HexMovement[0])) * 100) / 100, Mathf.Round((SaveGame.playerPos.y + (PosChange[1] * HexMovement[1])) * 100) / 100);
                Room NewRoom = new Room(NewHexPosition, ContainState.Empty, 0, false);

                foreach (Room Room in SaveGame.rooms)
                {
                    if (Room.location == NewHexPosition)
                    {
                        NewRoom = null;
                    }
                }

                if (NewRoom != null)
                {
                    GameObject NewHex = Instantiate(Hexagon, NewHexPosition, new Quaternion(0,0,0,0), HexParent.transform);
                    SaveGame.rooms.Add(NewRoom);
                    NewHex.name = "Hexagon" + SaveGame.intData.currentHexID;

                    bool SpawnBomb = false;

                    if (SaveGame.intData.currentHexID <= 6) { SpawnBomb = false; }
                    else { SpawnBomb = Random.Range(0, 100) > (100 - SaveGame.intData.difficulty); }

                    float KeyMinDistance = Mathf.Infinity;
                    if (SaveGame.intData.keyFix < 4)
                    {
                        float Distance = Vector2.Distance(SaveGame.keyHex[SaveGame.intData.keyFix], SaveGame.playerPos);
                        if (Distance < KeyMinDistance)
                        {
                            KeyMinDistance = Distance;
                        }
                    }

                    if (KeyMinDistance < 5.0f && !KeyHexFound)
                    {
                        KeyHexFound = true;
                        if (SaveGame.enableSight)
                        {
                            NewHex.transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                            NewHex.transform.Find("Canvas").Find("Icon").GetComponent<Image>().sprite = KeySprites[SaveGame.intData.keyFix];
                        }
                        SaveGame.intData.keyFix++;
                        NewRoom.contains = ContainState.Key;
                    }
                    else if (SpawnBomb && BombsFound != 3)
                    {
                        NewBombs++;
                        BombsFound++;
                        NewRoom.contains = ContainState.Bomb;
                        if (EnableBombSight)
                        {
                            NewHex.GetComponent<SpriteRenderer>().material.color = new Color(255, 0, 0);
                        }
                    }
                    else if (Random.Range(0,100) > (100 - CoinChance))
                    {
                        NewRoom.contains = ContainState.Coin;
                        if (SaveGame.enableSight)
                        {
                            NewHex.transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                            NewHex.transform.Find("Canvas").Find("Icon").GetComponent<Image>().sprite = CoinImg;
                        }
                    }
                    else if (ItemAdded && Random.Range(0, 100) > (100 - ItemChance) && SaveGame.intData.itemTotal < 3 && SaveGame.intData.currentMove > (SaveGame.intData.itemFix * 20 + 20))
                    {
                        ItemAdded = false;
                        SaveGame.intData.itemFix++;
                        NewRoom.contains = ContainState.Item;
                        if (SaveGame.enableSight)
                        {
                            NewHex.transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                            NewHex.transform.Find("Canvas").Find("Icon").GetComponent<Image>().sprite = ItemImg;
                        }
                    }

                    SaveGame.intData.currentHexID++;

                }
                else
                {
                    foreach (Room Room in SaveGame.rooms) { 
                        if (Room.location == NewHexPosition && Room.contains == ContainState.Bomb) 
                        {
                            BombsFound++;
                        }
                    }
                }
            }

            CurrentRoom.BombsFound = BombsFound;
            GameObject.Find("Hexagon" + SaveGame.rooms.IndexOf(CurrentRoom)).transform.Find("Canvas").Find("BombAmount").GetComponent<TextMeshProUGUI>().text = BombsFound.ToString();
            //print(SaveGame.rooms.Count);
            //print(CurrentRoom.location);
        }

        SaveGame.intData.currentMove++;
    }

    private IEnumerator CompassBlink()
    {
        Vector2 NextKeyPos = SaveGame.keyHex[SaveGame.intData.keyTotal];

        float direction = Mathf.Atan2(NextKeyPos.y - SaveGame.playerPos.y, NextKeyPos.x - SaveGame.playerPos.x) * 180 / Mathf.PI - 90;
        CompassMarker.transform.rotation = Quaternion.Euler(0,0,direction);

        Image Marker = CompassMarker.transform.Find("Marker").GetComponent<Image>();
        for (int i = 0; i < 4; i++)
        {
            while (Marker.color.a < 1.0f)
            {
                Marker.color += new Color(0, 0, 0, Time.deltaTime * 10);
                yield return null;
            }
            while (Marker.color.a > 0.0f)
            {
                Marker.color -= new Color(0, 0, 0, Time.deltaTime * 10);
                yield return null;
            }
        }
    }

    public void Alert(string Message)
    {
        StartCoroutine(Alerter(Message));
    }

    private IEnumerator Alerter(string Message)
    {
        if (Message.Contains("Objective:"))
        {
            ObjectiveBox.GetComponent<TextMeshProUGUI>().text = Message;
            SaveGame.intData.counterFixObjective++;
            while (ObjectiveBox.transform.position.x < -30)
            {
                ObjectiveBox.transform.position += new Vector3(500 * Time.deltaTime, 0, 0);
                yield return null;
            }

            yield return new WaitForSeconds(2);
            SaveGame.intData.counterFixObjective--;

            if (SaveGame.intData.counterFixObjective == 0)
            {
                while (ObjectiveBox.transform.position.x > -Screen.width * 0.601)
                {
                    ObjectiveBox.transform.position -= new Vector3(500 * Time.deltaTime, 0, 0);
                    yield return null;
                }
            }
        }
        else if (Message == "Coin")
        {
            Transform UIItems = CoinBox.transform.Find("Padding");
            UIItems.Find("CoinCounter").GetComponent<TextMeshProUGUI>().text = "<sprite index=0> " + SaveGame.intData.totalCoins.ToString("000");
        }
        else if (Message == "Item")
        {
            if (SaveGame.intData.itemTotal == 0)
            {
                ItemBox.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 12.5f);
            }
            ItemBox.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 87.5f);
            GameObject NewIcon = Instantiate(IconPrefab, IconParent.transform);
            NewIcon.transform.position += new Vector3(0, 150 * SaveGame.intData.itemTotal, 0);
            NewIcon.transform.Find("Icon").GetComponent<Image>().sprite = SaveGame.collectedItems[SaveGame.intData.itemTotal].icon;
            NewIcon.transform.Find("Icon").parent.name = SaveGame.collectedItems[SaveGame.intData.itemTotal].name;
            NewIcon.transform.Find("Icon").name = SaveGame.collectedItems[SaveGame.intData.itemTotal].name;
            SaveGame.intData.itemTotal++;
            if (SaveGame.intData.itemTotal == 3)
            {
                IconParent.transform.Find("Detonator").Find("Button").GetComponent<Button>().onClick.AddListener(UseDetonator);
                print("Added Listener");
            }
        }
        else
        {
            AlertBox.GetComponent<TextMeshProUGUI>().text = Message;
            yield return new WaitForSeconds(1);
            AlertBox.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void UseDetonator()
    {
        print("Detonator Used");
        if (SaveGame.hasDetonator && CanUseDetonator)
        {
            StartCoroutine(Detonator());
        }
    }

    private IEnumerator Detonator()
    {
        CanUseDetonator = false;

        Room ClosestBombRoom = new Room();
        float MinDistance = Mathf.Infinity;
        foreach (Room Room in SaveGame.rooms)
        {
            if (Room.contains == ContainState.Bomb)
            {
                float Distance = Vector2.Distance(SaveGame.playerPos, Room.location);
                if (Distance < MinDistance)
                {
                    MinDistance = Distance;
                    ClosestBombRoom = Room;
                }
            }
        }

        GameObject ExplosionAnimation = Instantiate(Explosion);
        ExplosionAnimation.transform.parent = ParticlesParent.transform;
        ExplosionAnimation.transform.position = ClosestBombRoom.location;
        ClosestBombRoom.contains = ContainState.Empty;

        Transform RoomCanvas = GameObject.Find("Hexagon" + SaveGame.rooms.IndexOf(ClosestBombRoom)).transform.Find("Canvas");
        RoomCanvas.Find("Icon").GetComponent<Image>().sprite = BombImg;
        RoomCanvas.Find("Icon").GetComponent<Image>().color = new Color(255,255,255,1);
        RoomCanvas.Find("Marker").gameObject.SetActive(false);

        TextMeshProUGUI Timer = IconParent.transform.Find("Detonator").Find("ItemTimer").GetComponent<TextMeshProUGUI>();

        int Time = 30;
        Timer.text = Time + "";
        while (Time > 0)
        {
            Time--;
            Timer.text = Time + "";
            yield return new WaitForSeconds(1);
        }
        Timer.text = "";
        CanUseDetonator = true;
    }

    private void EndGame()
    {
        int NewDifficulty = (int) Mathf.Floor((SaveGame.intData.difficulty + BombDifficulty)/2);
        SaveGame.intData.totalDeaths++;
        GetComponent<SavegameSystem>().FixedValues(NewDifficulty, SaveGame.intData.totalDeaths);
        GetComponent<Menus>().SetDeathHearts(5-SaveGame.intData.totalDeaths);
        DeathPanel.SetActive(true);
    }

    public GameObject IsPointerOverUIElement(string TagName)
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults(), TagName);
    }

    private GameObject IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, string TagName)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.tag == TagName)
                return curRaysastResult.gameObject;
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