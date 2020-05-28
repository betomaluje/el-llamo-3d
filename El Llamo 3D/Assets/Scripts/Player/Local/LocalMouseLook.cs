using BetoMaluje.Sikta;
using Cinemachine;
using System.Collections;
using UnityEngine;

public class LocalMouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;
    [SerializeField] private LocalPlayerGrab playerGrab;

    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey;
    [SerializeField] private float finalZoom = 10f;

    private float verticleAngle = 0f;

    private CinemachineVirtualCamera vcam;

    public Vector2 aiming;
    private float originalZoom;

    protected virtual void Start()
    {
        vcam = GetComponentInChildren<CinemachineVirtualCamera>(true);

        originalZoom = vcam.m_Lens.FieldOfView;

        aiming = new Vector2();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected virtual void Update()
    {
        if (!GameSettings.instance.usingNetwork)
        {
            aiming.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            aiming.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            if (Input.GetKeyDown(zoomKey))
            {
                StartCoroutine(CheckPressZoom(true));
            }
            else if (Input.GetKeyUp(zoomKey))
            {
                StartCoroutine(CheckPressZoom(false));
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if (!GameSettings.instance.usingNetwork)
        {
            Aim(aiming);
        }
    }

    private void Aim(Vector2 aim)
    {
        float mouseX = aim.x;
        float mouseY = aim.y;

        verticleAngle += -mouseY;
        verticleAngle = Mathf.Clamp(verticleAngle, -90f, 90f);

        playerGrab.GetActiveHand().localRotation = Quaternion.Euler(verticleAngle, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private IEnumerator CheckPressZoom(bool isZoomPressed)
    {
        float targetZoom;

        if (isZoomPressed)
        {
            targetZoom = finalZoom;
        }
        else
        {
            targetZoom = originalZoom;
        }

        float updateSpeedSeconds = 0.2f;
        float elapsed = 0f;
        float currentZoom = vcam.m_Lens.FieldOfView;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            vcam.m_Lens.FieldOfView = Mathf.Lerp(currentZoom, targetZoom, elapsed / updateSpeedSeconds);
            yield return null;
        }
    }
}
