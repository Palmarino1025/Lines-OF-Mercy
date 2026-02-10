using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pauseCanvas;
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
    }
}
