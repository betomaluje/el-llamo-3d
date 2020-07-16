using Llamo.Health;
using UnityEngine;

public class RevolverBullet : MonoBehaviour
{
    [SerializeField] private BulletSO bullet;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * bullet.shootingSpeed, ForceMode.Impulse);
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
