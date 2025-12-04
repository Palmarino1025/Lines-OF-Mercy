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
        // Only listen for E if:
        // 1) player is inside range
        // 2) dialogue is not already active
        if (isPlayerInsideInteractionRange && !isDialogueActive)
        {
            // GetKeyDown is true only on the frame the key is pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("NpcInteraction: E key pressed. Starting dialogue...");
                StartDialogueWithNpc();
            }
        }
    }

    // Called when something enters our trigger collider
    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = true;

            Debug.Log("NpcInteraction: Player entered interaction range.");

            ShowInteractionPrompt(true);
        }
    }

    // Called when something exits our trigger collider
    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = false;

            Debug.Log("NpcInteraction: Player left interaction range.");

            ShowInteractionPrompt(false);
        }
    }

    // Show or hide the "Press E" world-space prompt
    private void ShowInteractionPrompt(bool shouldShowPrompt)
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(shouldShowPrompt);
            Debug.Log("NpcInteraction: Prompt set to " + shouldShowPrompt);
        }
        else
        {
            Debug.LogWarning("NpcInteraction: interactionPromptObject is NOT assigned!");
        }
    }

    // starts the dialogue with this NPC
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
}
