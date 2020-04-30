using SWNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BetoMaluje.Sikta
{
    public class PlayerGrab : MonoBehaviour
    {
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private PlayerAnimationTrigger playerAnimationsTrigger;

        [Space]
        [Header("Stats")]
        [SerializeField] private float throwForce = 20;
        [SerializeField] private float timeResetTarget = 1f;

        [Space]
        [Header("Weapon")]
        public List<Grabable> grabbables;
        [SerializeField] private Transform[] playerHands;
        [SerializeField] private Image[] playerHandsUI;
        [SerializeField] private Color playerHandUIColor;

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
        private int selectedGrabbable = 0;
        private int maxGrabbables;

        private Transform playerHand;

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
            maxGrabbables = playerHands.Length;
            playerHand = playerHands[selectedGrabbable];
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
                if (grabbables.Count > 0)
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
                    if (lastObject != null)
                    {
                        lastObject.TargetOff();
                    }
                    lastObject = null;

                    Grabable grabable = targetHit.transform.GetComponentInChildren<Grabable>();
                    if (grabable != null && !grabable.isGrabbed())
                    {                    
                        PickupObject(grabable);                        
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
            Transform hand = GetActiveHand();
            Debug.Log("trying to pickup: " + hand);
            if (hand != null && hand.childCount == 0) 
            {                                
                grabable.StartPickup(hand.position);
            } 
            else 
            {
                // maybe throw one?
                Debug.Log("can't pickup! hands full!: " + selectedGrabbable);
                //ThrowObject(selectedGrabbable);              
            }
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

            if (!GameSettings.instance.usingNetwork)
            {                
                ITarget gun = playerHand.GetComponentInChildren<ITarget>();

                if (gun != null)
                {
                    gun.Shoot(aimPoint);

                    lastShootingState = false;
                }
            }

            // we mark the shooting point
            RaycastHit shootHit = shootingTarget.shootingHit;

            // if it was on target we take damage
            if (shootingTarget.onTarget)
            {
                aimPoint = shootHit.point;

                Health healthTarget = shootHit.transform.gameObject.GetComponent<Health>();
                Gun gunTarget = GetObjectFromHands<Gun>();

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
            ITarget gun = playerHand.GetComponentInChildren<ITarget>();

            if (syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue() &&
                gun != null)
            {
                gun.Shoot(aimPoint);

                networkAimPointUpdate?.Invoke(aimPoint);

                lastShootingState = false;
            }
        }

        private void ThrowObject(int selectedGrabbable)
        {
            Debug.Log("trying to throw: " + selectedGrabbable);
            Grabable gun = playerHand.GetComponentInChildren<Grabable>();
            if (gun != null)
            {
                gun.StartThrow(throwForce, sceneCamera.transform.forward);
            }
        }
        private T GetObjectFromHands<T>() 
        {
            foreach (Transform playerHand in playerHands)
            {
                T searched = playerHand.GetComponent<T>();            
                if (searched != null) 
                {                    
                    return searched;
                }

                T searched2 = playerHand.GetComponentInChildren<T>();
                if (searched2 != null) 
                {
                    return searched2;
                }

                T searched3 = playerHand.GetComponentInParent<T>();
                if (searched3 != null) 
                {
                    return searched3;
                }
            }

            return default(T);
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

        private void LateUpdate()
        {
            ChangeHandsUI(selectedGrabbable);
        }

        private bool HasGun()
        {
            Grabable weaponTarget = GetObjectFromHands<Grabable>();

            if (weaponTarget != null) 
            {                    
                return weaponTarget.getTargetType().Equals(TargetType.Shootable);
            }            

            return false;
        }

        private IEnumerator MakeTargetAvailable()
        {
            yield return new WaitForSeconds(timeResetTarget);
            hasPointedToObject = false;
        }

        /**
         * Find the first available hand
         */
        public Transform FindAvailableHand()
        {
            foreach (Transform playerHand in playerHands)
            {                
                if (playerHand.childCount == 0) 
                {                    
                    return playerHand;
                }
            }

            return null;
        }

        private void ChangeHandsUI(int selectedGrabbable)
        {
            int i = 0;
            foreach (Image hand in playerHandsUI)
            {
                if (i == selectedGrabbable)
                {
                    hand.color = playerHandUIColor;
                }
                else
                {
                    hand.color = Color.white;
                }
                i++;                
            }
        }

        public void ChangeHand()
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

            playerHand = playerHands[selectedGrabbable];

            Debug.Log("manual hand selected: " + selectedGrabbable);
        }

        public Transform GetActiveHand()
        {
            return playerHand;
        }

        public void AddGrabable(Grabable grabable)
        {
            // search if we already have it
            Grabable searched = grabbables.Find(obj => obj == grabable);
            if (searched != null)
            {
                return;
            }
       
            grabbables.Add(grabable);        
        }

        public void RemoveGrabable(Grabable grabable)
        {
            Grabable searched = grabbables.Find(obj => obj == grabable);
            if (searched != null)
            {
                Debug.Log("thrown remove: " + selectedGrabbable);
                grabbables.Remove(searched);
                // only if we are not in the first hand, we change it
                if (selectedGrabbable == 1) 
                {
                    ChangeHand();
                }
            }
        }

        private bool PlayerReachedMaxItems() {
            return grabbables.Count >= maxGrabbables;
        }

    }
}