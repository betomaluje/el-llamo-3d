using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GrabController : MonoBehaviour
{
    [Header("Grab Points")]
    [SerializeField] private GrabPoint[] grabPoints;
    [SerializeField] private KeyCode pointChanger = KeyCode.E;
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
    public void RemoveGrabable()
    {
        LocalGrabable searched = currentGrabPoint.grabedObject;
        if (searched != null)
        {
            currentGrabPoint.grabedObject = null;

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
