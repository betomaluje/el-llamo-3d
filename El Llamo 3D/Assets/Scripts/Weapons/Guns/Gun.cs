using BetoMaluje.Sikta;
using DG.Tweening;
using System;
using UnityEngine;

public abstract class Gun : MonoBehaviour, ITarget
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

    [Space]
    [Header("Pickup")]
    [SerializeField] private float speed = 0.5f;

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
        rb.useGravity = !isTargetDead;
        col.isTrigger = isTargetDead;
    }

    private void FinishPickup(Transform playerHand)
    {
        if (playerHand == null)
        {
            return;
        }

        Debug.Log("remote pickup 3: " + transform.position);

        SoundManager.instance.Play("Pickup");

        transform.parent = playerHand;

        transform.localPosition = Vector3.zero;
    }

    public void Pickup(Transform playerHand, Vector3 from, Vector3 to)
    {
        Debug.Log("remote pickup 2: " + from + " -> " + to);

        Vector3 rotation = new Vector3(-90, 0, 90);

        transform.position = from;

        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => ChangeSettings(true));
        s.AppendCallback(() => transform.DOMove(to, speed));
        s.AppendCallback(() => transform.DOLocalRotate(rotation, speed));
        s.AppendCallback(() => FinishPickup(playerHand));
    }

    public void Throw(float throwForce, Vector3 direction)
    {
        SoundManager.instance.Play("Throw");
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
        s.AppendCallback(() => ChangeSettings(false));
        s.AppendCallback(() => transform.parent = null);
        s.AppendCallback(() => rb.AddForce(direction * throwForce, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
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

    public TargetType getType()
    {
        return TargetType.Shootable;
    }

    public int GetDamage()
    {
        return UnityEngine.Random.Range(1, maxDamage);
    }
}
