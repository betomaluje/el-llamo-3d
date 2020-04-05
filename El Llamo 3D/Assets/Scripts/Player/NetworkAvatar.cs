using UnityEngine;

[System.Serializable]
public class NetworkAvatar
{
    public string nickname;
    public Color color;

    public NetworkAvatar(string n, Color c)
    {
        nickname = n;
        color = c;
    }

}
