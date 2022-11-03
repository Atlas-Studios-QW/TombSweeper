using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [Header("Objects")]
    public GameObject Player;
    public List<GameObject> UITags = new List<GameObject> ();

    [Header("-----Dev settings")]
    public float[] HexMovement = { 3.9f, 4.5f };
    [HideInInspector]
    private List<float[]> PositionCalc = new List<float[]> {
        new float[] {0.0f, 1.0f},
        new float[] {1.0f, 0.5f},
        new float[] {1.0f, -0.5f},
        new float[] {0.0f, -1.0f},
        new float[] {-1.0f, -0.5f},
        new float[] {-1.0f, 0.5f}
    };

    private bool CanMove = true;
    private int CurrentPos = 0;
    TextMeshProUGUI CurrentLabel;
    private List<Vector2> Positions = new List<Vector2> { new Vector2(0, 0) };

    private void Start()
    {
        foreach (float[] Calc in PositionCalc)
        {
            Positions.Add(new Vector2(Calc[0] * HexMovement[0], Calc[1] * HexMovement[1]));
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) ) && CanMove) { ChooseOption(); }
        if (Input.GetKeyDown(KeyCode.W)) { MovePlayer(0); }
        if (Input.GetKeyDown(KeyCode.E)) { MovePlayer(1); }
        if (Input.GetKeyDown(KeyCode.D)) { MovePlayer(2); }
        if (Input.GetKeyDown(KeyCode.S)) { MovePlayer(3); }
        if (Input.GetKeyDown(KeyCode.A)) { MovePlayer(4); }
        if (Input.GetKeyDown(KeyCode.Q)) { MovePlayer(5); }
        if (Input.GetKeyDown(KeyCode.Escape)) { StartCoroutine(ResetCamera()); }
    }

    public void ChooseOption()
    {
        if (CurrentPos == 1 && PlayerPrefs.HasKey("LatestSaveGame")) { SceneManager.LoadScene("Game"); }
        if (CurrentPos == 2) { StartCoroutine(ShowSaveMenu(true)); }
        if (CurrentPos == 6) { StartCoroutine(ShowSaveMenu(false)); }
        if (CurrentPos == 3) { SceneManager.LoadScene("Settings"); }
        if (CurrentPos == 4) { Application.Quit(); }
    }

    private IEnumerator ShowSaveMenu(bool ExistingSaves)
    {
        GameObject Camera = GameObject.Find("Main Camera");
        if (ExistingSaves) {
            while (Camera.transform.position.x < 15)
            {
                Camera.transform.position += new Vector3(Time.deltaTime * 10,0,0);
                yield return null;
            }
        }
        else
        {
            while (Camera.transform.position.x > -15)
            {
                Camera.transform.position -= new Vector3(Time.deltaTime * 10, 0, 0);
                yield return null;
            }
        }
    }
    private IEnumerator ResetCamera()
    {
        GameObject Camera = GameObject.Find("Main Camera");
        while (Camera.transform.position.x < 0)
        {
            Camera.transform.position += new Vector3(Time.deltaTime * 10, 0, 0);
            yield return null;
        }
        while (Camera.transform.position.x > 0)
        {
            Camera.transform.position -= new Vector3(Time.deltaTime * 10, 0, 0);
            yield return null;
        }
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
        if (Positions.Contains(NextPosition))
        {
            if (CurrentPos != 0)
            {
                if (!(CurrentPos == 1 && !PlayerPrefs.HasKey("LatestSaveGame")))
                {
                    CurrentLabel = GameObject.Find("Label" + CurrentPos).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI SelectHint = GameObject.Find("SelectHint").GetComponent<TextMeshProUGUI>();

                    while (CurrentLabel.color.a > 0.1f)
                    {
                        CurrentLabel.color -= new Color(0, 0, 0, 8 * Time.deltaTime);
                        SelectHint.color -= new Color(255, 255, 255, 8 * Time.deltaTime);
                        yield return null;
                    }

                    if (CurrentLabel.color.a < 0.1f)
                    {
                        CurrentLabel.color = new Color(255, 255, 255, 0.1f);
                        SelectHint.color = new Color(255, 255, 255, 0);
                    }
                }
            }

            while (Player.transform.position != NextPosition)
            {
                Player.transform.position = Vector3.MoveTowards(Player.transform.position, NextPosition, Time.deltaTime * 10);
                yield return null;
            }

            if (Player.transform.position == NextPosition)
            {
                if (Positions.IndexOf(NextPosition) == -1)
                {
                    float MinDistance = Mathf.Infinity;
                    foreach (Vector2 Hexagon in Positions)
                    {
                        float Distance = Vector2.Distance(Hexagon, NextPosition);
                        if (Distance < MinDistance)
                        {
                            MinDistance = Distance;
                            CurrentPos = Positions.IndexOf(Hexagon);
                        }
                    }
                }
                else
                {
                    CurrentPos = Positions.IndexOf(NextPosition);
                }
            }

            if (CurrentPos != 0)
            {
                if (!(CurrentPos == 1 && !PlayerPrefs.HasKey("LatestSaveGame")))
                {
                    CurrentLabel = GameObject.Find("Label" + CurrentPos).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI SelectHint = GameObject.Find("SelectHint").GetComponent<TextMeshProUGUI>();

                    while (CurrentLabel.color.a < 1)
                    {
                        CurrentLabel.color += new Color(255, 255, 255, 8 * Time.deltaTime);
                        SelectHint.color += new Color(255, 255, 255, 8 * Time.deltaTime);
                        yield return null;
                    }
                }
            }
        }

        CanMove = true;
    }
}