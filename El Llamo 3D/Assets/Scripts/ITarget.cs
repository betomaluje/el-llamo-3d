using UnityEngine;

public interface ITarget
{
    void Pickup(Transform weaponHolder);

    void Throw(float throwForce);

    void Shoot();

    TargetType getType();
}

[SerializeField]
public enum TargetType {
    Throwable, Shootable
}
