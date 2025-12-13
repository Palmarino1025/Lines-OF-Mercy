using UnityEngine;
using TMPro; // TextMeshPro namespace

public class KarmaHUD : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI karmaText; // for the Inspector

    [Header("Settings")]
    public bool showAlignmentLabel = true;
    public bool showHud = true;

    private void Update()
    {
        // If HUD is turned off or no text assigned, do nothing.
        if (!showHud || karmaText == null)
        {
            return;
        }

        // KarmaEngine must exists before trying to read from it.
        if (KarmaEngine.Instance == null)
        {
            karmaText.text = "KarmaEngine not found.";
            return;
        }

        // Read the current values from the KarmaEngine.
        float mobLoyaltyValue = KarmaEngine.Instance.mobLoyalty;
        float policeLoyaltyValue = KarmaEngine.Instance.policeLoyalty;
        float mercyValue = KarmaEngine.Instance.mercy;
        float ruthlessnessValue = KarmaEngine.Instance.ruthlessness;

        string alignmentText = "";

        if (showAlignmentLabel)
        {
            alignmentText = KarmaEngine.Instance.GetAlignmentQuadrant();
        }

        // Build a simple multi-line string for display.
        // Example:
        // Mob: 10  Cops: -5
        // Mercy: 0  Ruth: 5
        // Align: Neutral
        string hudText = "Mob: " + mobLoyaltyValue.ToString("F0") +
                         "    Cops: " + policeLoyaltyValue.ToString("F0") + "\n" +
                         "Mercy: " + mercyValue.ToString("F0") +
                         "    Ruth: " + ruthlessnessValue.ToString("F0");

        if (showAlignmentLabel)
        {
            hudText += "\nAlign: " + alignmentText;
        }

        karmaText.text = hudText;
    }
}
