using System.Collections.Generic;

[System.Serializable]

public class PlayerData
{
    public string playerName;

    // Karma

    public float mobLoyalty;
    public float policeLoyalty;
    public float mercy;
    public float ruthlessness;

    // NPC Relationships

    public List<NpcRelationshipData> npcRelationships = new();
}