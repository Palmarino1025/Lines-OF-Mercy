using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Diagnostics;

public class DialogueLuaBridge : MonoBehaviour
{
    private static bool registered = false;

    private void Awake()
    {
        if (registered) return;

        Lua.RegisterFunction(
            "EvaluateNpcAlignment",
            this,
            typeof(DialogueLuaBridge).GetMethod(nameof(EvaluateNpcAlignment))
        );

        registered = true;

        DontDestroyOnLoad(gameObject);

        UnityEngine.Debug.Log("[LuaBridge] EvaluateNpcAlignment registered.");
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("EvaluateNpcAlignment");
    }

    public void EvaluateNpcAlignment()
    {

        Transform npcTransform = DialogueManager.currentConversant;

        if (npcTransform == null)
        {
            UnityEngine.Debug.LogWarning("[LuaBridge] No NPC found.");
            return;
        }

        NpcInteraction npc = npcTransform.GetComponent<NpcInteraction>();

        if (npc == null)
        {
            UnityEngine.Debug.LogWarning("[LuaBridge] NPC has no NpcInteraction.");
            return;
        }

        //npc.EvaluatePlayerAlignment();
    }
}