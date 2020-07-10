using UnityEngine;

namespace Llamo.Turret
{
    public class TurretIntervalShoot : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private GameObject bullet;
        [SerializeField] private Transform shootingPosition;
        [SerializeField] private float fireRate = 3f;

        private float nextTimeToFire = 0f;

        private bool isPlayerNear = false;

        private TurretLookAtPlayer turretLookAtPlayer;

        private void OnEnable()
        {
            turretLookAtPlayer = GetComponentInChildren<TurretLookAtPlayer>(true);
            turretLookAtPlayer.playerNear += EnableFiring;
        }

        private void OnDisable()
        {
            turretLookAtPlayer.playerNear -= EnableFiring;
        }

        private void EnableFiring(bool playerNear)
        {
            isPlayerNear = playerNear;
        }

        private void Update()
        {
            if (isPlayerNear && Time.time >= nextTimeToFire)
            {
                // we can shoot here
                DoShooting();

                // we update the frequency of the shooting
                nextTimeToFire = Time.time + 1f / fireRate;
            }
        }

        private void DoShooting()
        {
            Instantiate(bullet, shootingPosition.position, shootingPosition.rotation);
        }
    }
}
