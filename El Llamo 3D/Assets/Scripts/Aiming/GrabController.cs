using System;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    [Header("Grab Points")]
    [SerializeField] private GrabPoint[] grabPoints;
    [SerializeField] private KeyCode pointChanger = KeyCode.E;

    [HideInInspector]
    public Action<GrabPoint> grabPointUpdate = delegate { };

    // 0: right, 1 left
    private int selectedGrabPoint = 0;

    private GrabPoint currentGrabPoint;

    void Start()
    {
        int i = 0;
        foreach (var grabPoint in grabPoints)
        {
            grabPoint.index = i;
            i++;
        }

        UpdateCurrentGrabPoint();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pointChanger))
        {
            ChangeHand();
        }
    }

    private void ChangeHand()
    {
        if (selectedGrabPoint == 0)
        {
            // left hand
            selectedGrabPoint = 1;
        }
        else
        {
            // right hand
            selectedGrabPoint = 0;
        }

        UpdateCurrentGrabPoint();

        Debug.Log("manual hand selected: " + selectedGrabPoint);
    }

    private void UpdateCurrentGrabPoint()
    {
        currentGrabPoint = grabPoints[selectedGrabPoint];

        grabPointUpdate(currentGrabPoint);
    }

    /**
     * Gets the current active Hand
     */
    public Transform GetActiveHand()
    {
        return currentGrabPoint.grabPosition;
    }

    /**
     * Gets the object in the current hand
     */
    public LocalGrabable GetCurrentGrabable()
    {
        return currentGrabPoint.grabedObject;
    }

    /**
     * Gets a LocalGrabable object and add it as a child of the
     * current grab point
     */
    public void AddGrabable(LocalGrabable grabable, Transform grabableTransform)
    {
        Transform hand = currentGrabPoint.grabPosition;
        if (hand != null && hand.childCount == 0)
        {
            currentGrabPoint.grabedObject = grabable;
            grabableTransform.parent = currentGrabPoint.grabPosition;
        }
    }

    /**
     * Removes the current LocalGrabable object
     */
    public void RemoveGrabable()
    {
        LocalGrabable searched = currentGrabPoint.grabedObject;
        if (searched != null)
        {
            Debug.Log("thrown remove: " + selectedGrabPoint);
            currentGrabPoint.grabedObject = null;

            // only if we are not in the first hand, we change it
            if (selectedGrabPoint == 1)
            {
                ChangeHand();
            }
        }
    }
}
