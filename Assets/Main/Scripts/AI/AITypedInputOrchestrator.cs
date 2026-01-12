using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class AITypedInputOrchestrator : MonoBehaviour
{
    [Header("References")]
    public HostedInferenceProvider aiProvider;
    public AITypedKarmaMapper karmaMapper;

    [Header("DSU Variable Names")]
    public string dsuPlayerTextVar = "AI_PlayerText";
    public string dsuToneVar = "AI_Tone";
    public string dsuIntentVar = "AI_Intent";
    public string dsuTargetVar = "AI_Target";
    public string dsuConfidenceVar = "AI_Confidence";
    public string dsuNpcLineVar = "AI_NPC_Line";

    [Header("Context")]
    public string contextTag = "Default"; // set per NPC or per conversation

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

    private IEnumerator RunAnalysis(string playerText)
    {
        // Always write player text into DSU (even if AI fails)
        DialogueLua.SetVariable(dsuPlayerTextVar, playerText);

        AIAnalysisResult result = null;

        yield return aiProvider.AnalyzeTypedInput(playerText, contextTag, (r) =>
        {
            result = r;
        });

        if (result == null)
        {
            yield break;
        }

        // Push AI outputs into DSU so DSU can branch (Option 1).
        DialogueLua.SetVariable(dsuToneVar, result.tone);
        DialogueLua.SetVariable(dsuIntentVar, result.intent);
        DialogueLua.SetVariable(dsuTargetVar, result.target);
        DialogueLua.SetVariable(dsuConfidenceVar, result.confidence);
        DialogueLua.SetVariable(dsuNpcLineVar, string.IsNullOrEmpty(result.npcLine) ? "" : result.npcLine);

        // Apply Karma via caps/rules.
        if (karmaMapper != null)
        {
            karmaMapper.ApplyKarmaFromAnalysis(result);
        }
    }
}
