using DG.Tweening;
using SWNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BetoMaluje.Sikta;
using UnityEngine.UI;

public class PlayerGrab: MonoBehaviour 
{ 
    [SerializeField] private LocalInputHandler inputHandler;
	[SerializeField] private PlayerAnimationTrigger playerAnimationsTrigger;

	[Space][Header("Stats")][SerializeField] private float throwForce = 20;
	[SerializeField] private float timeResetTarget = 1f;

	[Space][Header("Weapon")]
	public List < Grabable > grabbables;
	[SerializeField] private Transform[] playerHands;
	[SerializeField] private Image[] playerHandsUI;
	[SerializeField] private Color playerHandUIColor;

	[SerializeField] private KeyCode weaponChanger = KeyCode.E;

	[HideInInspector]
	public Vector3 aimPoint;
	[HideInInspector]
	public Action < Vector3 > aimPointUpdate;
	[HideInInspector]
	public Action < Vector3 > networkAimPointUpdate;

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

	// network property syncing
	private SyncPropertyAgent syncPropertyAgent;
	private RemoteEventAgent remoteEventAgent;
	private const string SHOOTING = "Shooting";
	bool lastShootingState = false;

	#endregion

	private void Start() {
		syncPropertyAgent = GetComponent < SyncPropertyAgent > ();
		remoteEventAgent = GetComponent < RemoteEventAgent > ();

		playerAnimations = GetComponent < PlayerAnimations > ();

		grabbables = new List < Grabable > ();
		maxGrabbables = playerHands.Length;
		playerHand = playerHands[selectedGrabbable];
		ChangeHandsUI(selectedGrabbable);
	}

	/**
         * This method is called only for the owner of the network
         */
	public void SetupPlayer() {
		sceneCamera = Camera.main;

		inputHandler.fireReleaseCallback = () => {
			isFirePressed = false;
		};

		// handles when the player is throwing
		inputHandler.secondaryClickCallback = () => {
			if (grabbables.Count > 0) {
				playerAnimations.Throw();
			}
		};

		inputHandler.secondaryReleaseCallback = () => {
        };

		// handle actual object throwing
		playerAnimationsTrigger.throwTriggeredCallback = () => {
			ThrowObject(selectedGrabbable);
		};

		// handle target
		inputHandler.targetAquired += HandleTargetAquired;

		// handle shooting
		inputHandler.shootingTarget = HandleShooting;
	}

	private void OnDisable() {
		inputHandler.targetAquired -= HandleTargetAquired;
	}

	private void HandleTargetAquired(RaycastHit targetHit, bool onTarget) {
		if (onTarget) {
			lastObject = targetHit.transform.GetComponentInChildren < MaterialColorChanger > ();

			if (lastObject == null) {
				// we try in the parents game object
				lastObject = Utils.GetComponentInParents < MaterialColorChanger > (targetHit.transform.gameObject);
			}

			if (lastObject != null && lastObject.isEnabled && !hasPointedToObject) {
				hasPointedToObject = true;
				lastObject.TargetOn();
			}

			if (isFirePressed) {
				if (lastObject != null) {
					lastObject.TargetOff();
				}
				lastObject = null;

				Grabable grabable = targetHit.transform.root.GetComponentInChildren < Grabable > ();
				if (grabable != null && !grabable.isGrabbed()) {
					PickupObject(grabable);
				}
			}
		}
		else {
			if (lastObject != null && hasPointedToObject) {
				StartCoroutine(MakeTargetAvailable());
				lastObject.TargetOff();
			}

			lastObject = null;
		}
	}

	private void PickupObject(Grabable grabable) {
		Transform hand = GetActiveHand();
		if (hand != null && hand.childCount == 0) {
			grabable.StartPickup(hand.position);
		}
		else {
			// maybe throw one?
			Debug.Log("can't pickup! hands full!: " + selectedGrabbable);
			//ThrowObject(selectedGrabbable);              
		}
	}

