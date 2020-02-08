using System.Collections;
using UnityEngine;

namespace BetoMaluje.Sikta
{
    public class PlayerGrab : MonoBehaviour
    {
        public static PlayerGrab instance;

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

        private void Awake()
        {
            instance = this;
        }

        void Update()
        {
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
                    hit.transform.GetComponent<ITarget>().Pickup(weaponHolder);
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

            if (Input.GetMouseButtonDown(0) && HasGun())
            {
                weaponHolder.GetComponentInChildren<ITarget>().Shoot();
            }

            if (Input.GetMouseButtonDown(1) && HasGun())
            {
                weaponHolder.GetComponentInChildren<ITarget>().Throw(throwForce);
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