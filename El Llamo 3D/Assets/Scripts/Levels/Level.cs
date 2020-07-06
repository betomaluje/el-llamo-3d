using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data", order = 51)]
public class Level : ScriptableObject
{
    [Tooltip("This Level Numbers should match with the Build Settings added Scenes")]
    public LevelNumber levelNumber;

    public bool usesNetwork = true;

    public GameSettings.GameType gameType;

    public int numberOfPlayers = 2;

    /**
     * This Level Numbers should match with the Build Settings added Scenes
     */
    [System.Serializable]
    public enum LevelNumber
    {
        Lobby,
        Adventure,
        Viking,
        AdventureLocal,
        VikingLocal,
        TestScene,
        TestNetwork
    }
}