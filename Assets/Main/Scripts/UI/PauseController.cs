using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [Tooltip("Name of the pause menu scene to load")]
    public string pauseSceneName = "PauseMenu";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
        }
    }

    
    public void OpenPauseMenu()
    {
        // stop the game
        Time.timeScale = 0f;

        // load pause menu additively
        SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive);
    }
}
