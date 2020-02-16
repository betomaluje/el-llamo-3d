using UnityEngine;
using DG.Tweening;
using SWNetwork;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;

    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey;
    [SerializeField] private bool oneTimePress = true;
    [SerializeField] private float timeForLookout = 0.5f;
    [SerializeField] private Vector3 finalZoomPosition;

    private float xRotation = 0f;

    private bool isZoomedOut = false;
    private Vector3 originalPos;

    private NetworkID networkID;

    private Vector2 aiming;
    private bool isZoomPressed = false;

    private void Awake()
    {
        originalPos = Vector3.zero;
        originalPos.y = 1.63f;
    }

    void Start()
    {
        networkID = GetComponentInParent<NetworkID>();

        aiming = new Vector2();

        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;        
    }
    
    void Update()
    {
        if (networkID.IsMine)
        {
            aiming.x = Input.GetAxis("Mouse X");
            aiming.y = Input.GetAxis("Mouse Y");

            Aim(aiming);

            isZoomPressed = Input.GetKeyDown(zoomKey);

            if (oneTimePress)
            {
                CheckOneTimePressZoom(isZoomPressed);
            }
            else
            {
                CheckLongPressZoom(isZoomPressed);
            }
        }        
    }

    private void Aim(Vector2 aim)
    {
        float mouseX = aim.x * mouseSensitivity * Time.deltaTime;
        float mouseY = aim.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // no need for this since we are using Conemachine owns stuff
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void CheckOneTimePressZoom(bool isZoomPressed)
    {
        if (isZoomPressed)
        {
            if (!isZoomedOut)
            {
                ZoomOut();
            }
            else
            {
                RestoreZoom();
            }
        }
    }

    private void CheckLongPressZoom(bool isZoomPressed)
    {
        if (isZoomPressed && !isZoomedOut)
        {
            ZoomOut();  
        }

        if (!isZoomPressed && isZoomedOut)
        {
            RestoreZoom();
        }
    }

    private void ZoomOut()
    {
        isZoomedOut = true;
        cameraTransform.DOLocalMove(finalZoomPosition, timeForLookout).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void RestoreZoom()
    {
        Sequence s = DOTween.Sequence();
        s.Append(cameraTransform.DOLocalMove(originalPos, timeForLookout).SetEase(Ease.OutBack)).SetUpdate(true);
        s.AppendCallback(() => isZoomedOut = false);        
    }

}
