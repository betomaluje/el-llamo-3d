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

    public void SetUsingNetwork(bool useNetwork)
    {
        usingNetwork = useNetwork;
    }

    public void SetGameType(int type)
    {
        switch (type)
        {
            case 0:
                gameType = GameType.VS_MODE;
                break;
            case 1:
                gameType = GameType.POSESS_MODE;
                break;
            default:
                gameType = GameType.VS_MODE;
                break;
        }
    }
}
