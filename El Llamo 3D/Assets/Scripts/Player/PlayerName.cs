using SWNetwork;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshPro nicknameText;

    private SyncPropertyAgent syncPropertyAgent;
    public static string NICKNAME_PROPERTY = "Nickname";

    private void Start()
    {
        syncPropertyAgent = GetComponentInParent<SyncPropertyAgent>();
    }

    public void UpdateName()
    {
        string nickname = syncPropertyAgent.GetPropertyWithName(NICKNAME_PROPERTY).GetStringValue();
        nicknameText.text = nickname;
    }

    public void OnNicknameReady()
    {
        string nickname = syncPropertyAgent.GetPropertyWithName(NICKNAME_PROPERTY).GetStringValue();
        nicknameText.text = nickname;
    }
}
