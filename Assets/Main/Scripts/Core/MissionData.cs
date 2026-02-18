using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Game/Mission")]
public class MissionData : ScriptableObject
{
    public string missionId;                // Unique ID for the mission
    public string missionTitle;             // Short title for HUD or UI
    [TextArea(3, 10)]
    public string missionDescription;       // Long description for mission box
}
