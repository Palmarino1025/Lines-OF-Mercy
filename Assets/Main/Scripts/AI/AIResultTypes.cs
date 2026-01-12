using System;

[Serializable]
public class AIAnalysisResult
{
    public string tone;        // Empathetic, Neutral, Assertive, Aggressive, Manipulative, Dismissive, Desperate, Silent
    public string intent;       // Help, ExtractTruth, Control, Deceive, Deflect, TestLoyalty, EndConversation
    public string target;       // Civilian, Criminal, PoliceOfficer, MobAffiliate, Victim, AuthorityFigure, Unknown
    public float confidence;    // 0..1
    public string npcLine;      // generated line for DSU to display
}
