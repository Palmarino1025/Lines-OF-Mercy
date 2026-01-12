using UnityEngine;

public class AITypedKarmaMapper : MonoBehaviour
{
    [Header("Confidence Fail-Safe")]
    [Range(0f, 1f)]
    public float minConfidenceToApplyKarma = 0.55f; // "If AI confidence < threshold → apply no karma"

    public void ApplyKarmaFromAnalysis(AIAnalysisResult result)
    {
        if (result == null || KarmaEngine.Instance == null)
        {
            return;
        }

        if (result.confidence < minConfidenceToApplyKarma)
        {
            // Fail-safe: apply nothing
            return;
        }

        float mob = 0f;
        float police = 0f;
        float mercy = 0f;
        float ruth = 0f;

        // ---- Base: Tone (primary driver)
        // keep this conservative (caps are enforced by values chosen).
        if (result.tone == "Empathetic")
        {
            mercy = 3f; // Mercy +2 to +3
        }
        else if (result.tone == "Neutral")
        {
            // often zero
        }
        else if (result.tone == "Assertive")
        {
            // context-dependent; default small Ruth +1 (investigative pressure)
            ruth = 1f;
        }
        else if (result.tone == "Aggressive")
        {
            ruth = 3f; // Ruth +2 to +3
        }
        else if (result.tone == "Manipulative")
        {
            ruth = 2f;
            mercy = -1f; // would be 2 stats already (ruth + mercy)
        }
        else if (result.tone == "Dismissive")
        {
            mercy = -2f;
        }
        else if (result.tone == "Desperate")
        {
            mercy = 1f;
        }
        else if (result.tone == "Silent" || result.tone == "Avoidant" || result.tone == "Silent / Avoidant")
        {
            mercy = -1f;
        }

        // Intent modifier (secondary)
        // Only apply if we are not going to exceed "2 stats affected".
        if (result.intent == "Help")
        {
            if (mercy == 0f && ruth == 0f)
            {
                mercy = 1f;
            }
            else if (mercy > 0f)
            {
                mercy = Mathf.Clamp(mercy + 1f, -3f, 3f);
            }
        }
        else if (result.intent == "Control")
        {
            if (ruth > 0f)
            {
                ruth = Mathf.Clamp(ruth + 1f, -3f, 3f);
            }
        }
        else if (result.intent == "Deceive")
        {
            if (ruth == 0f && mercy == 0f)
            {
                ruth = 1f;
            }
            else if (ruth > 0f)
            {
                ruth = Mathf.Clamp(ruth + 1f, -3f, 3f);
            }
        }
        else if (result.intent == "Deflect")
        {
            if (mercy == 0f)
            {
                mercy = -1f;
            }
            else
            {
                mercy = Mathf.Clamp(mercy - 1f, -3f, 3f);
            }
        }

        // Target adjustments (who is being spoken to)
        // Only ONE allegiance stat max (+/-1) so we don't exceed the “2 stats” rule.
        if (result.target == "PoliceOfficer")
        {
            // police loyalty is more sensitive
            if (police == 0f && (mob == 0f))
            {
                police = 1f;
            }
        }
        else if (result.target == "MobAffiliate" || result.target == "Criminal")
        {
            if (mob == 0f && (police == 0f))
            {
                // Only grant if the tone supports it
                if (result.tone == "Aggressive" || result.tone == "Manipulative" || result.tone == "Assertive")
                {
                    mob = 1f;
                }
            }
        }
        else if (result.target == "Victim" || result.target == "Civilian")
        {
            // We keep allegiance unchanged; mercy/ruth already expresses the effect.
        }

        // Enforce “max 2 stats affected” strictly
        int affected = 0;
        if (mob != 0f) affected++;
        if (police != 0f) affected++;
        if (mercy != 0f) affected++;
        if (ruth != 0f) affected++;

        if (affected > 2)
        {
            // Priority: morality stats first (Mercy/Ruth), then allegiance.
            mob = 0f;
            police = 0f;
        }

        KarmaEngine.Instance.ApplyKarmaDelta(mob, police, mercy, ruth);
    }
}
