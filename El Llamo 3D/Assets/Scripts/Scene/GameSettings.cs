using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    public bool usingNetwork = true;

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
}
