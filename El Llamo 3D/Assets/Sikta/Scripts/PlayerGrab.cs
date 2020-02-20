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
        public ITarget target;
        [SerializeField] private LayerMask weaponLayer;
        [SerializeField] private float grabDistance = 10f;

        private MaterialColorChanger lastObject;
        private bool hasPointedToObject = false;

        private NetworkID networkID;

        private bool isFirePressed = false;
        private bool isThrowGunPressed = false;

        private Transform cameraTransform;

        public void SetupPlayer() 
        {
            networkID = GetComponentInParent<NetworkID>();
            cameraTransform = transform.parent.transform;
        }

        private void Update()
        {
            if (networkID == null) return;

            if (networkID.IsMine)
            {
                isFirePressed = Input.GetMouseButtonDown(0);
                isThrowGunPressed = Input.GetMouseButtonDown(1);

                RaycastHit hit;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, grabDistance, weaponLayer))
                {
                    lastObject = hit.transform.GetComponent<MaterialColorChanger>();
                    if (lastObject != null && lastObject.isEnabled && !hasPointedToObject)
                    {
                        hasPointedToObject = true;
                        lastObject.TargetOn();
                    }

                    if (isFirePressed && target == null)
                    {
                        hit.transform.GetComponent<ITarget>().Pickup(this, transform);
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

                if (HasGun() && isFirePressed)
                {
                    transform.GetComponentInChildren<ITarget>().Shoot();
                }
                
                if (isThrowGunPressed && target != null )
                {
                    target.Throw(throwForce);
                    target = null;
                }           
            }
        }

        private bool HasGun()
        {
            return transform.childCount > 0 && transform.GetComponentInChildren<ITarget>().getType().Equals(TargetType.Shootable);
        }

        private IEnumerator MakeTargetAvailable()
        {
            yield return new WaitForSeconds(timeResetTarget);
            hasPointedToObject = false;
        }

    }
}