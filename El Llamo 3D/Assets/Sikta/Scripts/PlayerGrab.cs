using SWNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetoMaluje.Sikta
{
    public class PlayerGrab : MonoBehaviour
    {
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private PlayerAnimationTrigger playerAnimationsTrigger;
        [SerializeField] private Vector3 handsOffset;

        [Space]
        [Header("Stats")]
        [SerializeField] private float throwForce = 20;
        [SerializeField] private float timeResetTarget = 1f;

        [Space]
        [Header("Weapon")]
        public List<Grabable> grabbables;
        [SerializeField] private bool hasInitialWeapon;
        [SerializeField] private Transform playerHand;
        [SerializeField] private int maxGrabbables = 2;
        [SerializeField] private KeyCode weaponChanger = KeyCode.E;

        [HideInInspector]
        public Vector3 aimPoint;
        [HideInInspector]
        public Action<Vector3> aimPointUpdate;
        [HideInInspector]
        public Action<Vector3> networkAimPointUpdate;

        private MaterialColorChanger lastObject;
        private bool hasPointedToObject = false;

        // 0: right, 1 left
        int selectedGrabbable = 0;

        private PlayerAnimations playerAnimations;

        #region Network

        private bool isFirePressed = false;

        private Camera sceneCamera;

        private bool isPlayerSetup = false;

        // network property syncing
        private SyncPropertyAgent syncPropertyAgent;
        private RemoteEventAgent remoteEventAgent;
        private const string SHOOTING = "Shooting";
        bool lastShootingState = false;

        #endregion

        private void Start()
        {
            syncPropertyAgent = GetComponent<SyncPropertyAgent>();
            remoteEventAgent = GetComponent<RemoteEventAgent>();

            playerAnimations = GetComponent<PlayerAnimations>();

            grabbables = new List<Grabable>();
        }

        /**
         * This method is called only for the owner of the network
         */
        public void SetupPlayer()
        {
            sceneCamera = Camera.main;

            inputHandler.fireReleaseCallback = () =>
            {
                isFirePressed = false;
            };

            // handles when the player is throwing
            inputHandler.secondaryClickCallback = () =>
            {
                if (playerHand.childCount > 0)
                {
                    playerAnimations.Throw();
                }
            };

            inputHandler.secondaryReleaseCallback = () =>
            {

            };

            // handle actual object throwing
            playerAnimationsTrigger.throwTriggeredCallback = () =>
            {
                ThrowObject(selectedGrabbable);
            };

            // handle target
            inputHandler.targetAquired += HandleTargetAquired;

            // handle shooting
            inputHandler.shootingTarget = HandleShooting;

            isPlayerSetup = true;
        }

        private void OnDisable()
        {
            inputHandler.targetAquired -= HandleTargetAquired;
        }

        private void HandleTargetAquired(RaycastHit targetHit, bool onTarget)
        {
            // if we already have a gun
            if (grabbables.Count > maxGrabbables)
            {
                return;
            }

            if (onTarget)
            {
                lastObject = targetHit.transform.GetComponentInChildren<MaterialColorChanger>();

                if (lastObject == null)
                {
                    // we try in the parents game object
                    lastObject = Utils.GetComponentInParents<MaterialColorChanger>(targetHit.transform.gameObject);
                }

                if (lastObject != null && lastObject.isEnabled && !hasPointedToObject)
                {
                    hasPointedToObject = true;
                    lastObject.TargetOn();
                }

                if (isFirePressed)
                {
                    Grabable grabable = targetHit.transform.GetComponentInChildren<Grabable>();
                    if (grabable != null && !grabable.isGrabbed())
                    {
                        if (lastObject != null)
                        {
                            lastObject.TargetOff();
                        }

                        PickupObject(grabable);
                        lastObject = null;
                    }
                }
            }
            else
            {
                if (lastObject != null && hasPointedToObject)
                {
                    StartCoroutine(MakeTargetAvailable());
                    lastObject.TargetOff();
                }

                lastObject = null;
            }
        }

        private void PickupObject(Grabable grabable)
        {
            Vector3 localPosition = Vector3.zero;

            if (selectedGrabbable == 1)
            {
                Debug.Log("pickup changing hands");
                localPosition = handsOffset;
            }

            grabable.StartPickup(playerHand.position, localPosition);
        }

        /**
         * Handles the shooting state only for the owner of the network 
         */
        private void HandleShooting(ShootingTarget shootingTarget)
        {
            isFirePressed = true;

            // if it doesn't have a gun, we return quickly
            if (!HasGun())
            {
                return;
            }

            // regardless if it is on target or not, we need to sync the shooting action
            if (isFirePressed != lastShootingState)
            {
                syncPropertyAgent.Modify(SHOOTING, isFirePressed);
                lastShootingState = isFirePressed;
            }

            // we mark the shooting point
            RaycastHit shootHit = shootingTarget.shootingHit;

            // if it was on target we take damage
            if (shootingTarget.onTarget)
            {
                aimPoint = shootHit.point;

                Health healthTarget = shootHit.transform.gameObject.GetComponent<Health>();
                Gun gunTarget = playerHand.GetComponentInChildren<Gun>();

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

        /**
         * Called from the Sync Property Agent
         */
        public void OnShootingChanged()
        {
            ITarget gun = GetGunFromHands();

            if (grabbables != null &&
                syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue() &&
                gun != null)
            {
                gun.Shoot(aimPoint);

                networkAimPointUpdate?.Invoke(aimPoint);

                lastShootingState = false;
            }
        }

        private void ThrowObject(int selectedGrabbable)
        {
            if (grabbables[selectedGrabbable] != null)
            {
                grabbables[selectedGrabbable].StartThrow(throwForce, sceneCamera.transform.forward);
            }
        }

        private ITarget GetGunFromHands()
        {
            ITarget searched = grabbables.Find(obj => obj.GetComponent<ITarget>() != null).GetComponent<ITarget>();

            return searched;
        }

        private void Update()
        {
            // we handle the animation
            playerAnimations.ShootAnim(isFirePressed && HasGun());

            if (Input.GetKeyDown(weaponChanger))
            {
                ChangeHand();
            }
        }

        private bool HasGun()
        {
            Grabable weaponTarget = playerHand.GetComponentInChildren<Grabable>();
            return playerHand.childCount > 0 && weaponTarget != null && weaponTarget.getTargetType().Equals(TargetType.Shootable);
        }

        private IEnumerator MakeTargetAvailable()
        {
            yield return new WaitForSeconds(timeResetTarget);
            hasPointedToObject = false;
        }

        /**
         * Find the first available hand
         */
        private int FindAvailableHand()
        {
            return 0;
        }

        private void ChangeHand()
        {
            if (selectedGrabbable == 0)
            {
                // left hand
                selectedGrabbable = 1;
            }
            else
            {
                // right hand
                selectedGrabbable = 0;
            }

            Debug.Log("hand selected: " + selectedGrabbable);
        }

        public void AddGrabable(Grabable grabable)
        {
            // search if we already have it
            Grabable searched = grabbables.Find(obj => obj == grabable);
            if (searched != null)
            {
                return;
            }

            // if we have more than we can have, we pop the last one
            if (grabbables.Count >= maxGrabbables)
            {
                ThrowObject(selectedGrabbable);
            }

            grabbables.Add(grabable);
            ChangeHand();
        }

        public void RemoveGrabable(Grabable grabable)
        {
            Grabable searched = grabbables.Find(obj => obj == grabable);
            if (searched != null)
            {
                Debug.Log("thrown remove: " + selectedGrabbable);
                grabbables.Remove(searched);
                ChangeHand();
            }
        }

    }
}