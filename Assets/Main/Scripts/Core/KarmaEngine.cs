using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Diagnostics;

public class KarmaEngine : MonoBehaviour
{
    // Singleton pattern so other scripts can easily find the KarmaEngine.
    public static KarmaEngine Instance;

    [Header("Karma Values (Range -100 to 100)")]

    [Range(-100f, 100f)]
    public float mobLoyalty;

    [Range(-100f, 100f)]
    public float policeLoyalty;

    [Range(-100f, 100f)]
    public float mercy;

    [Range(-100f, 100f)]
    public float ruthlessness;

    [Header("Debug Options")]
    public bool printKarmaChangesToConsole = true;

    private void Awake()
    {
        // Basic singleton setup.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // keep this object between scenes.
        DontDestroyOnLoad(this.gameObject);

        // Initialize any starting values here if needed.
        // For now everything starts at 0.
        SynchronizeWithDialogueSystem();
    }

    private void Start()
    {
        LoadFromPlayerData();
    }

    /// <summary>
    /// Applies a change to the four karma values.
    /// This method will be called from choices or AI text results.
    /// </summary>
    public void ApplyKarmaDelta(
        float mobLoyaltyChange,
        float policeLoyaltyChange,
        float mercyChange,
        float ruthlessnessChange)
    {
        mobLoyalty += mobLoyaltyChange;
        policeLoyalty += policeLoyaltyChange;
        mercy += mercyChange;
        ruthlessness += ruthlessnessChange;

        // Clamp values so they stay in a safe range.
        mobLoyalty = Mathf.Clamp(mobLoyalty, -100f, 100f);
        policeLoyalty = Mathf.Clamp(policeLoyalty, -100f, 100f);
        mercy = Mathf.Clamp(mercy, -100f, 100f);
        ruthlessness = Mathf.Clamp(ruthlessness, -100f, 100f);

        // Send updated values to Dialogue System.
        SynchronizeWithDialogueSystem();

        if (printKarmaChangesToConsole)
        {
            UnityEngine.Debug.Log("[KarmaEngine] Updated Karma:" +
                      " MobLoyalty=" + mobLoyalty +
                      " PoliceLoyalty=" + policeLoyalty +
                      " Mercy=" + mercy +
                      " Ruthlessness=" + ruthlessness +
                      " | Alignment=" + GetAlignmentQuadrant());
        }

        // Save updated values to DataManager
        WriteToPlayerData();
    }

    /// <summary>
    /// Writes the current Karma values into Dialogue System variables.
    /// This lets DSU use them in Lua conditions or variable checks.
    /// </summary>
    public void SynchronizeWithDialogueSystem()
    {
        // These variable names must match what you create in DSU.
        DialogueLua.SetVariable("Karma_MobLoyalty", mobLoyalty);
        DialogueLua.SetVariable("Karma_PoliceLoyalty", policeLoyalty);
        DialogueLua.SetVariable("Karma_Mercy", mercy);
        DialogueLua.SetVariable("Karma_Ruthlessness", ruthlessness);
    }

    /// <summary>
    /// reads back values from Dialogue System
    /// want DSU to be the source of truth.
    /// For now we keep KarmaEngine as the main owner.
    /// </summary>
    public void PullFromDialogueSystem()
    {
        mobLoyalty = DialogueLua.GetVariable("Karma_MobLoyalty").asFloat;
        policeLoyalty = DialogueLua.GetVariable("Karma_PoliceLoyalty").asFloat;
        mercy = DialogueLua.GetVariable("Karma_Mercy").asFloat;
        ruthlessness = DialogueLua.GetVariable("Karma_Ruthlessness").asFloat;
    }

    /// <summary>
    /// Returns a simple label describing the player's current alignment quadrant.
    /// This will be useful for branching logic (endings, special scenes).
    /// </summary>
    public string GetAlignmentQuadrant()
    {
        // You can tweak these thresholds later.
        float loyaltyThreshold = 20f;
        float moralityThreshold = 20f;

        bool isMobLeaning = mobLoyalty > policeLoyalty + loyaltyThreshold;
        bool isPoliceLeaning = policeLoyalty > mobLoyalty + loyaltyThreshold;
        bool isMerciful = mercy >= moralityThreshold;
        bool isRuthless = ruthlessness >= moralityThreshold;

        if (isMobLeaning && isRuthless)
        {
            return "MobEnforcer";
        }

        if (isMobLeaning && isMerciful)
        {
            return "MobBeliever";
        }

        if (isPoliceLeaning && isRuthless)
        {
            return "DirtyCop";
        }

        if (isPoliceLeaning && isMerciful)
        {
            return "TrueOfficer";
        }

        // Neutral / in-between.
        return "Neutral";
    }

    /// <summary>
    /// This saves all karma values to PlayerData.
    /// This is used for saving karma values outside of single gaming sessions.
    /// </summary>
    public void WriteToPlayerData()
    {
        DataManager.Instance.playerData.mobLoyalty = mobLoyalty;
        DataManager.Instance.playerData.policeLoyalty = policeLoyalty;
        DataManager.Instance.playerData.mercy = mercy;
        DataManager.Instance.playerData.ruthlessness = ruthlessness;

        DataManager.Instance.SavePlayerData();
    }

    /// <summary>
    /// This loads saved karma values from Player Data.
    /// It allows players to use previously saved karma values over multiple sessions.
    /// </summary>
    public void LoadFromPlayerData()
    {
        mobLoyalty = DataManager.Instance.playerData.mobLoyalty;
        policeLoyalty = DataManager.Instance.playerData.policeLoyalty;
        mercy = DataManager.Instance.playerData.mercy;
        ruthlessness = DataManager.Instance.playerData.ruthlessness;

        SynchronizeWithDialogueSystem();
    }

    /// <summary>
    /// Resets all obtained Karma values.
    /// This is for when a player wants to start a new game.
    /// </summary>
    public void ResetKarma()
    {
        mobLoyalty = 0f;
        policeLoyalty = 0f;
        mercy = 0f;
        ruthlessness = 0f;

        SynchronizeWithDialogueSystem();

        // Persist reset to save data
        if (DataManager.Instance != null && DataManager.Instance.playerData != null)
        {
            DataManager.Instance.playerData.mobLoyalty = 0f;
            DataManager.Instance.playerData.policeLoyalty = 0f;
            DataManager.Instance.playerData.mercy = 0f;
            DataManager.Instance.playerData.ruthlessness = 0f;

            DataManager.Instance.SavePlayerData();
        }
    }
}