	/**
         * Handles the shooting state only for the owner of the network 
         */
	private void HandleShooting(ShootingTarget shootingTarget) {
		isFirePressed = true;

		ITarget gun = GetGunInActiveHand();
		// if it doesn't have a gun, we return quickly
		if (gun == null) {
			return;
		}

		// regardless if it is on target or not, we need to sync the shooting action
		if (isFirePressed != lastShootingState) {
			syncPropertyAgent.Modify(SHOOTING, isFirePressed);
			lastShootingState = isFirePressed;
		}

		if (!GameSettings.instance.usingNetwork) {
			gun.Shoot(aimPoint);

			lastShootingState = false;
		}

		// we mark the shooting point
		RaycastHit shootHit = shootingTarget.shootingHit;

		// if it was on target we take damage
		if (shootingTarget.onTarget) {
			aimPoint = shootHit.point;

			Gun gunTarget = GetActiveHand().GetComponentInChildren < Gun > ();

			if (gunTarget != null) {
				int damage = gunTarget.GetDamage();

				Health healthTarget = shootHit.transform.gameObject.GetComponent < Health > ();

				if (healthTarget != null) {
					healthTarget.PerformDamage(damage);
				}

				CorpseHealth corpseHealth = shootHit.transform.gameObject.GetComponentInParent < CorpseHealth > ();

				if (corpseHealth != null) {
					Debug.Log("Impact on corpse");
					corpseHealth.PerformDamage(damage, shootHit.point);
				}

				if (shootHit.rigidbody != null) {
					Debug.Log(shootHit.transform.gameObject.name + " -> impact force! " + gunTarget.impactForce);
					shootHit.rigidbody.AddForce( - shootHit.normal * gunTarget.impactForce);
				}
			}
		}
		else {
			aimPoint = shootingTarget.ray.origin + shootingTarget.ray.direction * shootingTarget.shootingDistance;
		}

		aimPointUpdate ? .Invoke(aimPoint);
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

	/**
         * Throws the current [Grabable]
         */
	private void ThrowObject(int selectedGrabbable) {
		Debug.Log("trying to throw: " + selectedGrabbable);
		Grabable gun = GetActiveHand().GetComponentInChildren < Grabable > ();
		if (gun != null) {
			gun.StartThrow(throwForce, sceneCamera.transform.forward);
		}
	}

	private void Update() {
		// we handle the animation
		playerAnimations.ShootAnim(isFirePressed && HasGun());

		if (Input.GetKeyDown(weaponChanger)) {
			ChangeHand();
		}
	}

	/**
         * Gets the current [ITarget] on the active player's hand
         */
	private ITarget GetGunInActiveHand() {
		return GetActiveHand().GetComponentInChildren < ITarget > ();
	}

	/**
         * Checks if there's any [Grabbable] that is of type [TargetType.Shootable]
         */
	private bool HasGun() {
		foreach(Transform playerHand in playerHands) {
			Grabable searched = playerHand.GetComponent < Grabable > ();
			if (searched != null && searched.getTargetType().Equals(TargetType.Shootable)) {
				return true;
			}

			Grabable searched2 = playerHand.GetComponentInChildren < Grabable > ();
			if (searched2 != null && searched2.getTargetType().Equals(TargetType.Shootable)) {
				return true;
			}

			Grabable searched3 = playerHand.GetComponentInParent < Grabable > ();
			if (searched3 != null && searched3.getTargetType().Equals(TargetType.Shootable)) {
				return true;
			}
		}

		return false;
	}

	private IEnumerator MakeTargetAvailable() {
		yield
		return new WaitForSeconds(timeResetTarget);
		hasPointedToObject = false;
	}

	private void ChangeHandsUI(int selectedGrabbable) {
		int i = 0;
		foreach(Image hand in playerHandsUI) {
			if (i == selectedGrabbable) {
				hand.color = playerHandUIColor;
				hand.gameObject.transform.DOScale(1.3f, 0.25f);
			}
			else {
				hand.color = Color.white;
				hand.gameObject.transform.localScale = new Vector3(1, 1, 1);
			}
			i++;
		}
	}

	public void ChangeHand() {
		if (selectedGrabbable == 0) {
			// left hand
			selectedGrabbable = 1;
		}
		else {
			// right hand
			selectedGrabbable = 0;
		}

		playerHand = playerHands[selectedGrabbable];
		ChangeHandsUI(selectedGrabbable);

		Debug.Log("manual hand selected: " + selectedGrabbable);
	}

	public Transform GetActiveHand() {
		return playerHand;
	}

	public void AddGrabable(Grabable grabable) {
		// search if we already have it
		Grabable searched = grabbables.Find(obj => obj == grabable);
		if (searched != null) {
			return;
		}

		grabbables.Add(grabable);
	}

	public void RemoveGrabable(Grabable grabable) {
		Grabable searched = grabbables.Find(obj => obj == grabable);
		if (searched != null) {
			Debug.Log("thrown remove: " + selectedGrabbable);
			grabbables.Remove(searched);
			// only if we are not in the first hand, we change it
			if (selectedGrabbable == 1) {
				ChangeHand();
			}
		}
	}

}