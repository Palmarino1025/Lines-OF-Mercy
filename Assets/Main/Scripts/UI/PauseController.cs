using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public string pauseSceneName = "PauseMenu";
    public GameObject pauseCanvas;
    public GameObject player;
    public GameObject hudCanvas;

    [Header("Restart Settings")]
    public Transform warpPlayer;
    public Transform spawnPoint;

    [Header("Save and Exit")]
    public GameObject splashScreenCanvas;
    public GameObject image;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        pauseCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            // Unlock and show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Lock and hide cursor (typical gameplay mode)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    [Tooltip("Name of the pause menu scene to unload when resuming")]
    

    public void OnContinuePressed()
    {
        // Disables paused state
        TogglePause();

        // Resume time
        Time.timeScale = 1f;

        // Unload the pause menu scene
        //SceneManager.UnloadSceneAsync(pauseSceneName);
        pauseCanvas.SetActive(false);
        player.SetActive(true);
        hudCanvas.SetActive(true);
    }

    public void OnRestartPressed()
    {
        if (player != null && spawnPoint != null)
        {
            player.GetComponent<PlayerMovement>()
                  .TeleportTo(spawnPoint.position);
        }

        // Disables Paused State
        TogglePause();

        // Resumes Time
        Time.timeScale = 1f;

        // Unload the pause menu scene 
        pauseCanvas.SetActive(false);
        player.SetActive(true);
        hudCanvas.SetActive(true);
    }

    public void OnSaveExitPressed()
    {
        // Resumes Time
        Time.timeScale = 1f;

        // Disables Pause Menu, returns to Splash Screen
        pauseCanvas.SetActive(false);
        splashScreenCanvas.SetActive(true);
        image.SetActive(true);
    }
}
