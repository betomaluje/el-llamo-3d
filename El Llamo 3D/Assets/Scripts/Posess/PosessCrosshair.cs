using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PosessCrosshair : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float updateSpeedSeconds = 0.2f;

    public Action posessReady = delegate { };

    private float difficultyTime;
    private bool isRunning = false;

    private Coroutine posessCoroutine;

    private void Start()
    {
        foregroundImage.fillAmount = 0;
        foregroundImage.DOFade(0, 0);
        backgroundImage.DOFade(0, 0);
    }

    public void StartPosessCrosshair(float difficultyTime)
    {
        this.difficultyTime = difficultyTime;

        Tween t1 = foregroundImage.DOFade(1, updateSpeedSeconds);
        Tween t2 = backgroundImage.DOFade(1, updateSpeedSeconds);

        Sequence s = DOTween.Sequence();
        s.Join(t1);
        s.Join(t2);
        s.AppendCallback(() =>
        {
            isRunning = true;
            posessCoroutine = StartCoroutine(StartPosessing());
        });
    }

    public void Reset()
    {
        Debug.Log("Cancelling posessing");
        isRunning = false;
        foregroundImage.fillAmount = 0;
        foregroundImage.DOFade(0, updateSpeedSeconds);
        backgroundImage.DOFade(0, updateSpeedSeconds);
        StopCoroutine(posessCoroutine);
    }


    private IEnumerator StartPosessing()
    {
        WaitForSeconds secondsToWait = new WaitForSeconds(difficultyTime);

        Debug.Log("crosshair StartPosessing");
        int prePercentage = 0;

        while (prePercentage <= 100 && isRunning)
        {
            prePercentage++;
            foregroundImage.fillAmount = prePercentage / 100f;

            yield return secondsToWait;
        }

        Debug.Log("crosshair StartPosessing FINISHED: " + prePercentage);

        if (prePercentage >= 100)
        {
            posessReady();
        }

        Reset();
    }
}
