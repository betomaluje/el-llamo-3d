using UnityEngine;

public class PosessTarget : InputTarget
{
    public PosessTarget(RaycastHit targetHit, bool onTarget, bool isPressed) : base(targetHit, onTarget, isPressed)
    {
    }
}
