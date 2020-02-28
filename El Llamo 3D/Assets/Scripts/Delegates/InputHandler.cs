using System;
using System.Collections;
using System.Collections.Generic;
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
    public Action fireClickCallback;
    [HideInInspector]
    public Action fireReleaseCallback;

    [HideInInspector]
    public Action secondaryClickCallback;
    [HideInInspector]
    public Action secondaryReleaseCallback;

    private Camera sceneCamera;

    private void OnEnable() 
    {
        sceneCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            fireClickCallback();
        } else if (Input.GetMouseButtonUp(0)) {
            fireReleaseCallback();
        }

        if (Input.GetMouseButtonDown(1)) {
            secondaryClickCallback();
        } else if (Input.GetMouseButtonUp(1)) {
            secondaryReleaseCallback();
        }

        RaycastHit hit;
        if (Physics.Raycast(sceneCamera.transform.position, sceneCamera.transform.forward, out hit, grabDistance, targetLayer))
        {
        }

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out shootHit, shootingDistance, shootingLayer, QueryTriggerInteraction.Ignore))
        {
        }
    }
}
