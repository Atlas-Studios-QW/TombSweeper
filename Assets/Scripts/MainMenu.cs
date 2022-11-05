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
    private bool InMenu;
    private bool CanMove = true;
    private bool LoadMenu = false;
    private int CurrentPos = 0;
    private int CurrentSelected = 1;
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
        if (!InMenu && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) ) && CanMove) { ChooseOption(); }
        if (!InMenu && Input.GetKeyDown(KeyCode.W)) { MovePlayer(0); }
        if (!InMenu && Input.GetKeyDown(KeyCode.E)) { MovePlayer(1); }
        if (!InMenu && Input.GetKeyDown(KeyCode.D)) { MovePlayer(2); }
        if (!InMenu && Input.GetKeyDown(KeyCode.S)) { MovePlayer(3); }
        if (!InMenu && Input.GetKeyDown(KeyCode.A)) { MovePlayer(4); }
        if (!InMenu && Input.GetKeyDown(KeyCode.Q)) { MovePlayer(5); }

        if (InMenu && Input.GetKeyDown(KeyCode.Escape)) { StartCoroutine(ResetCamera()); }
        if (InMenu && Input.GetKeyDown(KeyCode.W)) { StartCoroutine(SelectSave(false)); }
        if (InMenu && Input.GetKeyDown(KeyCode.S)) { StartCoroutine(SelectSave(true)); }
        if (InMenu && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && CanMove) { ChooseSaveGame(); }
    }

    private void ChooseOption()
    {
        if (CurrentPos == 1 && PlayerPrefs.HasKey("LatestSaveGame")) { SceneManager.LoadScene("Game"); }
        if (CurrentPos == 2) { StartCoroutine(ShowSaveMenu(true)); }
        if (CurrentPos == 6) { StartCoroutine(ShowSaveMenu(false)); }
        if (CurrentPos == 3) { SceneManager.LoadScene("Settings"); }
        if (CurrentPos == 4) { Application.Quit(); }
    }

    private void ChooseSaveGame()
    {
        if (LoadMenu)
        {
            if (GetComponent<SavegameSystem>().CheckSavegame(CurrentSelected))
            {
                PlayerPrefs.SetInt("LatestSaveGame", CurrentSelected);
                SceneManager.LoadScene("Game");
            }
        }
        else
        {
            PlayerPrefs.SetInt("LatestSaveGame", CurrentSelected + 3);
            SceneManager.LoadScene("Game");
        }
    }

    private IEnumerator SelectSave(bool Next)
    {
        if ((Next && CurrentSelected < 3) || (!Next && CurrentSelected > 1))
        {
            if (LoadMenu)
            {
                GameObject SaveLoad = GameObject.Find("SaveLoad");
                Transform SelectorLoad = SaveLoad.transform.Find("Selector");
                Transform SelectedLoad = SaveLoad.transform.Find("Label" + CurrentSelected);
                SelectedLoad.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 0.1f);
                if (Next)
                {
                    CurrentSelected++;
                }
                else
                {
                    CurrentSelected--;
                }

                SelectedLoad = SaveLoad.transform.Find("Label" + CurrentSelected);

                if (Next)
                {
                    while (SelectorLoad.position.y > SelectedLoad.position.y)
                    {
                        SelectorLoad.position -= new Vector3(0, Time.deltaTime * 20, 0);
                        yield return null;
                    }
                    SelectorLoad.position = new Vector3(SelectorLoad.position.x, SelectedLoad.position.y, 0);
                }
                else
                {
                    while (SelectorLoad.position.y < SelectedLoad.position.y)
                    {
                        SelectorLoad.position += new Vector3(0, Time.deltaTime * 20, 0);
                        yield return null;
                    }
                    SelectorLoad.position = new Vector3(SelectorLoad.position.x, SelectedLoad.position.y, 0);
                }
                SelectedLoad.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 1);
            }
            else
            {
                GameObject SaveNew = GameObject.Find("SaveNew");
                Transform SelectorNew = SaveNew.transform.Find("Selector");
                Transform SelectedNew = SaveNew.transform.Find("Label" + CurrentSelected);
                SelectedNew.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 0.1f);
                if (Next)
                {
                    CurrentSelected++;
                }
                else
                {
                    CurrentSelected--;
                }

                SelectedNew = SaveNew.transform.Find("Label" + CurrentSelected);

                if (Next)
                {
                    while (SelectorNew.position.y > SelectedNew.position.y)
                    {
                        SelectorNew.position -= new Vector3(0, Time.deltaTime * 20, 0);
                        yield return null;
                    }
                    SelectorNew.position = new Vector3(SelectorNew.position.x, SelectedNew.position.y, 0);
                }
                else
                {
                    while (SelectorNew.position.y < SelectedNew.position.y)
                    {
                        SelectorNew.position += new Vector3(0, Time.deltaTime * 20, 0);
                        yield return null;
                    }
                    SelectorNew.position = new Vector3(SelectorNew.position.x, SelectedNew.position.y, 0);
                }
                SelectedNew.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 1);
            }
        }
    }

    private IEnumerator ShowSaveMenu(bool ExistingSaves)
    {
        GameObject Camera = GameObject.Find("Main Camera");
        if (ExistingSaves) {
            while (Camera.transform.position.x < 15)
            {
                Camera.transform.position += new Vector3(Time.deltaTime * 20,0,0);
                yield return null;
            }
            LoadMenu = true;
        }
        else
        {
            while (Camera.transform.position.x > -15)
            {
                Camera.transform.position -= new Vector3(Time.deltaTime * 20, 0, 0);
                yield return null;
            }
            LoadMenu = false;
        }
        InMenu = true;
    }
    private IEnumerator ResetCamera()
    {
        GameObject Camera = GameObject.Find("Main Camera");
        while (Camera.transform.position.x < 0)
        {
            Camera.transform.position += new Vector3(Time.deltaTime * 20, 0, 0);
            yield return null;
        }
        while (Camera.transform.position.x > 0)
        {
            Camera.transform.position -= new Vector3(Time.deltaTime * 20, 0, 0);
            yield return null;
        }
        InMenu = false;
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
                    CurrentLabel = GameObject.Find("OptionLabels").transform.Find("Label" + CurrentPos).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI SelectHint = GameObject.Find("OptionLabels").transform.Find("SelectHint").GetComponent<TextMeshProUGUI>();

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
                    TextMeshProUGUI SelectHint = GameObject.Find("OptionLabels").transform.Find("SelectHint").GetComponent<TextMeshProUGUI>();

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