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

        private void Start()
        {
            networkID = GetComponentInParent<NetworkID>();
        }

        void Update()
        {
            if (!networkID.IsMine)
            {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance, weaponLayer))
            {
                lastObject = hit.transform.GetComponent<MaterialColorChanger>();
                if (lastObject != null && lastObject.isEnabled && !hasPointedToObject)
                {
                    hasPointedToObject = true;
                    lastObject.TargetOn();
                }

                if (Input.GetMouseButtonDown(0) && weapon == null)
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

            if (HasGun()) {
                if (Input.GetMouseButtonDown(0))
                {
                    weaponHolder.GetComponentInChildren<ITarget>().Shoot();
                }

                if (Input.GetMouseButtonDown(1))
                {
                    weaponHolder.GetComponentInChildren<ITarget>().Throw(throwForce);
                    weapon = null;
                }
            }

            if (Input.GetMouseButtonUp(0) && weapon != null && !HasGun())
            {
                weapon.Throw(throwForce);
                weapon = null;
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