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

    protected override void Start()
    {
        base.Start();
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

        HandleShootingDetection(gun, shootingTarget);
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
            bool gunShotSuccessful = gun.Shoot(aimPoint);

            networkAimPointUpdate?.Invoke(aimPoint);

            lastShootingState = false;
        }
    }
}