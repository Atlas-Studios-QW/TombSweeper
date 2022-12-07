using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    public GameObject Bottom;

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Menu");
        }

        if (Bottom.transform.position.y > Screen.height)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            transform.position += new Vector3(0,Time.deltaTime * 250,0);
        }
    }
}
