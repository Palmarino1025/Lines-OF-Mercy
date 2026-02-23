using UnityEngine;

[CreateAssetMenu(fileName = "MissionDatabase", menuName = "Game/MissionDatabase")]
public class MissionDatabase : ScriptableObject
{
    public MissionData[] allMissions;

    public MissionData GetMissionById(string id)
    {
        foreach (var mission in allMissions)
        {
            if (mission.missionId == id)
                return mission;
        }

        Debug.LogWarning($"Mission with ID '{id}' not found!");
        return null;
    }
}
