using BetoMaluje.Sikta;
using DG.Tweening;
using System;
using UnityEngine;

public abstract class Gun : MonoBehaviour, ITarget
{
    public Transform shootingPosition;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private int maxDamage = 30;

    public float impactForce = 100f;

    private float nextTimeToFire = 0f;
    private Rigidbody rb;
    private Collider col;

    public Action OnPickedUp;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected void ChangeSettings(bool isTargetDead)
    {
        if (rb == null || col == null)
        {
            return;
        }

        rb.isKinematic = isTargetDead;
        rb.interpolation = isTargetDead ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
        col.isTrigger = isTargetDead;
    }

    public void Pickup(PlayerGrab playerGrab, Transform weaponHolder)
    {
        playerGrab.target = this;
        ChangeSettings(true);

        transform.parent = weaponHolder;

        Vector3 rotation = new Vector3(-90, 0, 90);

        transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
        transform.DOLocalRotate(rotation, .25f).SetUpdate(true);

        OnPickedUp?.Invoke();
    }

    public void Throw(float throwForce)
    {
        SoundManager.instance.Play("Throw");
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
        s.AppendCallback(() => ChangeSettings(false));
        s.AppendCallback(() => transform.parent = null);
        s.AppendCallback(() => rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
    }

    public void Shoot(Vector3 shootHit)
    {
        if (Time.time >= nextTimeToFire)
        {
            // we can shoot here
            DoShooting(shootHit);
            // we update the frequency of the shooting
            nextTimeToFire = Time.time + 1f / fireRate;
        }
    }

    protected abstract void DoShooting(Vector3 shootHit);

    public TargetType getType()
    {
        return TargetType.Shootable;
    }

    public int GetDamage()
    {
        return UnityEngine.Random.Range(1, maxDamage);
    }
}
