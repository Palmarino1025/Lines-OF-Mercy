using UnityEngine;

public class SceneDoor : MonoBehaviour
{
    [Header("Scene Transition")]
    public string sceneToLoad;       // e.g., "Act1_BackAlley_Luca"
    public string spawnPointName;    // e.g., "FromBar"

    private bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            playerInside = false; // prevent double triggering

            // Call GameManager and pass both scene name AND spawn point
            GameManager.Instance.LoadScene(sceneToLoad, spawnPointName);
        }
    }
}