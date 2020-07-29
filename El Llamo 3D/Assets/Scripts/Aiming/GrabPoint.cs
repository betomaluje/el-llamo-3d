using UnityEngine;
using UnityEngine.Animations.Rigging;

[System.Serializable]
public class GrabPoint
{
    public Transform grabPosition;

    // Hidden public objects
    public TwoBoneIKConstraint twoBoneIKConstraintData;

    [HideInInspector]
    public LocalGrabable grabedObject;

    public void UpdateWeight(float weight)
    {
        twoBoneIKConstraintData.weight = weight;
    }
}