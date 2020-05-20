using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private bool alwaysFaceCamera = true;

    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private float updateSpeedSeconds = 0.2f;

    private Transform mainCameraPos;
    private Health healthScript;

    private void Awake()
    {
        healthScript = GetComponentInParent<Health>();
    }

    private void OnEnable()
    {
        mainCameraPos = Camera.main.transform;
        healthScript.OnHealthChanged += HandleHealth;
    }

    private void OnDisable()
    {
        healthScript.OnHealthChanged -= HandleHealth;
    }

    private void HandleHealth(float healthPercentage)
    {
        StartCoroutine(ChangePercentage(healthPercentage));
    }

    private IEnumerator ChangePercentage(float healthPercentage)
    {
        float prePercentage = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(prePercentage, healthPercentage, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = healthPercentage;
    }

    private void LateUpdate()
    {
        if (mainCameraPos == null || !alwaysFaceCamera)
        {
            return;
        }

        transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + mainCameraPos.forward);
    }
}
