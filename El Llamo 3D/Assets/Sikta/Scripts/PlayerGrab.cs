using System;
using System.Collections;
using UnityEngine;
using SWNetwork;

namespace BetoMaluje.Sikta
{
    public class PlayerGrab : MonoBehaviour
    {
        [SerializeField] private InputHandler inputHandler;

        [Space]
        [Header("Stats")]
        [SerializeField] private float throwForce = 20;
        [SerializeField] private float timeResetTarget = 1f;

        [Space]
        [Header("Weapon")]
        public ITarget target;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float grabDistance = 10f;

        [Space]
        [Header("Shooting")]
        [SerializeField] private LayerMask shootingLayer;
        [SerializeField] private float shootingDistance = 100f;
        [SerializeField] private float fireRate = 3f;

        private float nextTimeToFire = 0f;

        public Vector3 aimPoint;

        private MaterialColorChanger lastObject;
        private bool hasPointedToObject = false;

        private RaycastHit shootHit;

        private PlayerAnimations playerAnimations;

        #region Network
        private NetworkID networkID;

        private bool isFirePressed = false;
        private bool isThrowObjectPressed = false;

        private Camera sceneCamera;

        private bool isPlayerSetup = false;

        // networ property syncing
        private SyncPropertyAgent syncPropertyAgent;
        const string SHOOTING = "Shooting";
        const string THROWING = "Throwing";
        bool lastShootingState = false;
        bool lastThrowingState = false;
        #endregion

        private void Start() 
        {
            networkID = GetComponentInParent<NetworkID>();
            syncPropertyAgent = GetComponentInParent<SyncPropertyAgent>();

            playerAnimations = transform.parent.GetComponentInParent<PlayerAnimations>();
        }

        public void SetupPlayer() 
        {
            sceneCamera = Camera.main;

            // handles when the player is shooting
            inputHandler.fireClickCallback = () => {
                isFirePressed = true;
            };

            inputHandler.fireReleaseCallback = () => {
                isFirePressed = false;
            };

            // handles when the player is throwing
            inputHandler.secondaryClickCallback = () => {
                isThrowObjectPressed = true;
            };

            inputHandler.secondaryReleaseCallback = () => {
                isThrowObjectPressed = false;
            };

            isPlayerSetup = true;
        }

        private void Update()
        {
            if (!isPlayerSetup) return;

            if (target!= null && syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue())
            {                         
                Debug.Log("synced shooting");
                isFirePressed = false;
                Shoot();
                target.Shoot(shootHit);

                syncPropertyAgent.Modify(SHOOTING, false);
                lastShootingState = false;
            }

            if (target!= null && syncPropertyAgent.GetPropertyWithName(THROWING).GetBoolValue())
            {
                target.Throw(throwForce);
                target = null;

                syncPropertyAgent.Modify(THROWING, false);
                lastThrowingState = false;
            }

            if (networkID.IsMine)
            {
                // first we check if we are trying to grab something
                if (transform.childCount == 0) 
                {
                    HandleGrabbing();
                }                

                if (HasGun()) {
                    // now we check if we are shooting                
                    if (isFirePressed != lastShootingState && Time.time >= nextTimeToFire)
                    {               
                        Debug.Log("Should be shooting: ");                
                        syncPropertyAgent.Modify(SHOOTING, isFirePressed);
                        lastShootingState = isFirePressed;                                   
                    }
                    
                    if (isThrowObjectPressed != lastThrowingState)
                    {                    
                        syncPropertyAgent.Modify(THROWING, isThrowObjectPressed);
                        lastThrowingState = isThrowObjectPressed;
                    }
                }                  
            }
        }

        private void LateUpdate() {
            // we handle the animation
            playerAnimations.ShootAnim(isFirePressed);
        }

        private void HandleGrabbing()
        {
            RaycastHit hit;
            if (Physics.Raycast(sceneCamera.transform.position, sceneCamera.transform.forward, out hit, grabDistance, targetLayer))
            {
                lastObject = hit.transform.GetComponent<MaterialColorChanger>();
                if (lastObject != null && lastObject.isEnabled && !hasPointedToObject)
                {
                    hasPointedToObject = true;
                    lastObject.TargetOn();
                }

                if (isFirePressed && target == null)
                {
                    hit.transform.GetComponent<ITarget>().Pickup(this, transform);
                }
            }
            else
            {
                if (lastObject != null && hasPointedToObject)
                {
                    StartCoroutine(MakeTargetAvailable());
                    lastObject.TargetOff();
                    lastObject = null;
                }
            }
        }

        private void Shoot()
        {            
            // check if we hit something with collider and in our shooting layer            
            Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out shootHit, shootingDistance, shootingLayer, QueryTriggerInteraction.Ignore))
            {
                aimPoint = shootHit.point;

                Health healthTarget = shootHit.transform.GetComponent<Health>();
                GunTarget gunTarget = transform.GetComponentInChildren<GunTarget>();

                if (gunTarget != null && healthTarget != null)
                {
                    healthTarget.PerformDamage(gunTarget.GetDamage());

                    if (shootHit.rigidbody != null)
                    {
                        Debug.Log("impact force! " + gunTarget.impactForce);
                        shootHit.rigidbody.AddForce(-shootHit.normal * gunTarget.impactForce);
                    }
                }
            }
            else
            {
                aimPoint = ray.origin + ray.direction * shootingDistance;
            }

            // we update the frequency of the shooting
            nextTimeToFire = Time.time + 1f / fireRate;
        }

        private bool HasGun()
        {
            return transform.childCount > 0 && transform.GetComponentInChildren<ITarget>().getType().Equals(TargetType.Shootable);
        }

        private IEnumerator MakeTargetAvailable()
        {
            yield return new WaitForSeconds(timeResetTarget);
            hasPointedToObject = false;
        }

    }
}