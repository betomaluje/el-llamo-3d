using UnityEngine;

[System.Serializable]
public class GrabPoint
{
    public Transform grabPosition;

    // Hidden public objects
    [HideInInspector]
    public int index;
    [HideInInspector]
    public LocalGrabable grabedObject;
}