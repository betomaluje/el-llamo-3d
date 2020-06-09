using SWNetwork;
using System;
using UnityEngine;

public class PlayerGrab : LocalPlayerGrab
{
    [HideInInspector]
    public Action<Vector3> networkAimPointUpdate;

    #region Network

    // network property syncing
    private SyncPropertyAgent syncPropertyAgent;
    private const string SHOOTING = "Shooting";
    private bool lastShootingState = false;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        syncPropertyAgent = GetComponent<SyncPropertyAgent>();
    }

    /**
    * Handles the shooting state only for the owner of the network 
    */
    protected override void HandleShooting(ShootingTarget shootingTarget)
    {
        IGun gun = GetGunInActiveHand();
        // if it doesn't have a gun, we return quickly
        if (gun == null)
        {
            return;
        }

        // regardless if it is on target or not, we need to sync the shooting action
        if (shootingTarget.isPressed != lastShootingState)
        {
            syncPropertyAgent.Modify(SHOOTING, shootingTarget.isPressed);
            lastShootingState = shootingTarget.isPressed;
        }

        // we mark the shooting point
        RaycastHit shootHit = shootingTarget.targetHit;

        // if it was on target we take damage
        if (shootingTarget.onTarget)
        {
            aimPoint = shootHit.point;

            Gun gunTarget = grabController.GetActiveHand().GetComponentInChildren<Gun>();

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
                    Debug.Log("Impact on corpse damage: " + damage);
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
        IGun gun = GetGunInActiveHand();

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