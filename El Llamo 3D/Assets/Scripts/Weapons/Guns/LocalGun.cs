using DG.Tweening;
using System;
using UnityEngine;

public abstract class LocalGun : LocalGrabable, IGun
{
    [Header("Stats")]
    public Transform shootingPosition;
    [SerializeField] private GunSO gun;

    private float nextTimeToFire = 0f;

    public Action<bool> OnPickedStateChanged = delegate { };

    public Action<int, int> OnAmmoChanged = delegate { };

    private int currentAmmo = 0;

    protected override void Start()
    {
        base.Start();
        currentAmmo = gun.ammo;
        OnAmmoChanged(gun.ammo, currentAmmo);
    }

    private void FixedUpdate()
    {
        if (getParentTransform().parent != null)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public bool Shoot(Vector3 shootHit)
    {
        bool successful = false;

        if (Time.time >= nextTimeToFire)
        {
            // we can shoot here
            DoRecoil();

            if (currentAmmo > 0)
            {
                currentAmmo--;

                OnAmmoChanged(gun.ammo, currentAmmo);

                successful = true;
                DoShooting(shootHit, shootingPosition);
            }

            // we update the frequency of the shooting
            nextTimeToFire = Time.time + 1f / gun.fireRate;
        }

        return successful;
    }

    protected abstract void DoShooting(Vector3 shootHit, Transform shootingPosition);

    public void DoRecoil()
    {
        Vector3 direction = -shootingPosition.right * gun.recoilAmount;
        transform.DOPunchPosition(direction, gun.recoilDuration, 1, 0.05f);
    }

    public int GetDamage()
    {
        return UnityEngine.Random.Range(gun.minDamage, gun.maxDamage);
    }

    public float GetImpactForce()
    {
        return gun.impactForce;
    }

    protected override Transform getParentTransform()
    {
        return transform;
    }

    public override void Pickup(Vector3 to)
    {
        Vector3 rotation = new Vector3(-90, 0, 90);

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(to, pickupSpeed));
        s.AppendCallback(() => transform.DOLocalRotate(rotation, pickupSpeed));
        s.AppendCallback(() =>
        {
            SoundManager.instance.Play("Pickup");

            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.Euler(rotation);

            sphereCollider.enabled = false;

            OnPickedStateChanged(true);

            OnAmmoChanged(gun.ammo, currentAmmo);

            Debug.Log("finish pickup " + gameObject.name);
        });
    }

    public override void Throw(float throwForce, Vector3 direction)
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(transform.position - transform.forward, .01f));
        s.AppendCallback(() => ChangeSettings(false));
        s.AppendCallback(() => transform.parent = null);
        s.AppendCallback(() => rb.AddForce(direction * throwForce, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));

        Debug.Log("finish throwing " + gameObject.name);

        OnPickedStateChanged(false);
    }

    public override TargetType getTargetType()
    {
        return TargetType.Shootable;
    }

    public bool CanShoot()
    {
        return IsGrabbed() && currentAmmo > 0;
    }

    public override bool ShouldChangeOutline()
    {
        return true;
    }
}
