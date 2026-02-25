using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public enum DialogueDirection
{
    Good,
    Neutral,
    Bad,
    Unknown
}

public class AITypedInputOrchestrator : MonoBehaviour
{
    [Header("References")]
    public HostedInferenceProvider aiProvider;
    public AITypedKarmaMapper karmaMapper;
    private PlayerData playerData;

    [Header("DSU Variable Names")]
    public string dsuPlayerTextVar = "AI_PlayerText";
    public string dsuToneVar = "AI_Tone";
    public string dsuIntentVar = "AI_Intent";
    public string dsuTargetVar = "AI_Target";
    public string dsuConfidenceVar = "AI_Confidence";
    public string dsuNpcLineVar = "AI_NPC_Line";

    [Header("Context")]
    public string contextTag = "Default"; // set per NPC or per conversation
                                          // persona key to select JSON persona on the Hugging Face backend
    public string personaKey = "default"; // ex: "mob_rico", "cop_holt"

    public DialogueDirection branch = DialogueDirection.Neutral;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            playerData = GameManager.Instance.playerData;
        }
        else
        {
            Debug.LogWarning("[AITypedInputOrchestrator] GameManager instance not found. Karma updates will fail.");
        }
    }

    public void AnalyzeAndApply(string playerText)
    {
        if (string.IsNullOrWhiteSpace(playerText))
        {
            return;
        }

        if (aiProvider == null)
        {
            Debug.LogWarning("[AITypedInputOrchestrator] aiProvider is not assigned.");
            return;
        }

        StartCoroutine(RunAnalysis(playerText));
    }

    private PlayerData GetPlayerData()
    {
        if (playerData == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.playerData != null)
            {
                playerData = GameManager.Instance.playerData;
            }
            else
            {
                Debug.LogWarning("[AITypedInputOrchestrator] No PlayerData available. Karma will not update.");
                return null;
            }
        }
        return playerData;
    }

    /// <summary>
    /// Maps Hugging Face AI intents to DialogueDirection branch type.
    /// Anything that doesn't make sense or is unrecognized → Unknown.
    /// </summary>
    DialogueDirection MapIntentToBranch(string intent)
    {
        if (string.IsNullOrEmpty(intent))
            return DialogueDirection.Unknown;

        switch (intent)
        {
            // ------------------------
            // Positive / Good Intent
            // ------------------------
            case "Help":
            case "ExtractTruth":
            case "TestLoyalty":
                return DialogueDirection.Good;

            // ------------------------
            // Negative / Bad Intent
            // ------------------------
            case "Control":
            case "Deceive":
            case "Deflect":
                return DialogueDirection.Bad;

            // ------------------------
            // EndConversation or unexpected / nonsensical
            // ------------------------
            case "EndConversation":
                return DialogueDirection.Unknown;

            default:
                return DialogueDirection.Unknown;
        }
    }


    private IEnumerator RunAnalysis(string playerText)
    {
        // Always write player text into DSU (even if AI fails)
        DialogueLua.SetVariable(dsuPlayerTextVar, playerText);

        AIAnalysisResult result = null;
        DialogueLua.SetVariable("AI_Ready", false);

        yield return aiProvider.AnalyzeTypedInput(playerText, contextTag, personaKey, (r) =>
        {
            result = r;
        });

        if (result == null)
        {
            yield break;
        }

        Debug.Log(
            "AI RESPONSE >> " +
            "Persona=" + personaKey +
            " | Tone=" + result.tone +
            " | Intent=" + result.intent +
            " | Target=" + result.target +
            " | Confidence=" + result.confidence +
            " | NPC Line=\"" + result.npcLine + "\""
            );


        PlayerData pd = GetPlayerData();

        // Push AI outputs into DSU so DSU can branch (Option 1).
        DialogueLua.SetVariable(dsuToneVar, result.tone);
        DialogueLua.SetVariable(dsuIntentVar, result.intent);
        DialogueLua.SetVariable(dsuTargetVar, result.target);
        DialogueLua.SetVariable(dsuConfidenceVar, result.confidence);
        DialogueLua.SetVariable(dsuNpcLineVar, string.IsNullOrEmpty(result.npcLine) ? "" : result.npcLine);

        karmaMapper.ApplyKarmaFromAnalysis(result);

        // Determine branch using AI intent
        branch = MapIntentToBranch(result.intent);

        // Set DSU variable for branching
        KarmaDialogueBridge.Instance.SetNextBranch(branch.ToString());
        DialogueLua.SetVariable("AI_Ready", true);
        // Manually continue conversation
        Debug.Log("[AIBranch] Player typed response → Branch: " + branch);
    }
}
