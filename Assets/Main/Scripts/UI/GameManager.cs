using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Pause Menu")]
    public GameObject pauseCanvas;

    private bool isPaused = false;

    private void Awake()
    {
        // Singleton pattern: only one GameManager allowed
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keeps this alive across scene loads
    }

    private void Update()
    {
        // Check for ESC key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);
        else
            Debug.LogWarning("Pause Canvas is not assigned in GameManager!");
        // Optional: mute audio, stop player input, etc.
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        // Optional: unmute audio, enable player input, etc.
    }

    // Optional: method to quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
