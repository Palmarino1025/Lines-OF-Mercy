using System.Collections.Generic;

[System.Serializable]

public class PlayerData
{
    public string playerName;

    // Karma

    public float mobLoyalty ;
    public float policeLoyalty ;
    public float mercy ;
    public float ruthlessness ;
    public float questProg ;
    public string currentMissionText;

    // Accessibility / UI

    public float textSpeed;
    public float brightness;

    // NPC Relationships

    public List<NpcRelationshipData> npcRelationships = new();
}