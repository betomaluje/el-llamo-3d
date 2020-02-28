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

        public Vector3 aimPoint;

        private MaterialColorChanger lastObject;
        private bool hasPointedToObject = false;        

        private PlayerAnimations playerAnimations;

        private RaycastHit shootHit;

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
                    targetHit.transform.GetComponent<ITarget>().Pickup(this, transform);
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

        private void HandleShooting(ShootingTarget shootingTarget)
        {
            isFirePressed = true;

            if (HasGun() && isFirePressed != lastShootingState)
            {
                Debug.Log("Should be shooting: ");
                syncPropertyAgent.Modify(SHOOTING, isFirePressed);
                lastShootingState = isFirePressed;
            }

            // we mark the shooting point
            shootHit = shootingTarget.shootingHit;

            // if it was on target we take damage
            if (shootingTarget.onTarget)
            {
                aimPoint = shootHit.point;

                Health healthTarget = shootingTarget.shootingHit.transform.GetComponent<Health>();
                GunTarget gunTarget = transform.GetComponentInChildren<GunTarget>();

                if (gunTarget != null && healthTarget != null)
                {
                    healthTarget.PerformDamage(gunTarget.GetDamage());

                    if (shootingTarget.shootingHit.rigidbody != null)
                    {
                        Debug.Log("impact force! " + gunTarget.impactForce);
                        shootingTarget.shootingHit.rigidbody.AddForce(-shootingTarget.shootingHit.normal * gunTarget.impactForce);
                    }
                }
            }
            else
            {
                aimPoint = shootingTarget.ray.origin + shootingTarget.ray.direction * shootingTarget.shootingDistance;
            }
        }

        private void Update()
        {
            if (!isPlayerSetup) return;

            if (target!= null && syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue())
            {                         
                Debug.Log("synced shooting");
                target.Shoot(shootHit);

                syncPropertyAgent.Modify(SHOOTING, false);
                lastShootingState = false;
            }

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