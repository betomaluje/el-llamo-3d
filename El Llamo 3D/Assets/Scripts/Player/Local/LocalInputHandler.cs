using System;
using UnityEngine;

public class LocalInputHandler : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float grabDistance = 10f;

    [Space]
    [Header("Shooting")]
    [SerializeField] private LayerMask shootingLayer;
    [SerializeField] private float shootingDistance = 100f;

    [HideInInspector]
    public Action fireReleaseCallback;

    [HideInInspector]
    public Action secondaryClickCallback;
    [HideInInspector]
    public Action secondaryReleaseCallback;

    [HideInInspector]
    public Action<RaycastHit, bool> targetAquired;

    [HideInInspector]
    public Action<ShootingTarget> shootingTarget;

    private Camera sceneCamera;

    private void OnEnable()
    {
        sceneCamera = Camera.main;
    }

    protected virtual void Update()
    {
        // check if user is pressing the Fire button
        if (Input.GetMouseButtonDown(0))
        {
            HandleShootingPointer();
        }

        if (Input.GetMouseButtonUp(0))
        {
            fireReleaseCallback();
        }

        if (Input.GetMouseButtonDown(1))
        {
            secondaryClickCallback();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            secondaryReleaseCallback();
        }

        HandleTargetPointer();
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
            Debug.DrawRay(ray.origin, ray.direction * shootingDistance, Color.grey);
        }

        // now we send the Action to all listeners
        shootingTarget(new ShootingTarget(ray, hit, onTarget, shootingDistance));
    }

    private void HandleTargetPointer()
    {
        RaycastHit hit;
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        // this bool check if we hit something that is "target" according to the targetLayer layer mask
        bool onTarget = Physics.Raycast(ray, out hit, grabDistance, targetLayer);

        if (onTarget)
        {
            Debug.DrawRay(ray.origin, ray.direction * shootingDistance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * shootingDistance, Color.grey);
        }

        targetAquired(hit, onTarget);
    }
}
