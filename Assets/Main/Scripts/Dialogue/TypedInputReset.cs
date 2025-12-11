using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class TypedInputReset : MonoBehaviour
{
    [Header("UI")]
    public InputField typedInputField;

    [Header("Dialogue System")]
    public string variableName = "playerTypedText";  // must match the variable you use

    public void ResetTypedInput()
    {
        // 1. Clear the UI text
        if (typedInputField != null)
        {
            typedInputField.text = string.Empty;
        }

        // 2. Clear the Dialogue System variable
        if (!string.IsNullOrEmpty(variableName))
        {
            DialogueLua.SetVariable(variableName, string.Empty);
        }
    }
}
