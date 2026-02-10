using UnityEngine;
[System.Serializable]

public class NpcRelationshipData
{
    public string npcId;

    [Range(-100f, 100f)]
    public float affinity;   // likes/dislikes player

    [Range(-100f, 100f)]
    public float trust;

    public NpcRelationshipData(string id)
    {
        npcId = id;
        affinity = 0f;
        trust = 0f;
    }
}