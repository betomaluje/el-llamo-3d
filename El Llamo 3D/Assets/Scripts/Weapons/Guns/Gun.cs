using BetoMaluje.Sikta;
using DG.Tweening;
using SWNetwork;
using System;
using UnityEngine;

public abstract class Gun : Grabable, ITarget
{
    [Header("Stats")]
    public Transform shootingPosition;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private int maxDamage = 30;
    public float impactForce = 100f;

    [Space]
    [Header("Recoil")]
    [SerializeField] private float recoilAmount = 0.3f;
    [SerializeField] private float recoilDuration = 0.3f;

    private float nextTimeToFire = 0f;

    public Action OnPickedUp;

    private void FixedUpdate()
    {
        if (getParentTransform().parent != null)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public void Shoot(Vector3 shootHit)
    {
        if (Time.time >= nextTimeToFire)
        {
            // we can shoot here
            DoRecoil(shootHit);

            DoShooting(shootHit);
            // we update the frequency of the shooting
            nextTimeToFire = Time.time + 1f / fireRate;
        }
    }

    protected abstract void DoShooting(Vector3 shootHit);

    public void DoRecoil(Vector3 shootHit)
    {
        Vector3 direction = -shootingPosition.right * recoilAmount;
        transform.DOPunchPosition(direction, recoilDuration, 1, 0.05f);
    }

    public int GetDamage()
    {
        return UnityEngine.Random.Range(1, maxDamage);
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
    }

    public override TargetType getTargetType()
    {
        return TargetType.Shootable;
    }

    protected override RemoteEventAgent getRemoteEventAgent()
    {
        return GetComponent<RemoteEventAgent>();
    }
}
