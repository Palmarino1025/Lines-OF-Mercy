using UnityEngine;
using UnityEngine.UI;           // For InputField
using PixelCrushers.DialogueSystem;  // For DialogueLua

public class TypedResponseUI : MonoBehaviour
{
    // Reference to the Unity UI InputField where the player types.
    public InputField typedInputField;

    // Name of the Dialogue System variable where we'll store the text.
    public string dialogueVariableName = "playerTypedText";

    // If true, we'll log the text to the Console for debugging.
    public bool logToConsole = true;

    // This method will be called by the Submit button and by the Enter key.
    public void SubmitTypedResponse()
    {
        if (typedInputField == null)
        {
            Debug.LogWarning("TypedResponseUI: typedInputField is not assigned.");
            return;
        }

        string playerText = typedInputField.text;

        // Do nothing if the player didn't type anything.
        if (string.IsNullOrWhiteSpace(playerText))
        {
            return;
        }

        // Store the text into a Dialogue System variable so we can use it later.
        DialogueLua.SetVariable(dialogueVariableName, playerText);

        if (logToConsole)
        {
            Debug.Log("TypedResponseUI: Player typed -> " + playerText);
        }

        // Clear the field for the next input.
        typedInputField.text = string.Empty;

        // Put focus back in the field, so they can type again if needed.
        typedInputField.ActivateInputField();
    }
}
