using UnityEngine;
using PixelCrushers.DialogueSystem;  // connect to Dialogue System

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

    private PCCameraController cameraController;

    private void Start()
    {
        // ensure the prompt is hidden at start of game
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(false);
            namePlate.SetActive(true);
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

    void LateUpdate()
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(false);
            namePlate.SetActive(true);
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
            namePlate.SetActive(!showPrompt);
        }
    }

    private void StartDialogueWithNpc()
    {
        // Hide the "Press E" prompt while we are talking
        ShowInteractionPrompt(false);

        CacheCameraController();

        if (cameraController != null)
        {
            cameraController.EnableCameraLook(false);
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

    public void ApplyKarma(
        float mobLoyalty,
        float policeLoyalty,
        float mercy,
        float ruthlessness)
    {
        if (KarmaEngine.Instance == null)
        {
            Debug.LogError("[NpcInteraction] KarmaEngine not found!");
            return;
        }

        KarmaEngine.Instance.ApplyKarmaDelta(
            mobLoyalty,
            policeLoyalty,
            mercy,
            ruthlessness
        );

        Debug.Log("[NpcInteraction] Karma applied from dialogue choice.");
    }

    private void CacheCameraController()
    {
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<PCCameraController>();
        }
    }
    private void OnEnable()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.conversationEnded += OnConversationEnded;
        }
        else
        {
            Debug.LogWarning("DialogueManager.instance is null in OnEnable");
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.instance != null)
            DialogueManager.instance.conversationEnded -= OnConversationEnded;
    }

    private void OnConversationEnded(Transform actor)
    {
        CacheCameraController();

        if (cameraController != null)
        {
            cameraController.EnableCameraLook(true);
        }
    }

    public void EvaluatePlayerAlignment()
    {
        if (DataManager.Instance == null) return;

        var npcData = DataManager.Instance.GetNpcRelationship(npcId);

        float alignmentScore =
            KarmaEngine.Instance.mobLoyalty * preferenceProfile.mobLoyaltyWeight +
            KarmaEngine.Instance.policeLoyalty * preferenceProfile.policeLoyaltyWeight +
            KarmaEngine.Instance.mercy * preferenceProfile.mercyWeight +
            KarmaEngine.Instance.ruthlessness * preferenceProfile.ruthlessnessWeight;

        npcData.affinity += alignmentScore * 0.1f;
        npcData.affinity = Mathf.Clamp(npcData.affinity, -100f, 100f);

        DataManager.Instance.SavePlayerData();

        Debug.Log($"[NPC:{npcId}] Alignment reaction: {npcData.affinity}");

        DialogueLua.SetVariable("MobNPC_Alignment", npcData.affinity);
    }
}
