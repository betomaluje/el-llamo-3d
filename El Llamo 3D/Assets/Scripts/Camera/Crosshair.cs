using UnityEngine;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    [Header("Zoom Effect")]
    [SerializeField] private float distanceForAnimation = 5f;
    [SerializeField] private float zoomDuration = 0.2f;
    [SerializeField] private Transform zoomedCrosshair;
    [SerializeField] private float zoomAmount = 2f;
    [SerializeField] private float rotationAngle = 180f;
    private InputHandler inputHandler;

    private bool isZoomedIn = false;
    
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

    public void MakeBig() 
    {
        zoomedCrosshair.DOScale(zoomAmount, zoomDuration);
        zoomedCrosshair.DOPunchRotation(new Vector3(0, 0, rotationAngle), zoomDuration, 1, 0.2f);
    }

    public void MakeNNormal() 
    {
        zoomedCrosshair.DOScale(1, zoomDuration);

        Sequence s = DOTween.Sequence();
        s.Append(zoomedCrosshair.DOPunchRotation(new Vector3(0, 0, -rotationAngle), zoomDuration, 1, 0.2f)).SetUpdate(true);
        s.AppendCallback(() => zoomedCrosshair.rotation = Quaternion.Euler(0, 0, 0));
    }
}
