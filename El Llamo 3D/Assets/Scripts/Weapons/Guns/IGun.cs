using UnityEngine;

[SerializeField]
public interface IGun
{
    /**
     * return True if successful shooting (aka had ammo), False otherwise
     */
    bool Shoot(Vector3 shootHit);

    int GetDamage();

    float GetImpactForce();
}

