using UnityEngine;

[System.Serializable]
public class GrabPoint
{
    public Transform grabPosition;

    // Hidden public objects
    [HideInInspector]
    public LocalGrabable grabedObject;
}