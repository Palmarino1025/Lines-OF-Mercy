using UnityEngine;

public class NpcInteraction : MonoBehaviour
{
    // player object Tag
    public string playerTagName = "Player";

    // reference to the "Press E" world-space canvas
    public GameObject interactionPromptObject;

    // This will be true only while the player is inside the trigger
    private bool isPlayerInsideInteractionRange = false;

    private void Start()
    {
        // ensure the prompt is hidden at start of game
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(false);
        }
    }

    // This method is called when something enters the trigger collider
    private void OnTriggerEnter(Collider otherCollider)
    {
        // Check if whatever entered has the same tag as player
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = true;

            Debug.Log("NpcInteraction: Player entered interaction range.");

            ShowInteractionPrompt(true);
        }
    }

    // This method is called when something exits the trigger collider
    private void OnTriggerExit(Collider otherCollider)
    {
        // Check if whatever left has the same tag as our player
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = false;

            Debug.Log("NpcInteraction: Player left interaction range.");

            ShowInteractionPrompt(false);
        }
    }
    // controls the UI prompt visibility
    private void ShowInteractionPrompt(bool shouldShowPrompt)
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(shouldShowPrompt);
        }

    }
}