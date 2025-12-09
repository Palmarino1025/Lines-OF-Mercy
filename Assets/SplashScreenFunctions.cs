using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class SplashScreenFunctions : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject menuButtonGroup;
    public GameObject nameEntryPanel;

    [Header("Name Entry")]
    public TMP_InputField playerNameInput;
    public GameObject splashScreenCanvas;

    // Hide the splash screen when pressed, show name-setting panel
    public void OnNewGamePressed()
    {
        menuButtonGroup.SetActive(false);
        nameEntryPanel.SetActive(true);
    }

    // Keep the player from advancing until a name is typed in
    public void OnContinuePressed()
    {
        string playerName = playerNameInput.text;

        if (string.IsNullOrEmpty(playerName))
        {
            UnityEngine.Debug.Log("Player name is empty.");
            return;
        }

        // Save name somewhere (just printing it for now)
        UnityEngine.Debug.Log("Player name: " + playerName);

        // Hide the splash screen, letting the player into the game
        splashScreenCanvas.SetActive(false);
    }
}