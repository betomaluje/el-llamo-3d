using System;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 cameraInitialPosition;
    [Range(0.01f, 0.5f)]
    public float shakeMagnetude = 0.05f;
    [Range(0, 1)]
    public float shakeTime = 0.5f;
    private Camera mainCamera;

    [HideInInspector]
    public Action actionShakeCamera;

    private void Awake()
    {
        actionShakeCamera = () =>
        {
            ShakeIt();
        };
    }

    public void ShakeIt()
    {
        mainCamera = Camera.main;
        cameraInitialPosition = mainCamera.transform.position;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
        Invoke("StopCameraShaking", shakeTime);
    }

    void StartCameraShaking()
    {
        float cameraShakingOffsetX = UnityEngine.Random.value * shakeMagnetude * 2 - shakeMagnetude;
        float cameraShakingOffsetY = UnityEngine.Random.value * shakeMagnetude * 2 - shakeMagnetude;
        Vector3 cameraIntermadiatePosition = mainCamera.transform.position;
        cameraIntermadiatePosition.x += cameraShakingOffsetX;
        cameraIntermadiatePosition.y += cameraShakingOffsetY;
        mainCamera.transform.position = cameraIntermadiatePosition;
    }

    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        mainCamera.transform.position = cameraInitialPosition;
    }

}
