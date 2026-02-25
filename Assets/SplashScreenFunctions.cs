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
    PCCameraController camController;

    void Start()
    {
        // Show and unlock cursor for splash screen
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Freeze camera + show cursor
        camController = player.GetComponentInChildren<PCCameraController>();
        camController.EnableCameraLook(false);

        player.GetComponent<PlayerMovement>().SetMovementLock(true);

    }

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
        GameManager.Instance.SetPlayerName(playerName);

        // Reset Karma for new game
        if (KarmaEngine.Instance != null)
        {
            KarmaEngine.Instance.ResetKarma();
        }

        // Update the HUD with entered name
        HUDPlayerName hud = FindObjectOfType<HUDPlayerName>();
        if (hud != null)
            hud.UpdatePlayerName("New Player");

        player.GetComponent<PlayerMovement>().SetMovementLock(false);

        // Hide the splash screen, activating the player and letting them into the game
        splashScreenCanvas.SetActive(false);
        background.SetActive(true);
        hudCanvas.SetActive(true);
        nameEntryPanel.SetActive(false);
        player.SetActive(true);

        // Unlock camera + hide cursor
        camController.EnableCameraLook(true);

        player.GetComponent<PlayerMovement>().SetMovementLock(false);
    }

    // Continue with all previous save data
    public void OnLoadPressed()
    {
        // Unlock camera + hide cursor
        camController.EnableCameraLook(true);

        player.GetComponent<PlayerMovement>().SetMovementLock(false);

        splashScreenCanvas.SetActive(false);
        background.SetActive(true);
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

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}