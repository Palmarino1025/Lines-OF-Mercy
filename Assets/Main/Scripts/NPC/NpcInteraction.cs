using UnityEngine;

public class NpcInteraction : MonoBehaviour
{
    // player object Tag
    public string playerTagName = "Player";

    // reference to the "Press E" world-space canvas
    public GameObject interactionPromptObject;

    // UI panel that holds your dialogue UI (DialogPanel)
    public GameObject dialoguePanelObject;

    // True only while the player is inside the interaction trigger
    private bool isPlayerInsideInteractionRange = false;

    // True while this NPC's dialogue is currently open
    private bool isDialogueActive = false;

    private void Start()
    {
        // ensure the prompt is hidden at start of game
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(false);
        }

        // Make sure the dialogue panel is hidden at the start
        if (dialoguePanelObject != null)
        {
            dialoguePanelObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Only listen for E if player is close and dialogue is not already open
        if (isPlayerInsideInteractionRange && !isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("NpcInteraction: E key pressed. Starting dialogue...");
                StartDialogueWithNpc();
            }
        }

        // allow closing dialogue with Escape
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("NpcInteraction: Escape pressed. Closing dialogue...");
            CloseDialogueWithNpc();
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = true;

            Debug.Log("NpcInteraction: Player entered interaction range.");

            // Only show the prompt if dialogue is not already active
            if (!isDialogueActive)
            {
                ShowInteractionPrompt(true);
            }
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = false;

            Debug.Log("NpcInteraction: Player left interaction range.");

            // Hide the E prompt when we are no longer near the NPC
            ShowInteractionPrompt(false);

            // Do NOT close the dialogue here – player closes it with Exit button / ESC
        }
    }

    private void ShowInteractionPrompt(bool shouldShowPrompt)
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(shouldShowPrompt);
        }
    }

    private void StartDialogueWithNpc()
    {
        // Hide the "Press E" prompt while we are talking
        ShowInteractionPrompt(false);

        // Turn on the dialogue UI panel
        if (dialoguePanelObject != null)
        {
            dialoguePanelObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("NpcInteraction: dialoguePanelObject is NOT assigned!");
        }

        // Mark dialogue as active so we do not reopen it again right away
        isDialogueActive = true;

        // TODO later:
        // - Call Dialogue Manager / Yarn Spinner node
        // - Freeze player movement
    }

    // the UI button can call this
    public void CloseDialogueWithNpc()
    {
        // Turn off the dialogue UI panel
        if (dialoguePanelObject != null && dialoguePanelObject.activeSelf)
        {
            dialoguePanelObject.SetActive(false);
        }

        // Reset dialogue state
        isDialogueActive = false;

        // If the player is still in range, show the "Press E" prompt again
        if (isPlayerInsideInteractionRange)
        {
            ShowInteractionPrompt(true);
        }
    }
}
