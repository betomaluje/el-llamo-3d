using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Llamo.Health
{
    public class EnemyDisplayDamage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float animationDuration = 1f;

        private LocalEnemyHealth healthScript;

        private bool isShown = false;

        private void Awake()
        {
            healthScript = GetComponentInParent<LocalEnemyHealth>();
            damageText.DOFade(0, 0);
        }

        private void OnEnable()
        {
            healthScript.OnDamagePerformed += HandleDamage;
            healthScript.OnCurrentChange += HandleHealth;
        }

        private void OnDisable()
        {
            healthScript.OnDamagePerformed -= HandleDamage;
            healthScript.OnCurrentChange -= HandleHealth;
        }

        private void HandleDamage(int damage)
        {
            damageText.text = damage.ToString();

            if (!isShown)
            {
                DoAppear();
            }
        }

        private void HandleHealth(int currentHealth, int maxHealth)
        {
            healthText.text = currentHealth + "/" + maxHealth;
        }

        private void DoAppear()
        {
            isShown = true;
            damageText.transform.DOLocalMoveY(70, animationDuration);
            Sequence s = DOTween.Sequence();
            s.Append(damageText.DOFade(1, animationDuration));
            s.AppendCallback(() => StartCoroutine(DoDisappear()));
        }

        private IEnumerator DoDisappear()
        {
            yield return new WaitForSeconds(1f);
            damageText.transform.DOLocalMoveY(0, animationDuration);
            Sequence s = DOTween.Sequence();
            s.Append(damageText.DOFade(0, animationDuration));
            s.AppendCallback(() => isShown = false);
        }
    }

}