using BetoMaluje.Sikta;
using Cinemachine;
using SWNetwork;
using UnityEngine;

public class PlayerNetworkSetup : MonoBehaviour
{
    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;

    private Crosshair crosshair;

    void Start()
    {
        networkID = GetComponent<NetworkID>();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();

        crosshair = FindObjectOfType<Crosshair>();

        if (networkID.IsMine)
        {
            SetupPlayer();
        }
        else
        {
            RemoveUnnecessaryContent();
        }
    }

    private void SetupPlayer()
    {
        PlayerGrab playerGrab = GetComponentInChildren<PlayerGrab>();
        playerGrab.SetupPlayer();

        crosshair.SetupPlayer(GetComponent<InputHandler>());

        string name = NetworkClient.Instance.PlayerId.Split('-')[1];
        syncPropertyAgent?.Modify(PlayerName.NICKNAME_PROPERTY, name);

        //AimDebug debugPanel = GameObject.FindWithTag("Debug").GetComponent<AimDebug>();
        //debugPanel.Setup(playerGrab);
    }

    private void RemoveUnnecessaryContent()
    {
        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
        Destroy(vcam.transform.gameObject);
    }
}
