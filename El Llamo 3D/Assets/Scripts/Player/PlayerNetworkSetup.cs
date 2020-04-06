using BetoMaluje.Sikta;
using SWNetwork;
using UnityEngine;

public class PlayerNetworkSetup : MonoBehaviour
{
    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;

    private MouseLook mouseLook;

    private Crosshair crosshair;

    // Start is called before the first frame update
    void Start()
    {
        networkID = GetComponent<NetworkID>();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();

        mouseLook = FindObjectOfType<MouseLook>();
        crosshair = FindObjectOfType<Crosshair>();

        if (networkID.IsMine)
        {
            SetupPlayer();
        }
    }

    private void SetupPlayer()
    {
        // set CameraFollow target
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.SetupPlayer(transform);

        mouseLook.SetupPlayer(cameraFollow);

        PlayerGrab playerGrab = GetComponentInChildren<PlayerGrab>();
        playerGrab.SetupPlayer();

        crosshair.SetupPlayer(GetComponent<InputHandler>());

        string name = NetworkClient.Instance.PlayerId.Split('-')[1];
        syncPropertyAgent?.Modify(PlayerName.NICKNAME_PROPERTY, name);

        //AimDebug debugPanel = GameObject.FindWithTag("Debug").GetComponent<AimDebug>();
        //debugPanel.Setup(playerGrab);
    }
}
