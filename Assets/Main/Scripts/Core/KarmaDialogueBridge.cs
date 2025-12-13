using System.Reflection;                           // Needed for GetMethod
using UnityEngine;
using PixelCrushers.DialogueSystem;               // Dialogue System namespace

public class KarmaDialogueBridge : MonoBehaviour
{
    // singleton to access this script directly.
    public static KarmaDialogueBridge Instance;

    private void Awake()
    {
        // Basic singleton setup.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        // Register the AddKarma method as a Lua function called "AddKarma".
        MethodInfo methodInfo = typeof(KarmaDialogueBridge).GetMethod("AddKarma",
            BindingFlags.Public | BindingFlags.Instance);

        if (methodInfo == null)
        {
            Debug.LogError("[KarmaDialogueBridge] Could not find method AddKarma to register with Lua.");
            return;
        }

        Lua.RegisterFunction("AddKarma", this, methodInfo);
        // Now you can call AddKarma(...) from DSU Lua scripts.
    }

    private void OnDisable()
    {
        // Clean up when the object is disabled.
        Lua.UnregisterFunction("AddKarma");
    }

    /// <summary>
    /// This method is called from Lua as: AddKarma(mob, police, mercy, ruthless)
    /// IMPORTANT: Use double for numeric parameters because Lua uses double types.
    /// </summary>
    public void AddKarma(double mobLoyaltyChange,
                         double policeLoyaltyChange,
                         double mercyChange,
                         double ruthlessnessChange)
    {
        if (KarmaEngine.Instance == null)
        {
            Debug.LogWarning("[KarmaDialogueBridge] KarmaEngine.Instance is null. " +
                             "Make sure a KarmaEngine GameObject exists in the scene.");
            return;
        }

        // Convert double (Lua) to float (our KarmaEngine uses float).
        KarmaEngine.Instance.ApplyKarmaDelta(
            (float)mobLoyaltyChange,
            (float)policeLoyaltyChange,
            (float)mercyChange,
            (float)ruthlessnessChange
        );
    }
}
