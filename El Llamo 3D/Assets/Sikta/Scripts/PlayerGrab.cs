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
        [SerializeField] private bool hasInitialWeapon;
        [SerializeField] private Transform playerHand;

        public Vector3 aimPoint;
        [HideInInspector]
        public Action<Vector3> aimPointUpdate;
        [HideInInspector]
        public Action<Vector3> networkAimPointUpdate;

        private MaterialColorChanger lastObject;
        private bool hasPointedToObject = false;        

        private PlayerAnimations playerAnimations;

        private RaycastHit shootHit;

        #region Network

        private bool isFirePressed = false;
        private bool isThrowObjectPressed = false;

        private Camera sceneCamera;

        private bool isPlayerSetup = false;

        // network property syncing
        private SyncPropertyAgent syncPropertyAgent;
        const string SHOOTING = "Shooting";
        const string THROWING = "Throwing";
        bool lastShootingState = false;
        bool lastThrowingState = false;

        #endregion

        private void Start() 
        {
            syncPropertyAgent = GetComponent<SyncPropertyAgent>();

            playerAnimations = GetComponent<PlayerAnimations>();            
        }

        /**
         * This method is called only for the owner of the network
         */ 
        public void SetupPlayer() 
        {
            sceneCamera = Camera.main;            

            inputHandler.fireReleaseCallback = () => {
                isFirePressed = false;                
            };

            // handles when the player is throwing
            inputHandler.secondaryClickCallback = () => {
                isThrowObjectPressed = true;

                if (isThrowObjectPressed != lastThrowingState)
                {
                    Debug.Log("Should be throwing: ");
                    syncPropertyAgent.Modify(THROWING, isThrowObjectPressed);
                    lastThrowingState = isThrowObjectPressed;
                }
            };

            inputHandler.secondaryReleaseCallback = () => {
                isThrowObjectPressed = false;
            };

            // handle target
            inputHandler.targetAquired = HandleTargetAquired;

            // handle shooting
            inputHandler.shootingTarget = HandleShooting;

            isPlayerSetup = true;
        }

        private void HandleTargetAquired(RaycastHit targetHit, bool onTarget)
        {
            if (onTarget)
            {
                lastObject = targetHit.transform.GetComponent<MaterialColorChanger>();
                if (lastObject != null && lastObject.isEnabled && !hasPointedToObject)
                {
                    hasPointedToObject = true;
                    lastObject.TargetOn();
                }

                if (isFirePressed && target == null)
                {
                    targetHit.transform.GetComponent<ITarget>().Pickup(this, playerHand);
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

        /**
         * Handles the shooting state only for the owner of the network 
         */
        private void HandleShooting(ShootingTarget shootingTarget)
        {
            isFirePressed = true;

            // regardless if it is on target or not, we need to sync the shooting action
            if (HasGun() && isFirePressed != lastShootingState)
            {
                syncPropertyAgent.Modify(SHOOTING, isFirePressed);
                lastShootingState = isFirePressed;
            }

            // we mark the shooting point
            shootHit = shootingTarget.shootingHit;

            // if it was on target we take damage
            if (shootingTarget.onTarget)
            {
                aimPoint = shootHit.point;

                Health healthTarget = shootHit.transform.gameObject.GetComponent<Health>();
                GunTarget gunTarget = playerHand.GetComponentInChildren<GunTarget>();
                
                if (gunTarget != null && healthTarget != null)
                {
                    healthTarget.PerformDamage(gunTarget.GetDamage());

                    if (shootHit.rigidbody != null)
                    {
                        Debug.Log(shootHit.transform.gameObject.name + " -> impact force! " + gunTarget.impactForce);
                        shootHit.rigidbody.AddForce(-shootHit.normal * gunTarget.impactForce);
                    }
                }
            }
            else
            {
                aimPoint = shootingTarget.ray.origin + shootingTarget.ray.direction * shootingTarget.shootingDistance;
            }            
      
            aimPointUpdate?.Invoke(aimPoint);
        }

        public void OnShootingChanged()
        {
            if (target != null && syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue())
            {
                target.Shoot(aimPoint);

                networkAimPointUpdate?.Invoke(aimPoint);

                lastShootingState = false;
            }
        }

        private void Update()
        {            
            if (target!= null && syncPropertyAgent.GetPropertyWithName(THROWING).GetBoolValue())
            {
                Debug.Log("synced throwing");
                target.Throw(throwForce);
                target = null;

                syncPropertyAgent.Modify(THROWING, false);
                lastThrowingState = false;
            }            
        }

        private void LateUpdate() {
            // we handle the animation
            playerAnimations.ShootAnim(isFirePressed);

            if (hasInitialWeapon && playerHand.childCount == 1)
            {
                hasInitialWeapon = false;
                playerHand.GetComponentInChildren<ITarget>().Pickup(this, playerHand);
            }
        }        

        private bool HasGun()
        {
            return playerHand.childCount > 0 && playerHand.GetComponentInChildren<ITarget>().getType().Equals(TargetType.Shootable);
        }

        private IEnumerator MakeTargetAvailable()
        {
            yield return new WaitForSeconds(timeResetTarget);
            hasPointedToObject = false;
        }

    }
}