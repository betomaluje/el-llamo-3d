using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private UnityEvent onEnter = default, onStay = default, onExit = default;

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskUtils.LayerMatchesObject(layerMask, other.gameObject))
            onEnter.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMaskUtils.LayerMatchesObject(layerMask, other.gameObject))
            onStay.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMaskUtils.LayerMatchesObject(layerMask, other.gameObject))
            onExit.Invoke();
    }
}
