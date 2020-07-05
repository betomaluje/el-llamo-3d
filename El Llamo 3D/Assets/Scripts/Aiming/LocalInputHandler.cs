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
        // check if a target has been clicked on
        RaycastHit hit;
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        // this bool check if we hit something that is "shootable" according to the shootingLayer layer mask
        bool onTarget = Physics.Raycast(ray, out hit, shootingDistance, shootingLayer);

        if (onTarget)
        {
            Debug.DrawRay(ray.origin, ray.direction * shootingDistance, Color.red);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * shootingDistance, Color.black);
        }

        // now we send the Action to all listeners
        shootingTarget(new ShootingTarget(ray, hit, onTarget, shootingDistance, isInputPressed));
    }

    private void HandleTargetPointer()
    {
        RaycastHit hit;
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        // this bool check if we hit something that is "target" according to the targetLayer layer mask
        bool onTarget = Physics.Raycast(ray, out hit, grabDistance, targetLayer);

        if (onTarget)
        {
            Debug.DrawRay(ray.origin, ray.direction * grabDistance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * grabDistance, Color.grey);
        }

        targetAquired(new PointingTarget(hit, onTarget, isInputPressed));
    }

    private void HandlePosessPointer()
    {
        RaycastHit hit;
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        // this bool check if we hit something that is "target" according to the targetLayer layer mask
        bool onTarget = Physics.Raycast(ray, out hit, posessDistance, posessLayer);

        if (onTarget)
        {
            Debug.DrawRay(ray.origin, ray.direction * posessDistance, Color.blue);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * posessDistance, Color.grey);
        }

        posessAquired(new PosessTarget(hit, onTarget, isInputPressed));
    }
}
