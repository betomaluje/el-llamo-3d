using UnityEngine;

[SerializeField]
public interface IGun
{
    void Shoot(Vector3 shootHit);

    int GetDamage();

    float GetImpactForce();
}

