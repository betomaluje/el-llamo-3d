using SWNetwork;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    [SerializeField] TextMeshPro nicknameText;

    private Transform mainCameraPos;

    private SyncPropertyAgent syncPropertyAgent;
    public static string NICKNAME_PROPERTY = "Nickname";

    private void Start()
    {
        syncPropertyAgent = GetComponentInParent<SyncPropertyAgent>();
    }

    private void OnEnable()
    {
        mainCameraPos = Camera.main.transform;
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

    private void LateUpdate()
    {
        if (mainCameraPos == null)
        {
            return;
        }

        transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + mainCameraPos.forward);
    }

}
