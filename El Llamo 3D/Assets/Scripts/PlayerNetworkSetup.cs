using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using BetoMaluje.Sikta;

public class PlayerNetworkSetup : MonoBehaviour
{
    private NetworkID networkID;

    [SerializeField] private MouseLook mouseLook;
    
    // Start is called before the first frame update
    void Start()
    {
        networkID = GetComponent<NetworkID>();

        if (networkID.IsMine)
        {
            SetupPlayer();
        }
    }

    private void SetupPlayer() 
    {
        // set CameraFollow target
        mouseLook.SetupPlayer();

        PlayerGrab playerGrab = GetComponentInChildren<PlayerGrab>();
        playerGrab.SetupPlayer();
    }
}
