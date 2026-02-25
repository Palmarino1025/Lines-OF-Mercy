using UnityEngine;
using TMPro;

public class HUDPlayerName : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private string defaultName = "Your character";

    private void OnEnable()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager not found.");
            return;
        }

        // Subscribe to changes
        GameManager.Instance.OnPlayerNameChanged += UpdatePlayerName;

        // Initial update (important if name already exists)
        UpdatePlayerName(GameManager.Instance.GetPlayerName());
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerNameChanged -= UpdatePlayerName;
    }

    public void UpdatePlayerName(string name)
    {
        playerNameText.text = string.IsNullOrWhiteSpace(name)
            ? defaultName
            : name;
    }
}
