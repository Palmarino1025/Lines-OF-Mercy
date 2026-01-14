using UnityEngine;
[System.Serializable]

public class NpcPreferenceProfile
{
    [Range(-1f, 1f)] public float mobLoyaltyWeight;
    [Range(-1f, 1f)] public float policeLoyaltyWeight;
    [Range(-1f, 1f)] public float mercyWeight;
    [Range(-1f, 1f)] public float ruthlessnessWeight;
}