using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private float updateSpeedSeconds = 0.2f;
    [SerializeField] private float yAmount = 6f;
    [SerializeField] private float hideTime = 1f;
    
    private bool isShown = false;
    private Transform mainCameraPos;
    
    private void Awake()
    {
        GetComponentInParent<Health>().OnHealthChanged = HandleHealth;        

        foregroundImage.DOFade(0, 0);
        backgroundImage.DOFade(0, 0);
    }

    private void OnEnable()
    {
        mainCameraPos = Camera.main.transform;
    }

    private void HandleHealth(float healthPercentage)
    {
        StartCoroutine(ChangePercentage(healthPercentage));
    }

    private IEnumerator ChangePercentage(float healthPercentage)
    {
        if (!isShown)
        {
            Show();
        }

        float prePercentage = foregroundImage.fillAmount;
        float elapsed = 0f;        

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(prePercentage, healthPercentage, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = healthPercentage;

        if (healthPercentage <= 0)
        {
            Hide();
        }
    }

    private void LateUpdate()
    {
        if (mainCameraPos == null)
        {
            return;
        }

        transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + mainCameraPos.forward);
    }

    private void Show()
    {
        isShown = true;
        
        foregroundImage.DOFade(1, 0.4f);
        backgroundImage.DOFade(1, 0.4f);       
    }

    private void Hide()
    {
        if (isShown)
        {           
            foregroundImage.DOFade(0, hideTime);
            backgroundImage.DOFade(0, hideTime);
            isShown = false;
        }        
    }
   
}
