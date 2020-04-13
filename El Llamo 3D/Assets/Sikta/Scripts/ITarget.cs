using UnityEngine;

namespace BetoMaluje.Sikta
{
    [SerializeField]
    public interface ITarget
    {
        void StartPickup(Transform playerHand, PlayerGrab playerGrab, Vector3 from);

        void Pickup(Vector3 from, Vector3 to);

        void Throw(float throwForce, Vector3 direction);

        void Shoot(Vector3 shootHit);

        TargetType getType();

        int GetDamage();

        bool isGrabbed();
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

