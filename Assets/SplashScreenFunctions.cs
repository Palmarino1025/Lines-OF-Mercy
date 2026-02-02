using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.Security.Permissions;

public class SplashScreenFunctions : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject menuButtonGroup;
    public GameObject nameEntryPanel;
    public GameObject hudCanvas;
    public GameObject settingsCanvas;
    public GameObject background;

    [Header("Name Entry")]
    public TMP_InputField playerNameInput;
    public GameObject splashScreenCanvas;

    [Header("Player")]
    public GameObject player;

    // Hide the splash screen when pressed, show name-setting panel
    public void OnNewGamePressed()
    {
        UnityEngine.Debug.Log("Entered");

        menuButtonGroup.SetActive(false);
        splashScreenCanvas.SetActive(false);
        nameEntryPanel.SetActive(true);
    }

    // Keep the player from advancing until a name is typed in
    public void OnContinuePressed()
    {
        string playerName = playerNameInput.text;

        if (string.IsNullOrEmpty(playerName))
            return;

        // Set the player's name as what they typed and save it
        DataManager.Instance.SetPlayerName(playerName);

        // Reset Karma for new game
        if (KarmaEngine.Instance != null)
        {
            KarmaEngine.Instance.ResetKarma();
        }

        // Update the HUD with entered name
        HUDPlayerName hud = FindObjectOfType<HUDPlayerName>();
        if (hud != null)
            hud.UpdatePlayerName();

        // Hide the splash screen, activating the player and letting them into the game
        splashScreenCanvas.SetActive(false);
        background.SetActive(false);
        hudCanvas.SetActive(true);
        nameEntryPanel.SetActive(false);
        player.SetActive(true);
    }

    // Continue with all previous save data
    public void OnLoadPressed()
    {
        splashScreenCanvas.SetActive(false);
        background.SetActive(false);
        hudCanvas.SetActive(true);
        player.SetActive(true);
    }

    // Open Settings
    public void OnSettingsPressed()
    {
        splashScreenCanvas.SetActive(false);
        background.SetActive(false);
        settingsCanvas.SetActive(true);
    }
}