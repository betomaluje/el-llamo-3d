using UnityEngine;

namespace BetoMaluje.Sikta
{
    [SerializeField]
    public interface ITarget
    {
        void Pickup(PlayerGrab playerGrab, Transform weaponHolder);

        void Throw(float throwForce, Vector3 direction);

        void Shoot(Vector3 shootHit);

        TargetType getType();

        int GetDamage();
    }

    [SerializeField]
    public enum TargetType
    {
        Throwable, Shootable
    }

    [SerializeField]
    public enum HoverType
    {
        None, Color
    }
}

