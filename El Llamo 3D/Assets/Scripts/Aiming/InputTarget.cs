using UnityEngine;

public abstract class InputTarget
{
    public RaycastHit targetHit;
    public bool onTarget;
    public bool isPressed;

    public InputTarget(RaycastHit targetHit, bool onTarget, bool isPressed)
    {
        this.targetHit = targetHit;
        this.onTarget = onTarget;
        this.isPressed = isPressed;
    }
}
