using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Sends player-typed text to a hosted AI endpoint (Hugging Face)
/// and returns structured analysis used by the game.
/// </summary>
public class HostedInferenceProvider : MonoBehaviour, IAIProvider
{
    [Header("Hosted Inference Settings")]

    // Full URL to Hugging Face Space endpoint (must end with /analyze)
    public string endpointUrl = "https://itsjustmeh-lines-of-mercy-ai.hf.space/analyze";

    // Optional bearer token (leave empty if Space is public)
    public string bearerToken = "";

    // Timeout in seconds (keep under Hugging Face limits)
    public float timeoutSeconds = 4.5f;

    
    // Payload sent to the AI backend.
    [Serializable]
    private class RequestPayload
    {
        public string text;        // Player's typed input
        public string contextTag;  // NPC / scene / conversation context
    }


    // Sends text to the AI and returns the analysis result.
    public IEnumerator AnalyzeTypedInput(
        string playerText,
        string contextTag,
        Action<AIAnalysisResult> onDone
    )
    {
        // Fallback result used if anything fails
        AIAnalysisResult fallbackResult = new AIAnalysisResult();
        fallbackResult.tone = "Neutral";
        fallbackResult.intent = "ExtractTruth";
        fallbackResult.target = "Unknown";
        fallbackResult.confidence = 0f;
        fallbackResult.npcLine = "";

        // Stop if endpoint is missing
        if (string.IsNullOrEmpty(endpointUrl))
        {
            Debug.LogWarning("[HostedInferenceProvider] Endpoint URL is not set.");
            onDone?.Invoke(fallbackResult);
            yield break;
        }

        // Log endpoint for debugging connectivity issues
        Debug.Log("[HostedInferenceProvider] Sending request to: " + endpointUrl);

        // Build request payload
        RequestPayload payload = new RequestPayload();
        payload.text = playerText;
        payload.contextTag = contextTag;

        // Convert payload to JSON
        string jsonBody = JsonUtility.ToJson(payload);
        byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);

        // Create HTTP POST request
        using (UnityWebRequest request = new UnityWebRequest(endpointUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Required headers
            request.SetRequestHeader("Content-Type", "application/json");

            // Add Authorization header if token is provided
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            }

            // Apply timeout
            request.timeout = Mathf.CeilToInt(timeoutSeconds);

            // Send the request
            yield return request.SendWebRequest();

            // Handle network errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(
                    "[HostedInferenceProvider] Request failed: " +
                    request.error
                );

                onDone?.Invoke(fallbackResult);
                yield break;
            }

            // Read response JSON
            string responseJson = request.downloadHandler.text;

            // Parse response
            AIAnalysisResult parsedResult = null;

            try
            {
                parsedResult = JsonUtility.FromJson<AIAnalysisResult>(responseJson);
            }
            catch (Exception parseException)
            {
                Debug.LogWarning(
                    "[HostedInferenceProvider] JSON parse error: " +
                    parseException.Message
                );
            }

            // Use fallback if parsing failed
            if (parsedResult == null)
            {
                onDone?.Invoke(fallbackResult);
                yield break;
            }

            // Return successful result
            onDone?.Invoke(parsedResult);
        }
    }
}
