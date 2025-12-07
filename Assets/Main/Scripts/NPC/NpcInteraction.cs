using UnityEngine;
using PixelCrushers.DialogueSystem;  // connect to Dialogue System

public class NpcInteraction : MonoBehaviour
{
    // player object Tag
    public string playerTagName = "Player";

    // reference to the "Press E" world-space canvas
    public GameObject interactionPromptObject;

    // Name of the conversation in the Dialogue Database
    public string conversationTitle = "Test Conversation";

    // True only while the player is inside the interaction trigger
    private bool isPlayerInsideInteractionRange = false;

    // keep a reference to the player who entered the trigger
    private Transform playerTransform;

    private void Start()
    {
        // ensure the prompt is hidden at start of game
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Only listen for E if the player is inside interaction range
        if (isPlayerInsideInteractionRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("NpcInteraction: E pressed. Starting conversation: " + conversationTitle);
            StartDialogueWithNpc();
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = true;
            playerTransform = otherCollider.transform;

            Debug.Log("NpcInteraction: Player entered interaction range.");
            ShowInteractionPrompt(true);
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = false;
            playerTransform = null;

            Debug.Log("NpcInteraction: Player left interaction range.");
            ShowInteractionPrompt(false);
        }
    }

    private void ShowInteractionPrompt(bool showPrompt)
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(showPrompt);
        }
    }

    private void StartDialogueWithNpc()
    {
        // Hide the "Press E" prompt while we are talking
        ShowInteractionPrompt(false);

        if (playerTransform != null)
        {
            // player = actor, npc = conversant
            DialogueManager.StartConversation(conversationTitle, playerTransform, transform);
        }
        else
        {
            // Fallback if we somehow lost the player reference
            DialogueManager.StartConversation(conversationTitle);
        }
    }
}
