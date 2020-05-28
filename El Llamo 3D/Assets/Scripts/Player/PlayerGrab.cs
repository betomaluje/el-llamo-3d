using SWNetwork;
using UnityEngine;
using BetoMaluje.Sikta;

public class PlayerGrab: LocalPlayerGrab 
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
		syncPropertyAgent = GetComponent <SyncPropertyAgent> ();
		remoteEventAgent = GetComponent <RemoteEventAgent> ();
		base.Start();		
	}

	/**
         * Handles the shooting state only for the owner of the network 
         */
	protected override void HandleShooting(ShootingTarget shootingTarget)
	{
		base.HandleShooting(shootingTarget);
		// regardless if it is on target or not, we need to sync the shooting action
		if (isFirePressed != lastShootingState) {
			syncPropertyAgent.Modify(SHOOTING, isFirePressed);
			lastShootingState = isFirePressed;
		}
	}

	/**
         * Called from the Sync Property Agent
         */
	public void OnShootingChanged() {
		ITarget gun = GetGunInActiveHand();

		bool remoteShootingPressed = syncPropertyAgent.GetPropertyWithName(SHOOTING).GetBoolValue();
		bool playerHasGun = gun != null;

		if (remoteShootingPressed && playerHasGun) {
			gun.Shoot(aimPoint);

			networkAimPointUpdate ? .Invoke(aimPoint);

			lastShootingState = false;
		}
	}
}