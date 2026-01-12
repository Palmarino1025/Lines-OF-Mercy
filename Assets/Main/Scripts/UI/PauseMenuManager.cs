using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Tooltip("Name of the pause menu scene to unload when resuming")]
    public string pauseSceneName = "PauseMenu";

    public void OnContinuePressed()
    {
        // resume time
        Time.timeScale = 1f;

        // unload the pause menu scene
        SceneManager.UnloadSceneAsync(pauseSceneName);
    }
}
