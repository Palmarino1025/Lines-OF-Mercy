using UnityEngine;
using System.IO;
using System;


public class DataManager : MonoBehaviour
{
    [Header("Mission Database")]
    public MissionDatabase missionDatabase;
    public string currentMissionId;
    public static DataManager Instance { get; private set; }

    public PlayerData playerData { get; private set; }

    private string savePath;

    // Event for UI updates (HUD, dialogue, etc.)
    public event Action<string> OnPlayerNameChanged;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Important if changing scenes

        savePath = Path.Combine(Application.persistentDataPath, "playerdata.json");

        LoadPlayerData();

        if (string.IsNullOrEmpty(currentMissionId))
        {
            currentMissionId = "Mission001"; // your first mission's ID
            OnMissionTextChanged?.Invoke(GetCurrentMissionText());
        }
    }

    // =========================
    // PLAYER NAME
    // =========================

    public void SetPlayerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        playerData.playerName = name.Trim();
        SavePlayerData();

        OnPlayerNameChanged?.Invoke(playerData.playerName);
    }

    public string GetPlayerName()
    {
        return playerData?.playerName ?? "";
    }

    // =========================
    // SAVE / LOAD
    // =========================

    public void SavePlayerData()
    {
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save player data: {e.Message}");
        }
    }

    public void LoadPlayerData()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                playerData = JsonUtility.FromJson<PlayerData>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load player data: {e.Message}");
        }

        if (playerData == null)
            playerData = new PlayerData();
    }

    // =========================
    // NPC RELATIONSHIPS
    // =========================

    public NpcRelationshipData GetNpcRelationship(string npcId)
    {
        if (string.IsNullOrEmpty(npcId))
            return null;

        var list = playerData.npcRelationships;

        var npc = list.Find(n => n.npcId == npcId);

        if (npc == null)
        {
            npc = new NpcRelationshipData(npcId);
            list.Add(npc);
            SavePlayerData();
        }

        return npc;
    }

    // =========================
    // MISSION TEXT
    // =========================

    public event System.Action<string> OnMissionTextChanged;

    public void SetMissionText(string missionText)
    {
        playerData.currentMissionText = missionText;
        SavePlayerData();

        OnMissionTextChanged?.Invoke(missionText);
    }

    public string GetMissionText()
    {
        return playerData.currentMissionText ?? "";
    }

    public string GetCurrentMissionText()
    {
        // 1?? Check if missionDatabase exists
        if (missionDatabase == null)
        {
            Debug.LogError("[DataManager] MissionDatabase is NOT assigned!");
            return "No active mission";
        }

        // 2?? Check currentMissionId
        if (string.IsNullOrEmpty(currentMissionId))
        {
            Debug.LogWarning("[DataManager] currentMissionId is empty!");
            return "No active mission";
        }

        // 3?? Try to get the mission from the database
        MissionData mission = missionDatabase.GetMissionById(currentMissionId);

        if (mission == null)
        {
            Debug.LogWarning($"[DataManager] Mission with ID '{currentMissionId}' not found in MissionDatabase!");
            return "No active mission";
        }

        // 4?? Log what text is being returned
        Debug.Log($"[DataManager] Current mission text: {mission.missionDescription}");

        return mission.missionDescription;
    }

    // Optional: debug when setting a mission
    public void SetCurrentMission(string missionId)
    {
        currentMissionId = missionId;
        Debug.Log($"[DataManager] Setting currentMissionId to '{missionId}'");

        string text = GetCurrentMissionText();
        Debug.Log($"[DataManager] Mission text after setting: {text}");

        OnMissionTextChanged?.Invoke(text);
    }

}
