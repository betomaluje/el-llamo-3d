using System.Collections;
using UnityEngine;
using Llamo.Health;

public class PosessHandler : MonoBehaviour
{
    [SerializeField] private LocalInputHandler inputHandler;
    [SerializeField] private float posessTimeReset = 0.5f;

    private void OnEnable()
    {
        inputHandler.targetAquired += HandleTargetPosess;
        inputHandler.secondaryClickCallback += HandleSecondaryClick;
    }

    private void OnDisable()
    {
        inputHandler.targetAquired -= HandleTargetPosess;
        inputHandler.secondaryClickCallback -= HandleSecondaryClick;
    }

    private void HandleSecondaryClick()
    {

    }

    private void HandleTargetPosess(PointingTarget pointingTarget)
    {
        if (pointingTarget.onTarget && pointingTarget.isPressed)
        {
            StartCoroutine(StartPosess(pointingTarget.targetHit.collider));
        }
    }

    private IEnumerator StartPosess(Collider target)
    {
        PosessController minePosessController = GetComponent<PosessController>();
        if (minePosessController != null)
        {
            minePosessController.DisableComponents();
            Debug.Log("Stop posessing " + minePosessController.gameObject.name);
        }

        PosessController otherPosessController = target.GetComponent<PosessController>();
        if (otherPosessController != null)
        {
            Debug.Log("Posessing " + otherPosessController.gameObject.name);
            otherPosessController.EnableComponents();

            // we deactivate current game object
            LocalHealth localHealth = GetComponent<LocalHealth>();
            if (localHealth != null)
            {
                localHealth.Posess();
            }
        }

        yield return new WaitForSeconds(posessTimeReset);
    }
}
