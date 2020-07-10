using System;
using UnityEngine;

namespace Llamo.Turret
{
    public class TurretLookAtPlayer : MonoBehaviour
    {
        [SerializeField] private LayerMask attackLayer;
        [SerializeField] private float turnSpeed = 10f;

        private Transform playerPosition;

        public Action<bool> playerNear = delegate { };

        private void OnTriggerEnter(Collider other)
        {
            if (playerPosition == null && LayerMaskUtils.LayerMatchesObject(attackLayer, other.gameObject))
            {
                playerPosition = other.transform;
                playerNear(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (LayerMaskUtils.LayerMatchesObject(attackLayer, other.gameObject))
            {
                playerPosition = null;
                playerNear(false);
            }
        }

        private void Update()
        {
            if (playerPosition != null)
            {
                RotateTowards(playerPosition.position);
            }
        }

        private void RotateTowards(Vector3 to)
        {
            Vector3 pos = to - transform.position;
            Quaternion newRot = Quaternion.LookRotation(pos);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * turnSpeed);
        }
    }
}
