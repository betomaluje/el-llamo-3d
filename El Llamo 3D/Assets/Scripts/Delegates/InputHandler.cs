using System;
using UnityEngine;
using SWNetwork;

public class InputHandler : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float grabDistance = 10f;

    [Space]
    [Header("Shooting")]
    [SerializeField] private LayerMask shootingLayer;
    [SerializeField] private float shootingDistance = 100f;    
    [SerializeField] private float fireRate = 3f;
    private float nextTimeToFire = 0f;
    
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

    private NetworkID networkID;

    private void Start()
    {
        networkID = GetComponentInParent<NetworkID>();
    }

    private void OnEnable() 
    {
        sceneCamera = Camera.main;
    }

    void Update()
    {
        if (!networkID.IsMine) return;

        // check if user is pressing the Fire button and if it was pointing to a target
        if (Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire)
        {
            // check if something has been clicked on
            RaycastHit shootHit;
            Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out shootHit, shootingDistance, shootingLayer, QueryTriggerInteraction.Ignore))
            {
                shootingTarget(new ShootingTarget(ray, shootHit, true, shootingDistance));
            }
            else
            {
                shootingTarget(new ShootingTarget(ray, shootHit, false, shootingDistance));
            }

            // we update the frequency of the shooting
            nextTimeToFire = Time.time + 1f / fireRate;
        }       

        if (Input.GetMouseButtonUp(0)) {
            fireReleaseCallback();
        }

        if (Input.GetMouseButtonDown(1)) {
            secondaryClickCallback();
        } else if (Input.GetMouseButtonUp(1)) {
            secondaryReleaseCallback();
        }

        RaycastHit targetHit;
        if (Physics.Raycast(sceneCamera.transform.position, sceneCamera.transform.forward, out targetHit, grabDistance, targetLayer))
        {
            targetAquired(targetHit, true);
        } else
        {
            targetAquired(targetHit, false);
        }        
    }
}
