using UnityEngine;

namespace BetoMaluje.Sikta
{
    public interface ITarget
    {
        void Pickup(PlayerGrab playerGrab, Transform weaponHolder);

        void Throw(float throwForce);

        void Shoot();

        TargetType getType();
    }

    [SerializeField]
    public enum TargetType {
        Throwable, Shootable
    }
}

