using UnityEngine;

public class BulletTarget : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckLayerMask(collision.gameObject))
        {
            Debug.Log(gameObject.name + " is dying!");

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            Health health = GetComponent<Health>();

            if (bullet != null && health != null)
            {
                PerformDamage(bullet, health);
            }

        }
    }

    private void PerformDamage(Bullet bullet, Health health)
    {
        int damage = Random.Range(1, bullet.maxDamage);

        Debug.Log(gameObject.name + " took " + damage + " damage!");

        health.ModifyHealth(-damage);
    }

    private bool CheckLayerMask(GameObject target)
    {
        return (targetLayerMask & 1 << target.layer) == 1 << target.layer;
    }
}
