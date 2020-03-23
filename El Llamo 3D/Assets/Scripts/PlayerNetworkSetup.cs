using BetoMaluje.Sikta;
using SWNetwork;
using UnityEngine;

public class PlayerNetworkSetup : MonoBehaviour
{
    private NetworkID networkID;

    private MouseLook mouseLook;

    // Start is called before the first frame update
    void Start()
    {
        networkID = GetComponent<NetworkID>();

        mouseLook = FindObjectOfType<MouseLook>();

        if (networkID.IsMine)
        {
            SetupPlayer();
        }
    }

    private void SetupPlayer()
    {
        // set CameraFollow target
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.target = transform;

        mouseLook.SetupPlayer(cameraFollow);

        PlayerGrab playerGrab = GetComponentInChildren<PlayerGrab>();
        playerGrab.SetupPlayer();

        //AimDebug debugPanel = GameObject.FindWithTag("Debug").GetComponent<AimDebug>();
        //debugPanel.Setup(playerGrab);

    }
}
