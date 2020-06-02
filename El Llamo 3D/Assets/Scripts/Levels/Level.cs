using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data", order = 51)]
public class Level : ScriptableObject
{
    public LevelNumber levelNumber;

    [System.Serializable]
    public enum LevelNumber
    {
        Lobby,
        MultiPlayer,
        SinglePlayer,
        Adventure,
        Viking,
        AdventureLocal
    }
}