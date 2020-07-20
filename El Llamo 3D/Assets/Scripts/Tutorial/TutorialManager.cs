using DG.Tweening;
using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float textAnimationTime = 0.5f;
    [SerializeField] private float restTime = 0.5f;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private CinemachineVirtualCamera enemyCamera;

    private bool enemyCameraShown = false;

    private string[] tutorialSteps = {
        "Use WASD to move",
        "Now use Mouse click to grab the gun",
        "Use the Mouse click to shoot",
        "Once killed, change hands by scrolling and grab the corpse",
        "Change the camera using Tab",
        "No try zooming in with the right Mouse click",
        "You can throw any onbject by pressing E",
        "You can grab and throw items as many times as you want",
        "Finally, use ESC to go to the Lobby or change settings"
    };

    private int currentIndex = -1;
    private WaitForSeconds waitForSeconds;

    private void Start()
    {
        waitForSeconds = new WaitForSeconds(restTime);
    }

    public void NextStep()
    {
        StartCoroutine(GoToNextStep());
    }

    public void PreviousStep()
    {
        StartCoroutine(GoToPreviousStep());
    }

    public void GoToStep(int step)
    {
        currentIndex = step;
        AnimateText(tutorialSteps[currentIndex]);
    }

    private IEnumerator GoToNextStep()
    {
        currentIndex++;
        AnimateText(tutorialSteps[currentIndex]);
        yield return waitForSeconds;
    }

    private IEnumerator GoToPreviousStep()
    {
        currentIndex--;
        AnimateText(tutorialSteps[currentIndex]);
        yield return waitForSeconds;
    }

    private void AnimateText(string text)
    {
        Sequence s = DOTween.Sequence();
        s.Append(stepText.DOFade(0, textAnimationTime));
        s.AppendCallback(() => stepText.SetText(text));
        s.AppendCallback(() => stepText.DOFade(1, textAnimationTime));
    }

    public void ShowEnemy()
    {
        if (!enemyCameraShown)
        {
            enemyCameraShown = true;
            StartCoroutine(ChangeToEnemyCamera());
        }
    }

    private IEnumerator ChangeToEnemyCamera()
    {
        enemyCamera.Priority = 11;
        yield return new WaitForSeconds(3);
        enemyCamera.Priority = 9;
    }
}
