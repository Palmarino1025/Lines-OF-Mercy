
using UnityEngine;
using TMPro; // Include this if you are using TextMeshPro

public class UIRotate : MonoBehaviour
{
    public TextMeshPro npcNameText; // Change to 'Text' if not using TextMeshPro
    private Camera mainCam;

    void Start()
    {
        // Find the main camera in the scene
        mainCam = Camera.main;
        // Optional: Set the initial name via script if needed
        // npcNameText.text = "My NPC Name";
    }

    void LateUpdate()
    {
        // Make the nameplate rotate to always face the camera
        if (mainCam != null)
        {
            transform.rotation = mainCam.transform.rotation;
        }
    }
}

