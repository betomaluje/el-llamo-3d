using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletSO bullet;

    public int GetDamage() {
        return Random.Range(1, bullet.maxDamage);
    }

    public GameObject GetExplosionParticles() {
        return bullet.explosionParticles;
    }

    public float GetShootingSpeed()
    {
        return bullet.shootingSpeed;
    }
}
