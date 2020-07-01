using Cinemachine;
using System.Collections;
using UnityEngine;

public class LocalMouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;
    [SerializeField] private GrabController grabController;

    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey;
    [SerializeField] private float finalZoom = 10f;

    [Space]
    [Header("3rd person")]
    [SerializeField] private KeyCode thirdPersonKey;
    [SerializeField] private Vector3 finalThirdPosition;
    [SerializeField] private GameObject[] objectsToHide;

    [Space]
    [Header("1st person")]
    [SerializeField] private Vector3 finalFirstPosition = new Vector3(0, 2.4f, 1);

    [HideInInspector]
    public Vector2 aiming;

    private float verticleAngle = 0f;

    private CinemachineVirtualCamera vcam;

    private float originalZoom;

    private bool isFirstPerson = true;

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

        if (Input.GetKeyDown(thirdPersonKey))
        {
            if (isFirstPerson)
            {
                // toggle to 3rd person
                StartCoroutine(ChangeToThirdPersonCamera());
            }
            else
            {
                // toggle to 1st person
                StartCoroutine(ChangeToFirstPersonCamera());
            }
        }
    }

    private IEnumerator ChangeToFirstPersonCamera()
    {
        if (!isFirstPerson)
        {
            ShowExtras();

            isFirstPerson = true;

            float updateSpeedSeconds = 0.2f;
            float elapsed = 0f;
            Vector3 currentPosition = vcam.transform.localPosition;

            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                vcam.transform.localPosition = Vector3.Lerp(currentPosition, finalFirstPosition, elapsed / updateSpeedSeconds);
                yield return null;
            }
        }
    }

    private IEnumerator ChangeToThirdPersonCamera()
    {
        if (isFirstPerson)
        {
            HideExtras();

            isFirstPerson = false;

            float updateSpeedSeconds = 0.2f;
            float elapsed = 0f;
            Vector3 currentPosition = vcam.transform.localPosition;

            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                vcam.transform.localPosition = Vector3.Lerp(currentPosition, finalThirdPosition, elapsed / updateSpeedSeconds);
                yield return null;
            }
        }
    }

    private void HideExtras()
    {
        if (objectsToHide.Length <= 0)
        {
            return;
        }

        foreach (var toHide in objectsToHide)
        {
            toHide?.SetActive(false);
        }
    }

    private void ShowExtras()
    {
        if (objectsToHide.Length <= 0)
        {
            return;
        }

        foreach (var toShow in objectsToHide)
        {
            toShow?.SetActive(true);
        }
    }

    protected virtual void LateUpdate()
    {
        Aim(aiming);
    }

    private void Aim(Vector2 aim)
    {
        float mouseX = aim.x;
        float mouseY = aim.y;

        verticleAngle += -mouseY;
        verticleAngle = Mathf.Clamp(verticleAngle, -90f, 90f);

        Transform hand = grabController?.GetActiveHand();
        if (hand != null)
        {
            hand.localRotation = Quaternion.Euler(verticleAngle, 0, 0);
        }

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
