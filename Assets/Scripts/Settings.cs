using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public GameObject ViewModeSelector;

    public int ViewMode = 0;

    TMP_Dropdown ViewModeDropDown;

    public void ConfirmChanges()
    {
        SetSettings();
        LoadSettings();
    }

    private void LoadSettings()
    {
        ViewMode = PlayerPrefs.GetInt("ViewMode");

        if (ViewMode == 0) { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; }
        if (ViewMode == 1) { Screen.fullScreenMode = FullScreenMode.Windowed; }
        if (ViewMode == 2) { Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; }
    }

    private void Start()
    {
        LoadSettings();

        if (SceneManager.GetActiveScene().name == "Settings")
        {
            ViewModeDropDown = ViewModeSelector.GetComponent<TMP_Dropdown>();
            ViewModeDropDown.value = ViewMode;
        }
    }


    private void SetSettings()
    {
        PlayerPrefs.SetInt("ViewMode", ViewModeDropDown.value);
    }
}
