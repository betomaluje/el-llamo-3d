using System.Collections;
using UnityEngine;
using SWNetwork;

namespace BetoMaluje.Sikta
{
    public class PlayerGrab : MonoBehaviour
    {
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

        public void SetupPlayer() 
        {
            networkID = GetComponentInParent<NetworkID>();
            syncPropertyAgent = GetComponentInParent<SyncPropertyAgent>();

            sceneCamera = Camera.main;

            playerAnimations = transform.parent.GetComponentInParent<PlayerAnimations>();

            isPlayerSetup = true;
        }

        private void Update()
        {
            if (!isPlayerSetup) return;

            if (syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue() && target!= null)
            {
                Debug.Log("synced shooting");
                target.Shoot(shootHit);
                Shoot();
            }

            if (syncPropertyAgent.GetPropertyWithName(THROWING).GetBoolValue() && target != null)
            {
                target.Throw(throwForce);
                target = null;
            }

            if (networkID.IsMine)
            {
                isFirePressed = Input.GetMouseButtonDown(0);
                isThrowObjectPressed = Input.GetMouseButtonDown(1);                

                // first we check if we are trying to grab something
                HandleGrabbing();

                // now we check if we are shooting
                bool isShooting = HasGun() && (isFirePressed != lastShootingState);

                playerAnimations.ShootAnim(isShooting);

                if (isShooting)
                {
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