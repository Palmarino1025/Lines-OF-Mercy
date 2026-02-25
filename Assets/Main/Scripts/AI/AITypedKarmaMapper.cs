using UnityEngine;

public class AITypedKarmaMapper : MonoBehaviour
{
    [Header("Confidence Fail-Safe")]
    [Range(0f, 1f)]
    //public float minConfidenceToApplyKarma = -20; // "If AI confidence < threshold → apply no karma"

    private PlayerData pd;

    private void Awake()
    {
        // Get PlayerData reference once
        if (GameManager.Instance != null && GameManager.Instance.playerData != null)
        {
            pd = GameManager.Instance.playerData;
        }
        else
        {
            Debug.LogWarning("[AITypedKarmaMapper] No PlayerData found. Karma will not update.");
        }
    }

    public void ApplyKarmaFromAnalysis(AIAnalysisResult result)
    {
        if (result == null || KarmaEngine.Instance == null)
            return;

        if (pd == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.playerData != null)
                pd = GameManager.Instance.playerData;
            else
            {
                Debug.LogWarning("[AITypedKarmaMapper] No PlayerData found.");
                return;
            }
        }

        Debug.Log($"[AITypedKarmaMapper] AI → Tone={result.tone}, Intent={result.intent}, Target={result.target}, Confidence={result.confidence}");

        // Base deltas (NOT total stats — just changes)
        double mobDelta = 0f;
        double policeDelta = 0f;
        double mercyDelta = 0f;
        double ruthDelta = 0f;

        // -------------------------
        // 1️⃣ Tone (Primary Driver)
        // -------------------------
        switch (result.tone)
        {
            case "Empathetic":
                mercyDelta += 2f;
                break;

            case "Aggressive":
                ruthDelta += 2f;
                break;

            case "Manipulative":
                ruthDelta += 1.5f;
                mercyDelta -= 1f;
                break;

            case "Dismissive":
                mercyDelta -= 1f;
                break;

            case "Desperate":
                mercyDelta += 1f;
                break;

            case "Assertive":
                ruthDelta += 1f;
                break;
        }

        // -------------------------
        // 2️⃣ Intent (Secondary)
        // -------------------------
        switch (result.intent)
        {
            case "Help":
                mercyDelta += 1f;
                break;

            case "Control":
            case "Deceive":
                ruthDelta += 1f;
                break;

            case "Deflect":
                mercyDelta -= 0.5f;
                break;
        }

        // -------------------------
        // 3️⃣ Target (Allegiance)
        // -------------------------
        switch (result.target)
        {
            case "PoliceOfficer":
                policeDelta += 1f;
                break;

            case "MobAffiliate":
            case "Criminal":
                mobDelta += 1f;
                break;
        }

        // -------------------------
        // 4️⃣ Confidence Scaling
        // -------------------------
        double confidenceMultiplier = Mathf.Clamp01(result.confidence);

        mobDelta *= confidenceMultiplier;
        policeDelta *= confidenceMultiplier;
        mercyDelta *= confidenceMultiplier;
        ruthDelta *= confidenceMultiplier;

        Debug.Log(
            $"[AITypedKarmaMapper] Scaled Deltas → " +
            $"Mob:{mobDelta:F2}, Police:{policeDelta:F2}, Mercy:{mercyDelta:F2}, Ruth:{ruthDelta:F2}"
        );

        // -------------------------
        // 5️⃣ Apply
        // -------------------------
        KarmaEngine.Instance.ApplyKarmaDelta((float)mobDelta, (float)policeDelta, (float)mercyDelta, (float)ruthDelta);
    }
};
