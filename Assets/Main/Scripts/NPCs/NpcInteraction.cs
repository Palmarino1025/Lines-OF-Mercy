using UnityEngine;
using PixelCrushers.DialogueSystem;  // connect to Dialogue System
using UnityEngine.EventSystems;      // Used to detect if player is typing in UI (InputField focus)
using UnityEngine.UI;   // For InputField
using TMPro;            // For TMP_InputField


public class NpcInteraction : MonoBehaviour
{
    [Header("NPC Identity")]
    public string npcId = "Mob_NPC";

    [Header("Karma Preferences")]
    public NpcPreferenceProfile preferenceProfile;

    // player object Tag
    public string playerTagName = "Player";

    // reference to the "Press E" world-space canvas
    public GameObject interactionPromptObject;
    public GameObject namePlate;

    // Name of the conversation in the Dialogue Database
    public string conversationTitle = "Test Conversation";

    // True only while the player is inside the interaction trigger
    public bool isPlayerInsideInteractionRange = false;

    // keep a reference to the player who entered the trigger
    private Transform playerTransform;

    // Data-driven Personas
    // This MUST match the persona JSON "key" in HuggingFace: /data/personas/*.json
    // Examples: "mob_rico", "cop_holt", "cop_cruz", "cop_briggs", "cop_okabe", "civilian_witness"
    public string personaKey = "default";

    // Context tag for the AI (scene / conversation context)
    // This is separate from personaKey and can be used for scene hints like "Interrogation", "Bar", etc.
    public string aiContextTag = "Default";

    // DSU variable names (so DSU + orchestrator pipeline can read them)
    public string dsuPersonaKeyVar = "AI_PersonaKey";
    public string dsuContextTagVar = "AI_ContextTag";

    // If AI orchestrator is in the scene
    private AITypedInputOrchestrator aiOrchestrator;

    private void Start()
    {
        // ensure the prompt is hidden at start of game
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(false);
            namePlate.SetActive(true);
        }

        // Auto-find the orchestrator once (safe if not present in a scene)
        aiOrchestrator = FindFirstObjectByType<AITypedInputOrchestrator>();
    }

    private void Update()
    {
        // If dialogue is active, do NOT allow starting a new conversation.
        // This prevents the "typing e in Hello" issue from re-triggering StartConversation.
        if (IsDialogueActiveSafe())
        {
            return;
        }

        // If the player is focused on UI (typing), do NOT allow E to trigger interactions.
        // Extra safety even if dialogue state changes later.
        if (IsPlayerTypingInUI())
        {
            return;
        }

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
            namePlate.SetActive(false);

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
            namePlate.SetActive(true);

            Debug.Log("NpcInteraction: Player left interaction range.");
            ShowInteractionPrompt(false);
        }
    }

    private void ShowInteractionPrompt(bool showPrompt)
    {
        // Never show the prompt while dialogue is active
        if (IsDialogueActiveSafe())
        {
            showPrompt = false;
        }

        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(showPrompt);
        }
    }

    private void StartDialogueWithNpc()
    {
        // Hide the "Press E" prompt while we are talking
        ShowInteractionPrompt(false);

        // Push personaKey + contextTag into DSU variables
        // This makes the active NPC persona data-driven
        if (!string.IsNullOrEmpty(dsuPersonaKeyVar))
        {
            DialogueLua.SetVariable(dsuPersonaKeyVar, personaKey);
        }

        if (!string.IsNullOrEmpty(dsuContextTagVar))
        {
            DialogueLua.SetVariable(dsuContextTagVar, aiContextTag);
        }

        // set the orchestrator contextTag (if it exists)
        // NOTE: personaKey is NOT stored in the orchestrator in current script set,
        // but contextTag is, and this still helps pipeline remain consistent.
        if (aiOrchestrator == null)
        {
            aiOrchestrator = FindFirstObjectByType<AITypedInputOrchestrator>();
        }

        if (aiOrchestrator != null)
        {
            aiOrchestrator.contextTag = aiContextTag;

            aiOrchestrator.personaKey = personaKey; // NEW* Set personaKey so HF backend loads the correct /data/personas/<key>.json
            Debug.Log("NpcInteraction assigned Orchestrator personaKey=" + personaKey + " contextTag=" + aiContextTag); // debug persona
        }
        else
        {
            Debug.LogWarning("AITypedInputOrchestrator not found in scene."); // debug - default
        }

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

    //private void OnEnable()
    //{
    //    if (DialogueManager.instance != null)
    //    {
    //        DialogueManager.instance.conversationEnded += OnConversationEnded;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("DialogueManager.instance is null in OnEnable");
    //    }
    //}

    // Safely detect whether DSU dialogue is currently active WITHOUT hardcoding
    // a specific property name (prevents compile issues across DSU versions).
    private bool IsDialogueActiveSafe()
    {
        // Try property: DialogueManager.isConversationActive
        var type = typeof(DialogueManager);

        var prop1 = type.GetProperty("isConversationActive");
        if (prop1 != null)
        {
            object value = prop1.GetValue(null, null);
            if (value is bool b1) return b1;
        }

        // Try property: DialogueManager.IsConversationActive
        var prop2 = type.GetProperty("IsConversationActive");
        if (prop2 != null)
        {
            object value = prop2.GetValue(null, null);
            if (value is bool b2) return b2;
        }

        // Try field: DialogueManager.isConversationActive
        var field1 = type.GetField("isConversationActive");
        if (field1 != null)
        {
            object value = field1.GetValue(null);
            if (value is bool b3) return b3;
        }

        // If we can't detect it, assume false so gameplay doesn't lock up.
        return false;
    }

    // Detect if the player is currently typing in an input field (not just any UI selection)
    private bool IsPlayerTypingInUI()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
        {
            return false;
        }

        // Only block world input if the selected UI object is an actual text input field
        // This prevents needing a mouse click before pressing E.
        if (selectedObject.GetComponent<UnityEngine.UI.InputField>() != null)
        {
            return true;
        }

        if (selectedObject.GetComponent<TMPro.TMP_InputField>() != null)
        {
            return true;
        }

        return false;
    }

}
