using UnityEngine;
using DG.Tweening;

public class MaterialColorChanger : MonoBehaviour
{
    [Header("Color settings")]
    [SerializeField] private Color targetColor;
    [SerializeField] private float timeChange = 0.5f;

    [Space]
    [Header("Prefab settings")]
    public GameObject targetLock;
    [SerializeField] private float targetAnimationDuration = 0.2f;
    private Vector3 originalPos;

    private Renderer meshRenderer;
    private Color originalColor;    

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();

        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<Renderer>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInParent<Renderer>();
        }

        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }

        originalPos = targetLock.transform.position;
    }

    public void TargetOn()
    {
        PrefabLockTarget();

        foreach (var material in meshRenderer.materials)
        {
            material.DOColor(targetColor, timeChange);
        }
    }

    public void TargetOff()
    {
        ResetPrefabLockTarget();

        foreach (var material in meshRenderer.materials)
        {
            material.DOColor(originalColor, timeChange);
        }
    }

    private void PrefabLockTarget()
    {
        if (targetLock == null) return;

        targetLock.SetActive(true);
        Vector3 rotation = Vector3.zero;
        rotation.x = 90;
        rotation.z = 180;

        Vector3 position = transform.position;
        position.y = 0.5f;

        targetLock.transform.DOMove(position, targetAnimationDuration).SetEase(Ease.OutBack).SetUpdate(true);
        targetLock.transform.DOLocalRotate(rotation, targetAnimationDuration).SetUpdate(true);
    }

    private void ResetPrefabLockTarget()
    {
        if (targetLock == null) return;

        Vector3 rotation = Vector3.zero;
        rotation.x = 90;
        rotation.z = 0;

        Sequence s = DOTween.Sequence();
        s.Append(targetLock.transform.DOLocalRotate(rotation, targetAnimationDuration)).SetUpdate(true);
        s.Append(targetLock.transform.DOMove(originalPos, targetAnimationDuration).SetEase(Ease.OutBack)).SetUpdate(true);
        s.AppendCallback(() => targetLock.SetActive(false));
    }
}
