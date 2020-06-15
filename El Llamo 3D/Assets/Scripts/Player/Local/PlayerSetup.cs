using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    protected Crosshair crosshair;

    protected virtual void Awake()
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
}
