using System;
using UnityEngine;
using Llamo.Health;

public class LocalPlayerGrab : MonoBehaviour
{
    [SerializeField] private LocalInputHandler inputHandler;
    [SerializeField] private PlayerAnimationTrigger playerAnimationsTrigger;

    [Space]
    [Header("Stats")]
    [SerializeField] private float throwForce = 20;
    [SerializeField] private float timeResetTarget = 1f;

    [HideInInspector]
    public Vector3 aimPoint;
    [HideInInspector]
    public Action<Vector3> aimPointUpdate;

    protected GrabController grabController;

    private Transform crosshair;
    private PlayerAnimations playerAnimations;

    private Camera sceneCamera;

    protected virtual void Start()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
        grabController = GetComponent<GrabController>();
        crosshair = FindObjectOfType<Crosshair>().transform;
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
        inputHandler.secondaryClickCallback += HandleSecondaryClick;

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
        inputHandler.shootingTarget += HandleShooting;
    }

    private void HandleSecondaryClick()
    {
        // we need to get the currents hand object to throw it
        LocalGrabable currentGrabable = grabController.GetCurrentGrabable();
        if (currentGrabable != null)
        {
            if (playerAnimations != null)
            {
                playerAnimations.Throw();
            }
            else
            {
                ThrowObject();
            }
        }
    }

    private void OnDisable()
    {
        inputHandler.targetAquired -= HandleTargetAquired;
        inputHandler.shootingTarget -= HandleShooting;
        inputHandler.secondaryClickCallback -= HandleSecondaryClick;
    }

    private void HandleTargetAquired(PointingTarget pointingTarget)
    {
        IGun gun = GetGunInActiveHand();

        // if it does have a gun, we return quickly
        if (gun != null)
        {
            return;
        }

        if (pointingTarget.onTarget && pointingTarget.isPressed)
        {
            // if we already have something in the hand, we do nothing
            LocalGrabable grabable = pointingTarget.targetHit.transform.root.GetComponentInChildren<LocalGrabable>();
            if (grabable != null && !grabable.isGrabbed())
            {
                PickupObject(grabable);
                return;
            }
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

        // we mark the shooting point
        RaycastHit shootHit = shootingTarget.targetHit;

        // regardless if it is on target or not, we need to trigger the shooting action
        gun.Shoot(shootHit.point);

        // if it was on target we take damage
        if (shootingTarget.onTarget)
        {
            Vector3 direction = crosshair.position - shootHit.point;

            aimPoint = direction;

            int damage = gun.GetDamage();

            // now we checked if we hit another player

            IHealth healthTarget = shootHit.transform.root.GetComponentInChildren<IHealth>(true);

            if (healthTarget != null)
            {
                healthTarget.PerformDamage(damage, shootHit.point);
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
     * Gets the current [IGun] on the active player's hand
     */
    protected IGun GetGunInActiveHand()
    {
        return grabController.GetActiveHand().GetComponentInChildren<IGun>();
    }
}
