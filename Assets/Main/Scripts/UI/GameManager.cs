using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public string nextSpawnPoint;
    public PlayerMovement playerMovement;
    public PCCameraController pcCameraController;

    private string savePath;

    // =========================================================
    // UI 
    // =========================================================

    [Header("UI Panels")]
    public GameObject pauseScreen;
    public GameObject settingsCanvas;

    [Header("Scene Transition")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    public CanvasGroup pauseCanvasGroup;

    [Header("Gameplay Scripts")]
    public MonoBehaviour[] scriptsToPause;

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

        pauseScreen.SetActive(true);
        pauseCanvasGroup.interactable = true;
        pauseCanvasGroup.blocksRaycasts = true;

        isPaused = true;

        PlayerMove(isPaused);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EventSystem.current.SetSelectedGameObject(null);
        Button defaultButton = pauseScreen.GetComponentInChildren<Button>();
        if (defaultButton != null)
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        pauseScreen.SetActive(false);

        isPaused = false;

        PlayerMove(isPaused);

        EventSystem.current.SetSelectedGameObject(null);

        StartCoroutine(LockCursorNextFrame());
    }

    IEnumerator LockCursorNextFrame()
    {
        yield return null;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
    }

    public void PlayerMove(bool paused)
    {
        if (playerMovement != null)
            playerMovement.SetMovementLock(paused);

        if (pcCameraController != null)
            pcCameraController.EnableCameraLook(!paused);
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

    // =======================================
    // SCENE TRANSITION
    // =======================================

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-enable camera rotation
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        PCCameraController cam = pm.GetComponentInChildren<PCCameraController>();

        if (cam != null)
            cam.EnableCameraLook(true);

        // Unlock player movement if it was locked
        if (pm != null)
            pm.SetMovementLock(false);

        // Move player to spawn point if specified
        if (!string.IsNullOrEmpty(nextSpawnPoint))
        {
            Transform sp = GameObject.Find(nextSpawnPoint)?.transform;
            if (sp != null)
                pm.TeleportTo(sp.position);

            nextSpawnPoint = null; // reset
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(Fade(1)); // Fade to black

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        operation.allowSceneActivation = true;

        yield return StartCoroutine(Fade(0)); // Fade back in
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    public void LoadScene(string sceneName, string spawnPointName)
    {
        nextSpawnPoint = spawnPointName;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }
}