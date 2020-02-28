using UnityEngine;

public class ShootingTarget
{
    public Ray ray;
    public RaycastHit shootingHit;
    public bool onTarget;
    public float shootingDistance;

    public ShootingTarget(Ray ray, RaycastHit shootingHit, bool onTarget, float shootingDistance)
    {
        this.ray = ray;
        this.shootingHit = shootingHit;
        this.onTarget = onTarget;
        this.shootingDistance = shootingDistance;
    }
}
