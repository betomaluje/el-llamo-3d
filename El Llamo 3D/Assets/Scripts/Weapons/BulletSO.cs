using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Weapons/Bullet", order = 0)]
public class BulletSO : ScriptableObject
{
    public GameObject explosionParticles;
    public LayerMask attackLayer;
    public int maxDamage = 10;
    public float shootingSpeed = 1000f;
    public float impactForce = 1000f;
}