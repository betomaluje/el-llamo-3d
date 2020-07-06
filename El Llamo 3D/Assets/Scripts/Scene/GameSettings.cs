using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    public bool usingNetwork = true;

    public GameType gameType;

    [System.Serializable]
    public enum GameType
    {
        VS_MODE = 0,
        POSESS_MODE = 1
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
