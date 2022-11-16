using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private LevelBuilder levelBuilder;
    private GameObject Player;
    private GameObject PausePanel;
    private GameObject DeathPanel;
    private GameObject WinPanel;
    private float[] HexMovement;
    private List<float[]> PositionCalc;
    private bool CanMove = true;

    private void Start()
    {
        levelBuilder = GetComponent<LevelBuilder>();
        Player = levelBuilder.Player;
        PausePanel = levelBuilder.PausePanel;
        DeathPanel = levelBuilder.DeathPanel;
        WinPanel = levelBuilder.WinPanel;
        HexMovement = levelBuilder.HexMovement;
        PositionCalc = levelBuilder.PositionCalc;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GameObject ClickedRoomUI = levelBuilder.IsPointerOverUIElement("RoomInteract");
            if (ClickedRoomUI != null)
            {
                Transform ClickedRoom = ClickedRoomUI.transform.parent.parent.parent;
                float RoomDistance = Vector2.Distance(levelBuilder.SaveGame.playerPos, ClickedRoom.position);
                if (RoomDistance < HexMovement[1] * 1.25f && RoomDistance > 0.2f)
                {
                    Room Clicked = null;

                    float MinDistance = Mathf.Infinity;
                    foreach (Room Room in levelBuilder.SaveGame.rooms)
                    {
                        float Distance = Vector2.Distance(ClickedRoom.position, Room.location);
                        if (Distance < MinDistance)
                        {
                            MinDistance = Distance;
                            Clicked = Room;
                        }
                    }

                    if (Clicked != null)
                    {
                        int direction = -(int)Mathf.Round((Mathf.Atan2(Clicked.location.y - levelBuilder.SaveGame.playerPos.y, Clicked.location.x - levelBuilder.SaveGame.playerPos.x) * 180 / Mathf.PI - 90) / 60);
                        if (direction < 0)
                        {
                            direction += 6;
                        }
                        MovePlayer(direction);
                    }
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.W)) { MovePlayer(0); }
        if (Input.GetKeyDown(KeyCode.E)) { MovePlayer(1); }
        if (Input.GetKeyDown(KeyCode.D)) { MovePlayer(2); }
        if (Input.GetKeyDown(KeyCode.S)) { MovePlayer(3); }
        if (Input.GetKeyDown(KeyCode.A)) { MovePlayer(4); }
        if (Input.GetKeyDown(KeyCode.Q)) { MovePlayer(5); }
    }

    public void MovePlayer(int DirectionInput)
    {
        if (CanMove && !PausePanel.activeSelf && !DeathPanel.activeSelf && !WinPanel.activeSelf)
        {
            CanMove = false;
            StartCoroutine(Mover(DirectionInput));
        }
    }

    private IEnumerator Mover(int Direction)
    {
        Vector3 NextPosition = Player.transform.position + new Vector3(HexMovement[0] * PositionCalc[Direction][0], HexMovement[1] * PositionCalc[Direction][1], 0);

        while (Player.transform.position != NextPosition)
        {
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, NextPosition, Time.deltaTime * 10);
            yield return null;
        }

        if (Player.transform.position == NextPosition)
        {
            GetComponent<LevelBuilder>().LoadNewHex();
        }

        CanMove = true;
    }
}
