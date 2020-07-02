using System.Collections;
using UnityEngine;
using Llamo.Health;

public class PosessHandler : MonoBehaviour
{
    [SerializeField] private LocalInputHandler inputHandler;
    [SerializeField] private float posessTimeReset = 0.5f;
    public float posessTimeDifficulty = 0.1f;

    private PosessCrosshair crosshair;

    private Collider lastTarget;

    private bool wasPosessing = false;
    private bool wasSuccessful = false;

    private void Start()
    {
        crosshair = FindObjectOfType<PosessCrosshair>();
        crosshair.posessReady += OnPosessReady;
    }

    private void OnEnable()
    {
        inputHandler.posessAquired += HandleTargetPosess;
        inputHandler.fireReleaseCallback += HandleClickReleased;
        if (crosshair != null)
        {
            crosshair.posessReady += OnPosessReady;
        }
    }

    private void OnDisable()
    {
        inputHandler.posessAquired -= HandleTargetPosess;
        inputHandler.fireReleaseCallback -= HandleClickReleased;

        crosshair.posessReady -= OnPosessReady;
    }

    private void HandleClickReleased()
    {
        if (wasPosessing)
        {
            ResetCrosshair(wasSuccessful);
        }
    }

    private void OnPosessReady()
    {
        if (lastTarget != null)
        {
            wasSuccessful = true;
            StartCoroutine(StartPosess(lastTarget));
        }
    }

    private void HandleTargetPosess(PosessTarget pointingTarget)
    {
        if (pointingTarget.onTarget && pointingTarget.isPressed)
        {
            if (!wasPosessing)
            {
                // tell crosshair to start loading
                PosessHandler targetPosessHandler = pointingTarget.targetHit.collider.GetComponent<PosessHandler>();
                if (targetPosessHandler != null)
                {
                    crosshair.StartPosessCrosshair(targetPosessHandler.posessTimeDifficulty);
                    lastTarget = pointingTarget.targetHit.collider;
                    wasSuccessful = false;
                    wasPosessing = true;
                }
            }
        }
        else
        {
            if (wasPosessing)
            {
                ResetCrosshair(wasSuccessful);
            }
        }
    }

    private void ResetCrosshair(bool successful)
    {
        if (crosshair != null)
        {
            if (successful)
            {
                crosshair.Reset();
            }
            else
            {
                crosshair.ResetCancelled();
            }
        }

        lastTarget = null;
        wasPosessing = false;
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

        ResetCrosshair(wasSuccessful);

        yield return new WaitForSeconds(posessTimeReset);
    }
}
