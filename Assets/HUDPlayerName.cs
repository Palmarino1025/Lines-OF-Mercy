using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class HUDPlayerName : MonoBehaviour
{
    public TMP_Text playerNameText;

    void Start()
    {
        UpdatePlayerName();
    }

    public void UpdatePlayerName()
    {
        if (DataManager.Instance == null)
        {
            UnityEngine.Debug.LogWarning("DataManager not found.");
            return;
        }

        string playerName = DataManager.Instance.GetPlayerName();

        if (string.IsNullOrEmpty(playerName))
        {
            playerNameText.text = "Your character";
        }

        else
        {
            playerNameText.text = playerName;
        }
    }
}