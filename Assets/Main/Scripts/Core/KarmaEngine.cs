using UnityEngine;
using PixelCrushers.DialogueSystem; // for DialogueLua

public class KarmaEngine : MonoBehaviour
{
    // Simple singleton so other scripts can find this easily
    public static KarmaEngine Instance { get; private set; }

    // Our 4 main alignment values
    public int mobLoyalty = 0;
    public int copLoyalty = 0;
    public int mercy = 0;
    public int ruthlessness = 0;

    private void Awake()
    {
        // Basic singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // This will be called whenever the player submits a typed response
    public void ApplyTypedResponse(string playerText)
    {
        Debug.Log("KarmaEngine: Player typed => " + playerText);

        // Store raw text into Dialogue System variable for later branching
        DialogueLua.SetVariable("LastTypedResponse", playerText);

        // TODO (future): call AI here and interpret tone/intent,
        // then adjust mobLoyalty, copLoyalty, mercy, ruthlessness.
        // For now, this is just our stub.
    }
}
