using UnityEngine;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float distanceFromCamera = 5f;
    [SerializeField] private float animationDuration = 0.2f;

    [SerializeField] private bool animated = false;

    private Camera mainCam;
    
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 resultingPosition = mainCam.transform.position + mainCam.transform.forward * distanceFromCamera;
        if (animated) 
        {
            transform.DOMove(resultingPosition, animationDuration);
        } else 
        {
            transform.position = resultingPosition;
        }    

        transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + mainCam.transform.forward);       
    }
}
