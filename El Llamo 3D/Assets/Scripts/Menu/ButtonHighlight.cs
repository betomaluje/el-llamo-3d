using DG.Tweening;
using UnityEngine;

public class ButtonHighlight : MonoBehaviour
{
    [SerializeField] private float finalScale = 1.2f;
    [SerializeField] private float scaleDuration = 0.25f;

    public void OnSelect()
    {
        transform.DOScale(finalScale, scaleDuration);
    }

    public void OnDeselect()
    {
        transform.DOScale(1, scaleDuration);
    }
}