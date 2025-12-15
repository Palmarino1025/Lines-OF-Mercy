using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public PlayerData playerData;

    private string savePath;

    void Awake()
    {
        // Single instance singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = UnityEngine.Application.persistentDataPath + "/playerdata.json";
        LoadPlayerData();
    }

    // Set the player name
    public void SetPlayerName(string name)
    {
        playerData.playerName = name;
        SavePlayerData();
    }

    // Save to the disk
    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(savePath, json);
    }

    // Load from the disk
    public void LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData = new PlayerData();
        }
    }

    // Load player name
    public string GetPlayerName()
    {
        if (playerData == null)
            return "";

        return playerData.playerName;
    }
}