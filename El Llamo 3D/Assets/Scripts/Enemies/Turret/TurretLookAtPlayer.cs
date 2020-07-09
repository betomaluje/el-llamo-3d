using UnityEngine;

namespace Llamo.Turret
{
    public class TurretLookAtPlayer : MonoBehaviour
    {
        [SerializeField] private LayerMask attackLayer;
        [SerializeField] private float turnSpeed = 10f;

        private Transform playerPosition;

        private bool isPlayerClose = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!isPlayerClose && LayerMaskUtils.LayerMatchesObject(attackLayer, other.gameObject))
            {
                isPlayerClose = true;
                playerPosition = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            playerPosition = null;
            isPlayerClose = false;
        }

        private void Update()
        {
            if (!isPlayerClose || playerPosition == null)
            {
                return;
            }

            RotateTowards(playerPosition.position);
        }

        private void RotateTowards(Vector3 to)
        {
            Vector3 difference = (to - transform.position).normalized;

            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            Debug.Log("Z angle " + rotationZ);

            if (rotationZ < 0) rotationZ -= 360;

            var rotationAngle = Quaternion.Euler(-90.0f, 0.0f, rotationZ);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotationAngle, Time.deltaTime * turnSpeed);
        }
    }
}
