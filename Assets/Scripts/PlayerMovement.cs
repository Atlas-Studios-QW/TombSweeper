using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private GameObject Player;
    private float[] HexMovement;
    private List<float[]> PositionCalc;
    private bool CanMove = true;

    private void Start()
    {
        Player = GetComponent<LevelBuilder>().Player;
        HexMovement = GetComponent<LevelBuilder>().HexMovement;
        PositionCalc = GetComponent<LevelBuilder>().PositionCalc;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) { MovePlayer(0); }
        if (Input.GetKeyDown(KeyCode.E)) { MovePlayer(1); }
        if (Input.GetKeyDown(KeyCode.D)) { MovePlayer(2); }
        if (Input.GetKeyDown(KeyCode.S)) { MovePlayer(3); }
        if (Input.GetKeyDown(KeyCode.A)) { MovePlayer(4); }
        if (Input.GetKeyDown(KeyCode.Q)) { MovePlayer(5); }
    }

    public void MovePlayer(int DirectionInput)
    {
        if (CanMove)
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
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, NextPosition, 0.3f);
            yield return null;
        }

        if (Player.transform.position == NextPosition)
        {
            GetComponent<LevelBuilder>().LoadNewRooms();
        }

        CanMove = true;
    }
}
