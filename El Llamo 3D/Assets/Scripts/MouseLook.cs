using UnityEngine;
using DG.Tweening;
using SWNetwork;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;

    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey;
    [SerializeField] private bool oneTimePress = true;
    [SerializeField] private float timeForLookout = 0.5f;
    [SerializeField] private Vector3 finalZoomPosition;

    private float verticleAngle = 0f;
    private float horizontalAngle = 0f;

    private bool isZoomedOut = false;
    private Vector3 originalPos;

    private NetworkID networkID;
    private Transform cameraTransform;
    public Vector2 aiming;
    private bool isZoomPressed = false;
    private bool isZoomReleased = false;

    private void Awake()
    {
        originalPos = new Vector3(0f, 2.7f, 1f);
    }

    void Start()
    {
        networkID = GetComponent<NetworkID>();

        aiming = new Vector2();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;        
    }

    public void SetupPlayer() 
    {
        // set CameraFollow target
        Camera mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;

        //cameraTransform.parent = transform;
        //cameraTransform.localPosition = originalPos;
    }
    
    void Update()
    {
        if (networkID.IsMine)
        {
            aiming.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            aiming.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;           

            isZoomPressed = Input.GetKeyDown(zoomKey);
            isZoomReleased = Input.GetKeyUp(zoomKey);

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

    private void LateUpdate()
    {
        if (networkID.IsMine)
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

        horizontalAngle += mouseX;

        cameraTransform.localRotation = Quaternion.Euler(verticleAngle, horizontalAngle, 0);
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

        if (isZoomReleased && isZoomedOut)
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
