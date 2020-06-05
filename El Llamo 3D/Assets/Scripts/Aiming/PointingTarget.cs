using UnityEngine;

public class PointingTarget : InputTarget
{
    public PointingTarget(RaycastHit targetHit, bool onTarget, bool isPressed) : base(targetHit, onTarget, isPressed)
    {
    }
}
