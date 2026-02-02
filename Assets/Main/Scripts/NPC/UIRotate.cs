
using UnityEngine;
using TMPro; // Include this if you are using TextMeshPro

public class UIRotate : MonoBehaviour
{
    public GameObject npcNameText; // Change to 'Text' if not using TextMeshPro
    private Camera mainCam;

    void Start()
    {
        // Find the main camera in the scene
        //mainCam = Camera.main;

        // Optional: Set the initial name via script if needed
        // npcNameText.text = "My NPC Name";
    }

    void LateUpdate()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main; // Automatically finds the scene's main camera by tag
        }

        if (mainCam == null)
        {
            Debug.LogWarning("UIRotate: No main camera found in the scene.");
        }
        // Make the nameplate rotate to always face the camera
        if (mainCam != null)
        {
            transform.rotation = mainCam.transform.rotation;
        }
    }
}

