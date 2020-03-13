using Cinemachine;
using UnityEngine;

public class MenuCameras : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;
    public Lobby lobbyScript;
    public GameObject lobbyCanvas;
    public GameObject[] mapCanvas;

    public GameObject uiCanvas;

    private int currentCamera = 0;

    void Start()
    {
        UpdateUIElements();
        UpdateCameras();        
    }

    public void ChangeCamera(int cameraIndex)
    {
        currentCamera = cameraIndex;
        lobbyScript.selectedLevel = cameraIndex;    
        UpdateCameras();
        UpdateUIElements();
    }

    private void UpdateUIElements() 
    {
        uiCanvas.SetActive(currentCamera != 0);
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
            camera.gameObject.SetActive(false);
        }
    }

    public void LobbyReady()
    {
        HideAllCameras();

        uiCanvas.SetActive(false);

        lobbyCanvas.SetActive(true);
        foreach(var canvas in mapCanvas) 
        {
            canvas.SetActive(false);
        }        
    }
}
