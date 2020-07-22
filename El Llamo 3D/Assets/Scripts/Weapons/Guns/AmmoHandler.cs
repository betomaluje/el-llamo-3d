using SWNetwork;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHandler : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private float updateSpeedSeconds = 0.2f;

    #region Network
    private NetworkID networkID;
    private SyncPropertyAgent syncPropertyAgent;

    private const string CURRENT_AMMO_EVENT = "CurrentAmmo";

    private int maxAmmo = 0;
    private int currentAmmo = 0;

    #endregion

    private Gun gun;

    private void Awake()
    {
        networkID = GetComponentInParent<NetworkID>();
        syncPropertyAgent = GetComponentInParent<SyncPropertyAgent>();
        gun = GetComponentInParent<Gun>();

        maxAmmo = gun.GetGun().ammo;
    }

    private void OnEnable()
    {
        gun.OnAmmoChanged += HandleAmmoChanged;

        gun.OnPickedStateChanged += OnWeaponPickedUp;

        container.SetActive(false);
    }

    private void OnDisable()
    {
        gun.OnAmmoChanged -= HandleAmmoChanged;
        gun.OnPickedStateChanged -= OnWeaponPickedUp;
    }

    private void OnWeaponPickedUp(bool grabbed)
    {
        if (networkID.IsMine)
        {
            container.SetActive(grabbed);
            UpdateAmmo();
        }
    }

    public void OnAmmoConflict(SWSyncConflict conflict, SWSyncedProperty property)
    {
        // 1
        int newLocalAmmo = (int)conflict.newLocalValue;
        int oldLocalAmmo = (int)conflict.oldLocalValue;
        int remoteAmmo = (int)conflict.remoteValue;

        // 2
        // check if player is already killed
        if (remoteAmmo == 0)
        {
            property.Resolve(0);
            return;
        }

        // 3
        // should use remoteHP instead of oldLocalHP to apply damage
        int damage = oldLocalAmmo - newLocalAmmo;
        int resolvedAmmo = remoteAmmo - damage;
        if (resolvedAmmo < 0)
        {
            resolvedAmmo = 0;
        }
        property.Resolve(resolvedAmmo);
    }

    public void OnCurrentAmmoReady()
    {
        int newAmmo = syncPropertyAgent.GetPropertyWithName(CURRENT_AMMO_EVENT).GetIntValue();
        int version = syncPropertyAgent.GetPropertyWithName(CURRENT_AMMO_EVENT).version;

        currentAmmo = newAmmo;

        // If version is 0, you can call the Modify() method on the SyncPropertyAgent to initialize player's ammo to currentAmmo.
        if (version == 0)
        {
            syncPropertyAgent.Modify(CURRENT_AMMO_EVENT, currentAmmo);
            currentAmmo = maxAmmo;
        }
    }

    public void RemoteCurrentAmmoChanged()
    {
        // Update the hpSlider when player hp changes
        int newAmmo = syncPropertyAgent.GetPropertyWithName(CURRENT_AMMO_EVENT).GetIntValue();

        currentAmmo = newAmmo;

        UpdateAmmo();
    }

    private void HandleAmmoChanged(int maxAmmo, int currentAmmo)
    {
        syncPropertyAgent?.Modify(CURRENT_AMMO_EVENT, currentAmmo);
    }

    private void UpdateAmmo()
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
