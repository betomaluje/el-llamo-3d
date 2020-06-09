using DG.Tweening;
using System;
using System.Collections;
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
    [SerializeField] private Image[] playerHandsUI;
    [SerializeField] private Color playerHandUIColor;

    [HideInInspector]
    public Vector3 aimPoint;
    [HideInInspector]
    public Action<Vector3> aimPointUpdate;

    private MaterialColorChanger lastObject;
    private bool hasPointedToObject = false;

    protected GrabController grabController;

    private PlayerAnimations playerAnimations;

    private Camera sceneCamera;

    protected virtual void Awake()
    {
        playerAnimations = GetComponent<PlayerAnimations>();

        grabController = GetComponent<GrabController>();
    }

    /**
     * This method is called only for the owner of the network
     */
    public void SetupPlayer()
    {
        sceneCamera = Camera.main;

        inputHandler.fireReleaseCallback = () =>
        {

        };

        // handles when the player is throwing
        inputHandler.secondaryClickCallback = () =>
        {
            // we need to get the currents hand object to throw it
            LocalGrabable currentGrabable = grabController.GetCurrentGrabable();
            if (currentGrabable != null)
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
            ThrowObject();
        };

        // handle target
        inputHandler.targetAquired += HandleTargetAquired;

        // handle shooting
        inputHandler.shootingTarget = (shootingTarget) =>
        {
            HandleShooting(shootingTarget);
        };
    }

    private void OnEnable()
    {
        grabController.grabPointUpdate += OnGrabPointChanged;
    }

    private void OnDisable()
    {
        inputHandler.targetAquired -= HandleTargetAquired;
        grabController.grabPointUpdate -= OnGrabPointChanged;
    }

    private void OnGrabPointChanged(GrabPoint grabPoint)
    {
        ChangeHandsUI(grabPoint.index);
    }

    private void HandleTargetAquired(PointingTarget pointingTarget)
    {
        if (pointingTarget.onTarget)
        {
            lastObject = pointingTarget.targetHit.transform.root.GetComponentInChildren<MaterialColorChanger>(true);

            if (lastObject != null && lastObject.isEnabled && !hasPointedToObject)
            {
                hasPointedToObject = true;
                lastObject.TargetOn();
            }

            if (pointingTarget.isPressed)
            {
                if (lastObject != null)
                {
                    lastObject.TargetOff();
                }
                lastObject = null;

                // if we already have something in the hand, we do nothing
                LocalGrabable grabable = pointingTarget.targetHit.transform.root.GetComponentInChildren<LocalGrabable>();
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
        Transform grabPoint = grabController.GetActiveHand();
        if (grabPoint != null && grabPoint.childCount == 0)
        {
            grabable.StartPickup(grabPoint.position);
        }
        else
        {
            // maybe throw one?
            Debug.Log("can't pickup! hands full!");
        }
    }

    /**
     * Handles the shooting state only for the owner of the network 
     */
    protected virtual void HandleShooting(ShootingTarget shootingTarget)
    {
        IGun gun = GetGunInActiveHand();
        // if it doesn't have a gun, we return quickly
        if (gun == null)
        {
            return;
        }

        // regardless if it is on target or not, we need to trigger the shooting action
        gun.Shoot(aimPoint);

        // we mark the shooting point
        RaycastHit shootHit = shootingTarget.targetHit;

        // if it was on target we take damage
        if (shootingTarget.onTarget)
        {
            aimPoint = shootHit.point;

            int damage = gun.GetDamage();

            // now we checked if we hit another player

            LocalHealth healthTarget = shootHit.transform.gameObject.GetComponent<LocalHealth>();

            if (healthTarget != null)
            {
                healthTarget.PerformDamage(damage);
            }

            // now we checked if we hit an enemy

            LocalCorpseHealth corpseHealth = shootHit.transform.gameObject.GetComponentInParent<LocalCorpseHealth>();

            if (corpseHealth != null)
            {
                Debug.Log("Impact on corpse");
                corpseHealth.PerformDamage(damage, shootHit.point);
            }

            // if the target has a rigidbody, we perform a impact force

            if (shootHit.rigidbody != null)
            {
                Debug.Log(shootHit.transform.gameObject.name + " -> impact force! " + gun.GetImpactForce());
                shootHit.rigidbody.AddForce(-shootHit.normal * gun.GetImpactForce());
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
    private void ThrowObject()
    {
        LocalGrabable gun = grabController.GetCurrentGrabable();
        Debug.Log("trying to throw: " + gun);
        if (gun != null)
        {
            gun.StartThrow(throwForce, sceneCamera.transform.forward);
        }
    }

    /**
     * Gets the current [ITarget] on the active player's hand
     */
    protected IGun GetGunInActiveHand()
    {
        return grabController.GetActiveHand().GetComponentInChildren<IGun>();
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
}
