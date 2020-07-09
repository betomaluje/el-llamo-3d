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
            Vector3 pos = to - transform.position;
            Quaternion newRot = Quaternion.LookRotation(pos);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * turnSpeed);
        }
    }
}
