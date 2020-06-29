using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Llamo.Health
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private bool alwaysFaceCamera = true;

        [SerializeField] private Image foregroundImage;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private float updateSpeedSeconds = 0.2f;

        private LocalEnemyHealth healthScript;

        private void Awake()
        {
            healthScript = GetComponentInParent<LocalEnemyHealth>();
        }

        private void OnEnable()
        {
            healthScript.OnHealthChanged += HandleHealthChanged;
        }

        private void OnDisable()
        {
            healthScript.OnHealthChanged -= HandleHealthChanged;
        }

        private void HandleHealthChanged(float healthPercentage)
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
    }
}