using UnityEngine;

public class DialogueFreezeController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public PCCameraController pcCameraController;

    // This is called when DSU/dialogue starts
    public void OnDialogueOpened()
    {
        if (playerMovement != null)
        {
            playerMovement.SetMovementLock(true);
        }

        if (pcCameraController != null)
        {
            pcCameraController.EnableCameraLook(false); // Unlock mouse, stop look
        }
    }

    // This is called when DSU/dialogue ends
    public void OnDialogueClosed()
    {
        if (playerMovement != null)
        {
            playerMovement.SetMovementLock(false);
        }

        if (pcCameraController != null)
        {
            pcCameraController.EnableCameraLook(true); // Lock mouse, resume look
        }
    }
}
