using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public GameObject HeartAlive;
    public GameObject HeartDead;
    public GameObject HeartsParent;

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetDeathHearts(int CurrentHearts)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject NewHeart;
            if (i < CurrentHearts)
            {
                NewHeart = Instantiate(HeartAlive, HeartsParent.transform);
            }
            else
            {
                NewHeart = Instantiate(HeartDead, HeartsParent.transform);
            }

            NewHeart.transform.position = new Vector2(Screen.width/2 - 250 + 125 * i, Screen.height / 2 - 50);
        }
    }
}
