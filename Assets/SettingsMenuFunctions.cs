using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuFunctions : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsCanvas;
    public GameObject splashScreenCanvas;

    public void OpenSettings()
    {
        splashScreenCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        splashScreenCanvas.SetActive(true);
    }
}
