using UnityEngine;

public class ShootingTarget : InputTarget
{
    public Ray ray;
    public float shootingDistance;

    public ShootingTarget(
        Ray ray,
        RaycastHit shootingHit,
        bool onTarget,
        float shootingDistance,
        bool isPressed) : base(shootingHit, onTarget, isPressed)
    {
        this.ray = ray;
        this.shootingDistance = shootingDistance;
    }
}
