using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // =========================================================
    // MISSION DATABASE
    // =========================================================

    [Header("Mission Database")]
    public MissionDatabase missionDatabase;
    public string currentMissionId;

    public event Action<string> OnMissionTextChanged;

    // =========================================================
    // PLAYER DATA
    // =========================================================

    public PlayerData playerData { get; private set; }
    public event Action<string> OnPlayerNameChanged;

    private string savePath;

    // =========================================================
    // UI / PAUSE
    // =========================================================

    [Header("UI Panels")]
    public GameObject pauseScreen;
    public GameObject settingsCanvas;

    private CanvasGroup pauseCanvasGroup;

    [Header("Gameplay Scripts")]
    public MonoBehaviour[] scriptsToPause;
    public MonoBehaviour playerMovementScript;

    private bool isPaused = false;

    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "playerdata.json");

        LoadPlayerData();

        SetupPauseSystem();

        // Default mission
        if (string.IsNullOrEmpty(currentMissionId))
        {
            currentMissionId = "Mission001";
            OnMissionTextChanged?.Invoke(GetCurrentMissionText());
        }
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    // =========================================================
    // PLAYER NAME
    // =========================================================

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

    // =========================================================
    // SAVE / LOAD
    // =========================================================

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

    // =========================================================
    // SETTINGS
    // =========================================================

    public void SetBrightness(float value)
    {
        playerData.brightness = value;
        SavePlayerData();
    }

    public float GetBrightness() => playerData.brightness;

    public void SetTextSpeed(float speed)
    {
        playerData.textSpeed = speed;
        SavePlayerData();
    }

    public float GetTextSpeed() => playerData.textSpeed;

    // =========================================================
    // NPC RELATIONSHIPS
    // =========================================================

    public NpcRelationshipData GetNpcRelationship(string npcId)
    {
        if (string.IsNullOrEmpty(npcId))
            return null;

        var npc = playerData.npcRelationships
            .Find(n => n.npcId == npcId);

        if (npc == null)
        {
            npc = new NpcRelationshipData(npcId);
            playerData.npcRelationships.Add(npc);
            SavePlayerData();
        }

        return npc;
    }

    // =========================================================
    // MISSIONS
    // =========================================================

    public string GetCurrentMissionText()
    {
        if (missionDatabase == null)
        {
            Debug.LogError("[GameManager] MissionDatabase NOT assigned!");
            return "No active mission";
        }

        MissionData mission = missionDatabase.GetMissionById(currentMissionId);

        if (mission == null)
            return "No active mission";

        return mission.missionDescription;
    }

    public void SetCurrentMission(string missionId)
    {
        currentMissionId = missionId;
        OnMissionTextChanged?.Invoke(GetCurrentMissionText());
    }

    // =========================================================
    // PAUSE SYSTEM
    // =========================================================

    private void SetupPauseSystem()
    {
        if (pauseScreen == null) return;

        pauseCanvasGroup = pauseScreen.GetComponent<CanvasGroup>();
        if (pauseCanvasGroup == null)
            pauseCanvasGroup = pauseScreen.AddComponent<CanvasGroup>();

        pauseScreen.SetActive(false);
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PauseGame()
    {
        foreach (var script in scriptsToPause)
        {
            if (script != null && script.enabled && script != playerMovementScript)
                script.enabled = false;
        }

        pauseScreen.SetActive(true);
        pauseCanvasGroup.interactable = true;
        pauseCanvasGroup.blocksRaycasts = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EventSystem.current.SetSelectedGameObject(null);
        Button defaultButton = pauseScreen.GetComponentInChildren<Button>();
        if (defaultButton != null)
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);

        isPaused = true;
    }

    public void ResumeGame()
    {
        foreach (var script in scriptsToPause)
        {
            if (script != null && !script.enabled && script != playerMovementScript)
                script.enabled = true;
        }

        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;
        pauseScreen.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
    }

    public void SaveAndExit()
    {
        SavePlayerData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}