using UnityEngine;

namespace BetoMaluje.Sikta
{
    [SerializeField]
    public interface ITarget
    {
        void Shoot(Vector3 shootHit);

        int GetDamage();
    }

    [SerializeField]
    public enum HoverType
    {
        None, Color
    }
}

