using BetoMaluje.Sikta;
using Cinemachine;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    private Crosshair crosshair;

    protected virtual void Start()
    {
        crosshair = FindObjectOfType<Crosshair>();
        SetupPlayer();
    }

    protected virtual void SetupPlayer()
    {
        LocalPlayerGrab playerGrab = GetComponentInChildren<LocalPlayerGrab>(true);
        playerGrab.SetupPlayer();

        crosshair.SetupPlayer(GetComponent<LocalInputHandler>());

        //AimDebug debugPanel = GameObject.FindWithTag("Debug").GetComponent<AimDebug>();
        //debugPanel.Setup(playerGrab);
    }

    protected void RemoveUnnecessaryContent()
    {
        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
        Destroy(vcam.transform.gameObject);
    }
}
