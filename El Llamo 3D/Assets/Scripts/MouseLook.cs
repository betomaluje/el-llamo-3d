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

    private void Awake()
    {
        originalPos = Vector3.zero;
        originalPos.y = 1.63f;
    }

    void Start()
    {
        networkID = GetComponentInParent<NetworkID>();

        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;        
    }
    
    void Update()
    {
        if (!networkID.IsMine)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // no need for this since we are using Conemachine owns stuff
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0 , 0);

        playerBody.Rotate(Vector3.up * mouseX);

        if (oneTimePress)
        {
            CheckOneTimePressZoom();
        } else
        {
            CheckLongPressZoom();
        }
    }

    private void CheckOneTimePressZoom()
    {
        if (Input.GetKeyDown(zoomKey))
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

    private void CheckLongPressZoom()
    {
        if (Input.GetKeyDown(zoomKey) && !isZoomedOut)
        {
            ZoomOut();  
        }

        if (Input.GetKeyUp(zoomKey) && isZoomedOut)
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
