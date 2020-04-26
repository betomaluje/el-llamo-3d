using SWNetwork;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHands;

    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey;

    private float verticleAngle = 0f;

    private NetworkID networkID;
    public Vector2 aiming;
    private bool isZoomPressed = false;

    void Start()
    {
        networkID = GetComponent<NetworkID>();

        aiming = new Vector2();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        playerHands.localRotation = Quaternion.Euler(verticleAngle, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void CheckPressZoom()
    {
        Debug.Log("Changing camera zoom");
    }

}
