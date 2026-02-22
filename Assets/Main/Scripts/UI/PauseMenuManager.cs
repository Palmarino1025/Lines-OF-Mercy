using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseScreen;      // The pause menu canvas
    private CanvasGroup pauseCanvasGroup;

    private bool isPaused = false;

    void Awake()
    {
        if (pauseScreen != null)
        {
            pauseCanvasGroup = pauseScreen.GetComponent<CanvasGroup>();
            if (pauseCanvasGroup == null)
            {
                pauseCanvasGroup = pauseScreen.AddComponent<CanvasGroup>();
            }
        }

        // Ensure pause screen starts hidden
        pauseScreen.SetActive(false);
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;

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
        // Disable gameplay scripts here (example)
        CharacterController player = FindObjectOfType<CharacterController>();
        if (player != null) player.enabled = false;

        // Show pause menu
        pauseScreen.SetActive(true);
        pauseCanvasGroup.interactable = true;
        pauseCanvasGroup.blocksRaycasts = true;

        // Unlock cursor for menu interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Optional: select default button for controller/keyboard navigation
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
        // Re-enable gameplay scripts
        CharacterController player = FindObjectOfType<CharacterController>();
        if (player != null) player.enabled = true;

        // Hide pause menu
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;
        pauseScreen.SetActive(false);

        // Lock cursor back for gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
    }

    public void SaveAndExit()
    {
        // Add save logic here if needed
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}