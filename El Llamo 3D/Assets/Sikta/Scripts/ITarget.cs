﻿using UnityEngine;

namespace BetoMaluje.Sikta
{
    [SerializeField]
    public interface ITarget
    {
        void Pickup(PlayerGrab playerGrab, Transform weaponHolder);

        void Throw(float throwForce);

        void Shoot(Vector3 shootHit);

        TargetType getType();
    }

    [SerializeField]
    public enum TargetType {
        Throwable, Shootable
    }
}

