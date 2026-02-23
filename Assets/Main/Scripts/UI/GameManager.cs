using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject pauseScreen;      // The pause menu canvas
    private CanvasGroup pauseCanvasGroup;

    [Header("Gameplay Scripts")]
    public MonoBehaviour[] scriptsToPause; // Assign in Inspector any scripts you want paused (enemy AI, combat, etc.)
    public MonoBehaviour playerMovementScript; // Your player movement script (never pause this)

    private bool isPaused = false;

    void Awake()
    {
        // Setup singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
        if (pauseScreen != null)
        {
            pauseCanvasGroup = pauseScreen.GetComponent<CanvasGroup>();
            if (pauseCanvasGroup == null)
            {
                pauseCanvasGroup = pauseScreen.AddComponent<CanvasGroup>();
            }

            // Start hidden
            pauseScreen.SetActive(false);
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }

        // Lock cursor initially
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Toggle pause on ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame()
    {
        // Pause all assigned scripts except player movement
        foreach (var script in scriptsToPause)
        {
            if (script != null && script.enabled && script != playerMovementScript)
            {
                script.enabled = false;
            }
        }

        // Show pause menu
        pauseScreen.SetActive(true);
        pauseCanvasGroup.interactable = true;
        pauseCanvasGroup.blocksRaycasts = true;

        // Unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Select default button
        EventSystem.current.SetSelectedGameObject(null);
        Button defaultButton = pauseScreen.GetComponentInChildren<Button>();
        if (defaultButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        }

        isPaused = true;
    }

    public void ResumeGame()
    {
        // Re-enable all paused scripts
        foreach (var script in scriptsToPause)
        {
            if (script != null && !script.enabled && script != playerMovementScript)
            {
                script.enabled = true;
            }
        }

        // Hide pause menu
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;
        pauseScreen.SetActive(false);

        // Lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
    }

    public void SaveAndExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}