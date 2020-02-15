using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Weapons/Bullet", order = 0)]
public class BulletSO : ScriptableObject {
    public GameObject explosionParticles;
    public int maxDamage = 10;
}