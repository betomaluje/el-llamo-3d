using UnityEngine;

[SerializeField]
public interface IHealth
{
    void PerformDamage(int damage, Vector3 impactPosition);

    void GiveHealth(int health, Vector3 impactPosition);

    void AddDamageSFX(Vector3 impactPosition);

    void AddHealSFX(Vector3 impactPosition);
}
