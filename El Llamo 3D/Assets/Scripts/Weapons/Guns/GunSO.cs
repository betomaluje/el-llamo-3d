using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapons/Gun", order = 0)]
public class GunSO : ScriptableObject
{
    // Stats
    public float fireRate = 3f;
    public int minDamage = 10;
    public int maxDamage = 30;
    public float impactForce = 100f;

    public int ammo = 10;

    // Recoil
    public float recoilAmount = 0.3f;
    public float recoilDuration = 0.3f;
}
