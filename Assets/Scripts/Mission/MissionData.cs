using UnityEngine;

[CreateAssetMenu(fileName = "NewMissionData", menuName = "Missions/Data")]
public class MissionData : ScriptableObject
{
    public string description;
    public string title;
    public string id;
    public Vector3 respawnPosition;
    public bool respawnPlayer = false;
}