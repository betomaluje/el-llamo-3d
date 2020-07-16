using System;
using UnityEngine;

public class LocalInputHandler : MonoBehaviour
{
    [Header("Key Codes")]
    [SerializeField] private KeyCode keyShoot = KeyCode.Mouse0;
    [SerializeField] private KeyCode keyThrow = KeyCode.R;

    [Space]
    [Header("Target")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float grabDistance = 10f;

    [Space]
    [Header("Shooting")]
    [SerializeField] private LayerMask shootingLayer;
    [SerializeField] private float shootingDistance = 100f;

    [Space]
    [Header("Posess")]
    [SerializeField] private LayerMask posessLayer;
    [SerializeField] private float posessDistance = 10f;

    [HideInInspector]
    public Action fireReleaseCallback = delegate { };

    [HideInInspector]
    public Action secondaryClickCallback = delegate { };
    [HideInInspector]
    public Action secondaryReleaseCallback = delegate { };

    [HideInInspector]
    public Action<PointingTarget> targetAquired = delegate { };

    [HideInInspector]
    public Action<ShootingTarget> shootingTarget = delegate { };

    [HideInInspector]
    public Action<PosessTarget> posessAquired = delegate { };

    private Camera sceneCamera;

    protected bool isInputPressed = false;

    private Vector3 centerOfScreen = new Vector3(0.5f, 0.5f, 0.0f);

    private void OnEnable()
    {
        sceneCamera = Camera.main;
    }

    protected virtual void Update()
    {
        // check if user is pressing the Fire button
        if (Input.GetKeyDown(keyShoot))
        {
            isInputPressed = true;
            HandleShootingPointer();
        }

        if (Input.GetKeyUp(keyShoot))
        {
            isInputPressed = false;
            fireReleaseCallback();
        }

        if (Input.GetKeyDown(keyThrow))
        {
            secondaryClickCallback();
        }
        else if (Input.GetKeyUp(keyThrow))
        {
            secondaryReleaseCallback();
        }

        HandleTargetPointer();

        HandlePosessPointer();
    }

    private void HandleShootingPointer()
    {
        Vector3 rayOrigin = sceneCamera.ViewportToWorldPoint(centerOfScreen);
        // check if a target has been clicked on
        RaycastHit hit;

        // this bool check if we hit something that is "shootable" according to the shootingLayer layer mask
        bool onTarget = Physics.Raycast(rayOrigin, sceneCamera.transform.forward, out hit, shootingDistance, shootingLayer);

        Debug.DrawRay(rayOrigin, sceneCamera.transform.forward * shootingDistance, onTarget ? Color.red : Color.black);

        // now we send the Action to all listeners
        shootingTarget(new ShootingTarget(hit, onTarget, shootingDistance, isInputPressed));
    }

    private void HandleTargetPointer()
    {
        Vector3 rayOrigin = sceneCamera.ViewportToWorldPoint(centerOfScreen);
        RaycastHit hit;

        // this bool check if we hit something that is "target" according to the targetLayer layer mask
        bool onTarget = Physics.Raycast(rayOrigin, sceneCamera.transform.forward, out hit, grabDistance, targetLayer);

        Debug.DrawRay(rayOrigin, sceneCamera.transform.forward * shootingDistance, onTarget ? Color.yellow : Color.grey);

        targetAquired(new PointingTarget(hit, onTarget, isInputPressed));
    }

    private void HandlePosessPointer()
    {
        Vector3 rayOrigin = sceneCamera.ViewportToWorldPoint(centerOfScreen);
        RaycastHit hit;

        // this bool check if we hit something that is "target" according to the targetLayer layer mask
        bool onTarget = Physics.Raycast(rayOrigin, sceneCamera.transform.forward, out hit, posessDistance, posessLayer);

        Debug.DrawRay(rayOrigin, sceneCamera.transform.forward * shootingDistance, onTarget ? Color.blue : Color.grey);

        posessAquired(new PosessTarget(hit, onTarget, isInputPressed));
    }
}
