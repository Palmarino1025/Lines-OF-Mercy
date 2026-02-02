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
        if (hudCanvas != null) hudCanvas.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        Time.timeScale = 0f;  // Pause the game
    }

    // Called when continue button is pressed
    public void OnResumePressed()
    {
        Time.timeScale = 1f;  // Resume game time
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);  // Hide pause menu
    }

    // Called when Save and Exit is pressed
    public void OnSaveExitPressed()
    {
        if (splashScreenCanvas != null) splashScreenCanvas.SetActive(true);
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        Time.timeScale = 1f;  // Resume time or transition accordingly
        // Add your saving logic here!
    }

    // Open Settings Menu from Pause Menu
    public void OnSettingsPressed()
    {
        if (settingsCanvas != null) settingsCanvas.SetActive(true);
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
    }

    // Called to close settings and go back to Pause Menu
    public void OnSettingsBackPressed()
    {
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
    }
}
