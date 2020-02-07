using UnityEngine;
using DG.Tweening;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHand;

    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey;
    [SerializeField] private bool oneTimePress = true;
    [SerializeField] private float timeForLookout = 0.5f;
    [SerializeField] private Vector3 finalZoomPosition;

    private float xRotation = 0f;

    private bool isZoomedOut = false;
    private Vector3 originalPos;

    private void Awake()
    {
        originalPos = Vector3.zero;
        originalPos.y = 1.63f;
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;        
    }
    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Quaternion cameraRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // no need for this since we are using Conemachine owns stuff
        //transform.localRotation = cameraRotation;
        playerHand.localRotation = cameraRotation;

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
        transform.DOLocalMove(finalZoomPosition, timeForLookout).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void RestoreZoom()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(originalPos, timeForLookout).SetEase(Ease.OutBack)).SetUpdate(true);
        s.AppendCallback(() => isZoomedOut = false);        
    }

}
