using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data", order = 51)]
public class Level : ScriptableObject
{
    public LevelNumber levelNumber;

    /**
     * This Level Numbers should match with the Build Settings added Scenes
     */
    [System.Serializable]
    public enum LevelNumber
    {
        Lobby,
        MultiPlayer,
        Adventure,
        Viking,
        AdventureLocal
    }
}