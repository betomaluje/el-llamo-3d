using UnityEngine;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Transform crosshair;

    [SerializeField] private float distanceFromCamera = 5f;
    [SerializeField] private float lerpDuration = 0.2f;

    [SerializeField] private bool animated = false;

    [Space]
    [Header("Zoom Effect")]
    [SerializeField] private float distanceForAnimation = 5f;
    [SerializeField] private float zoomDuration = 0.2f;
    [SerializeField] private Transform zoomedCrosshair;
    [SerializeField] private float zoomAmount = 2f;
    [SerializeField] private float rotationAngle = 180f;

    private Camera mainCam;
    private InputHandler inputHandler;

    private bool isZoomedIn = false;
    
    void Start()
    {
        mainCam = Camera.main;
    }

    public void SetupPlayer(InputHandler handler) 
    {
        inputHandler = handler;
         // handle target
        inputHandler.targetAquired += HandleTargetAquired;
    }

    private void OnDisable() {
        inputHandler.targetAquired -= HandleTargetAquired;
    }

    private void HandleTargetAquired(RaycastHit targetHit, bool onTarget)
        {
            if (onTarget)
            {
                if (targetHit.distance <= distanceForAnimation && !isZoomedIn)
                {
                    MakeBig();
                    isZoomedIn = true;   
                }                
            }
            else
            {
                if (isZoomedIn) 
                {
                    MakeNNormal();
                    isZoomedIn = false;
                }            
            }
        }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 resultingPosition = mainCam.transform.position + mainCam.transform.forward * distanceFromCamera;
        if (animated) 
        {
            transform.DOMove(resultingPosition, lerpDuration);
        } else 
        {
            transform.position = resultingPosition;
        }    

        transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + mainCam.transform.forward);       
    }

    public void MakeBig() 
    {
        zoomedCrosshair.DOScale(zoomAmount, zoomDuration);
        zoomedCrosshair.DOPunchRotation(new Vector3(0, 0, rotationAngle), zoomDuration, 1, 0.2f);
    }

    public void MakeNNormal() 
    {
        zoomedCrosshair.DOScale(1, zoomDuration);
        zoomedCrosshair.DOPunchRotation(new Vector3(0, 0, -rotationAngle), zoomDuration, 1, 0.2f);
    }
}
