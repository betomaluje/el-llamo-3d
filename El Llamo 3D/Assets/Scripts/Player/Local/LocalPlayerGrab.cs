﻿using BetoMaluje.Sikta;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerGrab : MonoBehaviour
{
    [SerializeField] private LocalInputHandler inputHandler;
    [SerializeField] private PlayerAnimationTrigger playerAnimationsTrigger;

    [Space]
    [Header("Stats")]
    [SerializeField] private float throwForce = 20;
    [SerializeField] private float timeResetTarget = 1f;

    [Space]
    [Header("Weapon")]
    public List<LocalGrabable> grabbables;
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

    private bool isFirePressed = false;

    private Camera sceneCamera;

    protected virtual void Start()
    {
        playerAnimations = GetComponent<PlayerAnimations>();

        grabbables = new List<LocalGrabable>();
        maxGrabbables = playerHands.Length;
        playerHand = playerHands[selectedGrabbable];
        ChangeHandsUI(selectedGrabbable);
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
    }

    private void OnDisable()
    {
        inputHandler.targetAquired -= HandleTargetAquired;
    }

    private void HandleTargetAquired(RaycastHit targetHit, bool onTarget)
    {
        if (onTarget)
        {
            lastObject = targetHit.transform.root.GetComponentInChildren<MaterialColorChanger>(true);

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

                LocalGrabable grabable = targetHit.transform.root.GetComponentInChildren<LocalGrabable>();
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

    private void PickupObject(LocalGrabable grabable)
    {
        Transform hand = GetActiveHand();
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
    protected virtual void HandleShooting(ShootingTarget shootingTarget)
    {
        isFirePressed = true;

        ITarget gun = GetGunInActiveHand();
        // if it doesn't have a gun, we return quickly
        if (gun == null)
        {
            return;
        }

        // regardless if it is on target or not, we need to trigger the shooting action
        gun.Shoot(aimPoint);

        // we mark the shooting point
        RaycastHit shootHit = shootingTarget.shootingHit;

        // if it was on target we take damage
        if (shootingTarget.onTarget)
        {
            aimPoint = shootHit.point;

            LocalGun gunTarget = GetActiveHand().GetComponentInChildren<LocalGun>();

            if (gunTarget != null)
            {
                int damage = gunTarget.GetDamage();

                LocalHealth healthTarget = shootHit.transform.gameObject.GetComponent<LocalHealth>();

                if (healthTarget != null)
                {
                    healthTarget.PerformDamage(damage);
                }

                LocalCorpseHealth corpseHealth = shootHit.transform.gameObject.GetComponentInParent<LocalCorpseHealth>();

                if (corpseHealth != null)
                {
                    Debug.Log("Impact on corpse");
                    corpseHealth.PerformDamage(damage, shootHit.point);
                }

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
     * Throws the current [LocalGrabable]
     */
    private void ThrowObject(int selectedGrabbable)
    {
        Debug.Log("trying to throw: " + selectedGrabbable);
        LocalGrabable gun = GetActiveHand().GetComponentInChildren<LocalGrabable>();
        if (gun != null)
        {
            gun.StartThrow(throwForce, sceneCamera.transform.forward);
        }
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

    /**
     * Gets the current [ITarget] on the active player's hand
     */
    protected ITarget GetGunInActiveHand()
    {
        return GetActiveHand().GetComponentInChildren<ITarget>();
    }

    /**
     * Checks if there's any [Grabbable] that is of type [TargetType.Shootable]
     */
    private bool HasGun()
    {
        foreach (Transform playerHand in playerHands)
        {
            LocalGrabable searched = playerHand.GetComponent<LocalGrabable>();
            if (searched != null && searched.getTargetType().Equals(TargetType.Shootable))
            {
                return true;
            }

            LocalGrabable searched2 = playerHand.GetComponentInChildren<LocalGrabable>();
            if (searched2 != null && searched2.getTargetType().Equals(TargetType.Shootable))
            {
                return true;
            }

            LocalGrabable searched3 = playerHand.GetComponentInParent<LocalGrabable>();
            if (searched3 != null && searched3.getTargetType().Equals(TargetType.Shootable))
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator MakeTargetAvailable()
    {
        yield return new WaitForSeconds(timeResetTarget);
        hasPointedToObject = false;
    }

    private void ChangeHandsUI(int selectedGrabbable)
    {
        int i = 0;
        foreach (Image hand in playerHandsUI)
        {
            if (i == selectedGrabbable)
            {
                hand.color = playerHandUIColor;
                hand.gameObject.transform.DOScale(1.3f, 0.25f);
            }
            else
            {
                hand.color = Color.white;
                hand.gameObject.transform.localScale = new Vector3(1, 1, 1);
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
        ChangeHandsUI(selectedGrabbable);

        Debug.Log("manual hand selected: " + selectedGrabbable);
    }

    public Transform GetActiveHand()
    {
        return playerHand;
    }

    public void AddGrabable(LocalGrabable grabable)
    {
        // search if we already have it
        LocalGrabable searched = grabbables.Find(obj => obj == grabable);
        if (searched != null)
        {
            return;
        }

        grabbables.Add(grabable);
    }

    public void RemoveGrabable(LocalGrabable grabable)
    {
        LocalGrabable searched = grabbables.Find(obj => obj == grabable);
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
}
