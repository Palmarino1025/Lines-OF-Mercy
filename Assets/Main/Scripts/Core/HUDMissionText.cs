using UnityEngine;
using TMPro;

public class HUDMissionText : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text missionText; // This is the TextMeshPro text component in your HUD
    [SerializeField] private string defaultText = "No active mission"; // Fallback text if no mission is active

    private void OnEnable()
    {
        // Safety check
        if (DataManager.Instance == null)
            return;

        // Subscribe to mission updates
        DataManager.Instance.OnMissionTextChanged += UpdateMissionText;

        // Immediately update with current mission
        UpdateMissionText(DataManager.Instance.GetCurrentMissionText());
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (DataManager.Instance != null)
            DataManager.Instance.OnMissionTextChanged -= UpdateMissionText;
    }

    // Called whenever the mission changes
    private void UpdateMissionText(string text)
    {
        if (missionText == null) return;

        missionText.text = string.IsNullOrWhiteSpace(text)
            ? defaultText
            : text;
    }
}
