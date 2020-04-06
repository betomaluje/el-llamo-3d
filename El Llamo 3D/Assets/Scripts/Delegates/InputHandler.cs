using SWNetwork;
using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
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

    #region Network

    private NetworkID networkID;

    #endregion

    private void Start()
    {
        networkID = GetComponent<NetworkID>();
    }

    private void OnEnable()
    {
        sceneCamera = Camera.main;
    }

    void Update()
    {
        // only the owner should be able to sync the aim and shooting of the mouse
        if (!networkID.IsMine)
        {
            return;
        }

        // check if user is pressing the Fire button
        if (Input.GetMouseButtonDown(0))
        {
            // check if a target has been clicked on
            RaycastHit shootHit;
            Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

            // this bool check if we hit something that is "shootable" according to the shootingLayer layer mask
            bool onTarget = Physics.Raycast(ray, out shootHit, shootingDistance, shootingLayer, QueryTriggerInteraction.Ignore);

            // now we send the Action to all listeners
            shootingTarget(new ShootingTarget(ray, shootHit, onTarget, shootingDistance));
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

        RaycastHit targetHit;
        Ray targetRay = sceneCamera.ScreenPointToRay(Input.mousePosition);

        // this bool check if we hit something that is "target" according to the targetLayer layer mask
        bool onTargetTarget = Physics.Raycast(targetRay, out targetHit, grabDistance, targetLayer, QueryTriggerInteraction.Ignore);
        targetAquired(targetHit, onTargetTarget);
    }
}
