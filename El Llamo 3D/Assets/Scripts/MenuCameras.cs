using Cinemachine;
using UnityEngine;

public class MenuCameras : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;
    public Lobby lobbyScript;
    public GameObject lobbyCanvas;
    public GameObject mapCanvas;

    private int currentCamera = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCameras();
    }

    public void ChangeCamera(int cameraIndex)
    {
        currentCamera = cameraIndex;
        lobbyScript.selectedLevel = cameraIndex;
        UpdateCameras();
    }

    private void UpdateCameras()
    {
        int i = 0;
        foreach (CinemachineVirtualCamera camera in cameras)
        {
            camera.gameObject.SetActive(i == currentCamera);
            i++;
        }
    }

    private void HideAllCameras()
    {
        foreach (CinemachineVirtualCamera camera in cameras)
        {
            camera.enabled = false;
        }
    }

    public void LobbyReady()
    {
        HideAllCameras();
        lobbyCanvas.SetActive(true);
        mapCanvas.SetActive(false);
    }
}
