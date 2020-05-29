using BetoMaluje.Sikta;
using SWNetwork;
using UnityEngine;

public class PlayerGrab : LocalPlayerGrab
{
    #region Network
    private bool isFirePressed = false;

    private Camera sceneCamera;

    // network property syncing
    private SyncPropertyAgent syncPropertyAgent;
    private RemoteEventAgent remoteEventAgent;
    private const string SHOOTING = "Shooting";
    bool lastShootingState = false;

    #endregion

    protected override void Start()
    {
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();
        remoteEventAgent = GetComponent<RemoteEventAgent>();
        base.Start();
    }

    /**
    * Handles the shooting state only for the owner of the network 
    */
    protected override void HandleShooting(ShootingTarget shootingTarget)
    {
        ITarget gun = GetGunInActiveHand();
        // if it doesn't have a gun, we return quickly
        if (gun == null)
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

            Gun gunTarget = GetActiveHand().GetComponentInChildren<Gun>();

            if (gunTarget != null)
            {
                int damage = gunTarget.GetDamage();

                Health healthTarget = shootHit.transform.gameObject.GetComponent<Health>();

                if (healthTarget != null)
                {
                    healthTarget.PerformDamage(damage);
                }

                CorpseHealth corpseHealth = shootHit.transform.gameObject.GetComponentInParent<CorpseHealth>();

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
         * Called from the Sync Property Agent
         */
    public void OnShootingChanged()
    {
        ITarget gun = GetGunInActiveHand();

        bool remoteShootingPressed = syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue();
        bool playerHasGun = gun != null;

        if (remoteShootingPressed && playerHasGun)
        {
            gun.Shoot(aimPoint);

            networkAimPointUpdate?.Invoke(aimPoint);

            lastShootingState = false;
        }
    }
}