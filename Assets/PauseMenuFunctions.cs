using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuFunctions : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject hudCanvas;
    public GameObject pauseCanvas;
    public GameObject settingsCanvas;
    public GameObject splashScreenCanvas;

    // Called when pause button is pressed
    public void OnPausePressed()
    {
        hudCanvas.SetActive(false); // Hide HUD
        pauseCanvas.SetActive(true); // Show Pause menu
    }

    // Called when continue button is pressed
    public void OnResumePressed()
    {
        hudCanvas.SetActive(true); // Show HUD
        pauseCanvas.SetActive(false); // Hide Pause menu
    }

    // Called when Save and Exit is pressed
    public void OnSaveExitPressed()
    {
        splashScreenCanvas.SetActive(true); // Show HUD
        pauseCanvas.SetActive(false); // Hide Pause menu
    }

    // Open Settings Menu
    public void OnSettingsPressed()
    {
        settingsCanvas.SetActive(true); // Show Settings Canvas
    }
}
