using UnityEngine;

public class NormalBullet : Bullet
{
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(GetExplosionParticles(), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
