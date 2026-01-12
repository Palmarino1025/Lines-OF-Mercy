using System.Collections;

public interface IAIProvider
{
    IEnumerator AnalyzeTypedInput(string playerText, string contextTag, System.Action<AIAnalysisResult> onDone);
}
