using DG.Tweening;
using UnityEngine;

public class MenuPlayerChanger : MonoBehaviour
{
    [SerializeField] private float animationSpeed = 0.5f;
    [SerializeField] private GameObject singlePlayer;
    [SerializeField] private GameObject multiPlayer;
    [SerializeField] private GameObject optionsPlayer;
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

    public void SelectOptions()
    {
        BringToFront(optionsPlayer);
    }

    private void BringToFront(GameObject target)
    {
        if (target == currentObject)
        {
            return;
        }

        target.transform.DOMove(targetTransform.position, animationSpeed);
        currentObject.transform.DOMove(hiddenTransform.position, animationSpeed);

        currentObject = target;
    }

    /*
     *
     *
    private float targetX, hiddenX;

    private void Start()
    {
        targetX = targetTransform.position.x;
        hiddenX = hiddenTransform.position.x;
    }

    public void SelectSinglePlayer()
    {
        singlePlayer.transform.DOMoveX(targetX, animationSpeed);
        multiPlayer.transform.DOMoveX(hiddenX, animationSpeed);
    }

    public void SelectMultiPlayer()
    {
        multiPlayer.transform.DOMoveX(targetX, animationSpeed);
        singlePlayer.transform.DOMoveX(hiddenX, animationSpeed);
    }
     * 
     */

}
