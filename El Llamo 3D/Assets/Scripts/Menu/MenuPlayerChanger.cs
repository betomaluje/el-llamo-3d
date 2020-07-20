using DG.Tweening;
using UnityEngine;

public class MenuPlayerChanger : MonoBehaviour
{
    [SerializeField] private float animationSpeed = 0.5f;
    [SerializeField] private GameObject singlePlayer;
    [SerializeField] private GameObject multiPlayer;
    [SerializeField] private GameObject tutorialPlayer;
    [SerializeField] private GameObject optionsPlayer;
    [SerializeField] private GameObject exitPlayer;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform hiddenTransform;

    private GameObject currentObject;

    private void Start()
    {
        currentObject = singlePlayer;
    }

    public void SelectSinglePlayer()
    {
        BringToFront(singlePlayer);
    }

    public void SelectMultiPlayer()
    {
        BringToFront(multiPlayer);
    }

    public void SelectTutorial()
    {
        BringToFront(tutorialPlayer);
    }

    public void SelectOptions()
    {
        BringToFront(optionsPlayer);
    }

    public void SelectExit()
    {
        BringToFront(exitPlayer);
    }

    private void BringToFront(GameObject target)
    {
        if (target == currentObject)
        {
            return;
        }

        target.SetActive(true);
        target.transform.DOMove(targetTransform.position, animationSpeed);

        Sequence s = DOTween.Sequence();
        s.Append(currentObject.transform.DOMove(hiddenTransform.position, animationSpeed));
        s.AppendCallback(() =>
        {
            currentObject.SetActive(false);
            currentObject = target;
        });
    }
}
