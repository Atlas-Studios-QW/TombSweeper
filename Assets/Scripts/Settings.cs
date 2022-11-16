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
    public GameObject IntervalSelector;
    public GameObject Confirmation;

    private int ViewMode = 0;
    private int Interval = 0;

    TMP_Dropdown ViewModeDropDown;
    Slider IntervalSlider;

    public void ConfirmChanges()
    {
        SetSettings();
        LoadSettings();
    }
    private void Start()
    {
        LoadSettings();

        if (SceneManager.GetActiveScene().name == "Settings")
        {
            ViewModeDropDown = ViewModeSelector.GetComponent<TMP_Dropdown>();
            ViewModeDropDown.value = ViewMode;

            IntervalSlider = IntervalSelector.transform.Find("Slider").GetComponent<Slider>();
            IntervalSlider.value = Interval;

            SliderUpdate();
        }
    }

    private void LoadSettings()
    {
        ViewMode = PlayerPrefs.GetInt("ViewMode");

        if (ViewMode == 0) { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; }
        if (ViewMode == 1) { Screen.fullScreenMode = FullScreenMode.Windowed; }
        if (ViewMode == 2) { Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; }

        Interval = PlayerPrefs.GetInt("AutoSaveTime");
    }

    public void SliderUpdate()
    {
        string IndicatorText = "";
        if (IntervalSlider.value == 0) { IndicatorText = "OFF"; } else { IndicatorText = IntervalSlider.value + " Min."; }
        IntervalSelector.transform.Find("Indicator").GetComponent<TextMeshProUGUI>().text = IndicatorText;
    }

    private void SetSettings()
    {
        PlayerPrefs.SetInt("ViewMode", ViewModeDropDown.value);
        PlayerPrefs.SetInt("AutoSaveTime", int.Parse(IntervalSlider.value.ToString()));
        StartCoroutine(Alerter());
    }

    private IEnumerator Alerter()
    {
        Confirmation.SetActive(true);
        yield return new WaitForSeconds(1);
        Confirmation.SetActive(false);
    }
}
