using UnityEngine;
using TMPro;

public class SceneDoor : MonoBehaviour
{
    [Header("Scene Transition")]
    public string sceneToLoad;       // e.g., "Act1_BackAlley_Luca"
    public string spawnPointName;    // e.g., "FromBar"

    [Header("UI Prompt")]
    public GameObject interactionPrompt; // UI text object

    private bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            playerInside = false;

            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            GameManager.Instance.LoadScene(sceneToLoad, spawnPointName);
        }
    }
}