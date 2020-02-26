using UnityEngine;
using DG.Tweening;
using SWNetwork;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHands;

    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey;

    private float verticleAngle = 0f;
    private float horizontalAngle = 0f;    

    private NetworkID networkID;
    private Transform cameraTransform;
    public Vector2 aiming;
    private bool isZoomPressed = false;
    private CameraFollow cameraFollow;

    void Start()
    {
        networkID = GetComponent<NetworkID>();

        aiming = new Vector2();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;        
    }

    public void SetupPlayer(CameraFollow camFollow) 
    {
        // set CameraFollow target
        Camera mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;

        cameraFollow = camFollow;
    }
    
    void Update()
    {
        if (networkID.IsMine)
        {
            aiming.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            aiming.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;           

            isZoomPressed = Input.GetKeyDown(zoomKey);

            if (isZoomPressed)
            {
                CheckPressZoom();
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
        playerHands.localRotation = Quaternion.Euler(verticleAngle, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void CheckPressZoom()
    {
        if (cameraFollow != null)
        {
            cameraFollow.ChangeCameraType();
        }        
    }    

}
