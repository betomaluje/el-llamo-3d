using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabController : MonoBehaviour
{
    [Header("Grab Points")]
    [SerializeField] private List<GrabPoint> grabPoints;
    [Space]
    [Header("UI")]
    [SerializeField] private Image[] playerHandsUI;
    [SerializeField] private Color playerHandUIColor;

    // 0: right, 1 left
    private int selectedGrabPoint = 0;

    private GrabPoint currentGrabPoint;

    private void Start()
    {
        UpdateCurrentGrabPoint();
    }

    private void Update()
    {
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d != 0f)
        {
            ChangeHand();
        }
    }

    private void LateUpdate()
    {
        CheckHandWeights();
    }

    private void CheckHandWeights()
    {
        foreach (var grabPoint in grabPoints)
        {
            float weight = 0f;

            if (grabPoint.grabedObject != null)
            {
                weight = 1f;
            }

            grabPoint.UpdateWeight(weight);
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

        Debug.Log("manual hand selected: " + selectedGrabPoint);

        UpdateCurrentGrabPoint();
    }

    private void UpdateCurrentGrabPoint()
    {
        currentGrabPoint = grabPoints[selectedGrabPoint];

        ChangeHandsUI(selectedGrabPoint);
    }

    private void ChangeHandsUI(int selectedGrabbable)
    {
        int i = 0;
        foreach (Image hand in playerHandsUI)
        {
            if (i == selectedGrabbable)
            {
                hand.color = playerHandUIColor;
                hand.gameObject.transform.DOScale(1.3f, 0.25f);
            }
            else
            {
                hand.color = Color.white;
                hand.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            i++;
        }
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
            Debug.Log("add grabable: " + grabable.gameObject);
            currentGrabPoint.grabedObject = grabable;
            grabableTransform.parent = currentGrabPoint.grabPosition;
        }
    }

    /**
     * Removes the current LocalGrabable object
     */
    public void RemoveGrabable(LocalGrabable grabable)
    {
        GrabPoint searched = grabPoints.Find(obj => obj.grabedObject == grabable);
        if (searched != null)
        {
            // if it's the same as the current one, we reset it
            if (currentGrabPoint == searched)
            {
                currentGrabPoint.grabedObject = null;
            }
            searched.grabedObject = null;

            // only if we are not in the first hand, we change it
            if (selectedGrabPoint == 1)
            {
                ChangeHand();
            }
        }
    }

    public void RemoveAllGrabables()
    {
        foreach (GrabPoint grabPoint in grabPoints)
        {
            Debug.Log("thrown remove: " + grabPoint.grabedObject);
            if (grabPoint.grabedObject != null)
            {
                //Debug.Log("thrown remove: " + grabPoint.grabedObject);
                grabPoint.grabedObject.StartThrow(100f, Vector3.up);
            }
        }
    }
}
