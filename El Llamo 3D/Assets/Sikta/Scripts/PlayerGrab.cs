using System.Collections;
using UnityEngine;
using SWNetwork;

namespace BetoMaluje.Sikta
{
    public class PlayerGrab : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float throwForce = 20;
        [SerializeField] private float timeResetTarget = 1f;

        [Space]
        [Header("Weapon")]
        public ITarget weapon;
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private LayerMask weaponLayer;
        [SerializeField] private float grabDistance = 10f;

        private MaterialColorChanger lastObject;
        private bool hasPointedToObject = false;

        private NetworkID networkID;

        private bool isFirePressed = false;
        private bool isThrowGunPressed = false;

        private void Start()
        {
            networkID = GetComponentInParent<NetworkID>();
        }

        void Update()
        {
            if (networkID.IsMine)
            {
                isFirePressed = Input.GetMouseButtonDown(0);

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance, weaponLayer))
                {
                    lastObject = hit.transform.GetComponent<MaterialColorChanger>();
                    if (lastObject != null && lastObject.isEnabled && !hasPointedToObject)
                    {
                        hasPointedToObject = true;
                        lastObject.TargetOn();
                    }

                    if (isFirePressed && weapon == null)
                    {
                        hit.transform.GetComponent<ITarget>().Pickup(this, weaponHolder);
                    }
                }
                else
                {
                    if (lastObject != null && hasPointedToObject)
                    {
                        StartCoroutine(MakeTargetAvailable());
                        lastObject.TargetOff();
                        lastObject = null;
                    }
                }

                if (HasGun())
                {
                    if (isFirePressed)
                    {
                        weaponHolder.GetComponentInChildren<ITarget>().Shoot();
                    }

                    isThrowGunPressed = Input.GetMouseButtonDown(1);

                    if (isThrowGunPressed)
                    {
                        weaponHolder.GetComponentInChildren<ITarget>().Throw(throwForce);
                        weapon = null;
                    }
                }

                if (!isFirePressed && weapon != null && !HasGun())
                {
                    weapon.Throw(throwForce);
                    weapon = null;
                }
            }            
        }

        private bool HasGun()
        {
            return weaponHolder.childCount > 0 && weaponHolder.GetComponentInChildren<ITarget>().getType().Equals(TargetType.Shootable);
        }

        private IEnumerator MakeTargetAvailable()
        {
            yield return new WaitForSeconds(timeResetTarget);
            hasPointedToObject = false;
        }

    }
}