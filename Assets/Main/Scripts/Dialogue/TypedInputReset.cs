using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class TypedInputReset : MonoBehaviour
{
    
    public InputField typedInputField;

    // Call this from Dialogue System Events (OnConversationStart and OnConversationEnd)
    public void ResetTypedInput()
    {
        // Clear the dialogue system variable used by TextInput
        DialogueLua.SetVariable("playerTypedText", "");

        // Clear the visual text in the UI
        if (typedInputField != null)
        {
            typedInputField.text = "";
        }
    }
}
