using UnityEngine;

public class ShootingTarget : InputTarget
{
    public float shootingDistance;

    public ShootingTarget(
        RaycastHit shootingHit,
        bool onTarget,
        float shootingDistance,
        bool isPressed) : base(shootingHit, onTarget, isPressed)
    {
        this.shootingDistance = shootingDistance;
    }
}
