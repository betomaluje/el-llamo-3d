using Llamo.Health;
using UnityEngine;

namespace Llamo.Turret
{
    public class TurretBullet : MonoBehaviour
    {
        [SerializeField] private BulletSO bullet;

        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            rb.velocity = transform.forward * bullet.shootingSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (LayerMaskUtils.LayerMatchesObject(bullet.attackLayer, collision.gameObject))
            {
                DealDamage(collision);
            }

            Instantiate(bullet.explosionParticles, collision.transform.position, collision.transform.rotation);
            Destroy(gameObject);
        }

        private void DealDamage(Collision collision)
        {
            IHealth healthTarget = collision.transform.root.GetComponentInChildren<IHealth>(true);

            if (healthTarget != null)
            {
                healthTarget.PerformDamage(bullet.maxDamage, collision.transform.position);
            }

            // if the target has a rigidbody, we perform a impact force

            if (collision.rigidbody != null)
            {
                Debug.Log(collision.transform.gameObject.name + " -> impact force! " + bullet.impactForce);
                collision.rigidbody.AddForce(-collision.transform.forward * bullet.impactForce);
            }
        }
    }
}
