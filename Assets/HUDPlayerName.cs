using UnityEngine;
using TMPro;

public class HUDPlayerName : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private string defaultName = "Your character";

    private void OnEnable()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("DataManager not found.");
            return;
        }

        // Subscribe to changes
        DataManager.Instance.OnPlayerNameChanged += UpdatePlayerName;

        // Initial update (important if name already exists)
        UpdatePlayerName(DataManager.Instance.GetPlayerName());
    }

    private void OnDisable()
    {
        if (DataManager.Instance != null)
            DataManager.Instance.OnPlayerNameChanged -= UpdatePlayerName;
    }

    public void UpdatePlayerName(string name)
    {
        playerNameText.text = string.IsNullOrWhiteSpace(name)
            ? defaultName
            : name;
    }
}
