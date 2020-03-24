﻿using BetoMaluje.Sikta;
using DG.Tweening;
using UnityEngine;

public class ThrowableRagdoll : MonoBehaviour, ITarget
{
    [SerializeField] private Vector3 handsOffset;
    private Rigidbody rb;
    private Collider col;

    private Transform parentTransform;
    private bool isGrabbed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        parentTransform = Utils.GetComponentInParents<Transform>(gameObject);
    }

    private void LateUpdate()
    {
        if (isGrabbed)
        {
            parentTransform.localPosition = handsOffset;
        }
    }

    private void ChangeSettings(bool isTargetDead)
    {
        if (parentTransform.parent != null)
        {
            return;
        }

        rb.isKinematic = isTargetDead;
        rb.interpolation = isTargetDead ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
        rb.useGravity = isTargetDead;
        col.isTrigger = isTargetDead;
    }

    public void Pickup(PlayerGrab playerGrab, Transform weaponHolder)
    {
        playerGrab.target = this;
        isGrabbed = true;
        ChangeSettings(true);

        parentTransform.transform.parent = weaponHolder;

        transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
        transform.DOLocalRotate(Vector3.zero, .25f).SetUpdate(true);
    }

    public void Throw(float throwForce, Vector3 direction)
    {
        SoundManager.instance.Play("Throw");
        isGrabbed = false;

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
        s.AppendCallback(() => parentTransform.parent = null);
        s.AppendCallback(() => ChangeSettings(false));
        s.AppendCallback(() => rb.AddForce(direction * throwForce, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
    }

    public void Shoot(Vector3 shootHit)
    {
        // not implemented here        
    }

    public TargetType getType()
    {
        return TargetType.Throwable;
    }

    public int GetDamage()
    {
        return 0;
    }
}
