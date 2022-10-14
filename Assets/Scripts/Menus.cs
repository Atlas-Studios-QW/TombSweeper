using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        print(CheckItem());
    }

    public string CheckItem()
    {
        int layerMask = 1 << 5;
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics2D.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.transform.gameObject.name;
        }
        return null;
    }
}
