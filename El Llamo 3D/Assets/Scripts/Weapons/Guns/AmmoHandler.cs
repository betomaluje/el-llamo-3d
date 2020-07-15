using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoHandler : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private float updateSpeedSeconds = 0.2f;

    private Gun gun;

    private void OnEnable()
    {
        gun = GetComponentInParent<Gun>();
        gun.OnAmmoChanged += HandleAmmoChanged;

        gun.OnPickedStateChanged += OnWeaponPickerUp;

        container.SetActive(false);
    }

    private void OnDisable()
    {
        gun.OnAmmoChanged -= HandleAmmoChanged;
        gun.OnPickedStateChanged -= OnWeaponPickerUp;
    }

    private void OnWeaponPickerUp(bool grabbed)
    {
        container.SetActive(grabbed);
    }

    private void HandleAmmoChanged(int maxAmmo, int currentAmmo)
    {
        Debug.Log("current ammo: " + currentAmmo);
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
