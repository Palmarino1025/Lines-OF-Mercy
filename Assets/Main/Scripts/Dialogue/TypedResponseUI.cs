using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Handles the player's typed input.
/// We are NOT using a Submit button.
/// DSU (Standard UI Input Field) calls SubmitTypedResponse() when Enter is pressed.
/// </summary>
public class TypedResponseUI : MonoBehaviour
{
    [Header("UI Input")]
    public InputField typedInputField;

    [Header("Dialogue System Variable")]
    public string dialogueVariableName = "playerTypedText";

    [Header("AI Orchestrator")]
    public AITypedInputOrchestrator aiOrchestrator;

    [Header("Debug")]
    public bool logToConsole = true;

    private void Awake()
    {
        // Safety check: make sure the input field is assigned
        if (typedInputField == null)
        {
            Debug.LogWarning("TypedResponseUI: typedInputField is not assigned.");
        }

        // IMPORTANT:
        // If Unity loses the reference (common with UI/prefabs),
        // we auto-find the orchestrator at runtime.
        if (aiOrchestrator == null)
        {
            aiOrchestrator = FindFirstObjectByType<AITypedInputOrchestrator>();
        }
    }

    /// <summary>
    /// This is called by DSU's Standard UI Input Field "On Accept()"
    /// when the player presses Enter.
    /// </summary>
    public void SubmitTypedResponse()
    {
        Debug.Log("TypedResponseUI on object: " + gameObject.name);

        if (typedInputField == null)
        {
            Debug.LogWarning("TypedResponseUI: typedInputField is missing.");
            return;
        }

        string playerText = typedInputField.text;

        // Ignore empty input
        if (string.IsNullOrWhiteSpace(playerText))
        {
            return;
        }

        // Store into DSU variable so DSU can reference what the player typed
        DialogueLua.SetVariable(dialogueVariableName, playerText);

        if (logToConsole)
        {
            Debug.Log("TypedResponseUI: Player typed -> " + playerText);
        }

        // If reference is missing, try to find it again (extra safe)
        if (aiOrchestrator == null)
        {
            aiOrchestrator = FindFirstObjectByType<AITypedInputOrchestrator>();
        }

        // Send to AI
        if (aiOrchestrator != null)
        {
            aiOrchestrator.AnalyzeAndApply(playerText);
        }
        else
        {
            Debug.LogWarning("TypedResponseUI: aiOrchestrator is not assigned (and could not be found).");
        }

        // Clear the field for the next line
        typedInputField.text = string.Empty;

        // Put focus back so the player can keep typing naturally
        typedInputField.ActivateInputField();
    }
}
