using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalAmmoHandler : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private float updateSpeedSeconds = 0.2f;

    private LocalGun localGun;

    private void OnEnable()
    {
        localGun = GetComponentInParent<LocalGun>();
        localGun.OnAmmoChanged += HandleAmmoChanged;

        localGun.OnPickedStateChanged += OnWeaponPickedUp;

        container.SetActive(false);
    }

    private void OnDisable()
    {
        localGun.OnAmmoChanged -= HandleAmmoChanged;
        localGun.OnPickedStateChanged -= OnWeaponPickedUp;
    }

    private void OnWeaponPickedUp(bool grabbed)
    {
        container.SetActive(grabbed);
    }

    private void HandleAmmoChanged(int maxAmmo, int currentAmmo)
    {
        ammoText.SetText(currentAmmo + "/" + maxAmmo);
        StartCoroutine(ChangePercentage((float)currentAmmo / (float)maxAmmo));
    }

    private IEnumerator ChangePercentage(float ammoPercentage)
    {
        float prePercentage = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(prePercentage, ammoPercentage, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = ammoPercentage;
    }
}
