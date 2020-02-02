﻿using UnityEngine;
using DG.Tweening;

public class Gun : MonoBehaviour, ITarget
{
    [SerializeField] private Transform shootingPosition;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootingSpeed = 100;

    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void ChangeSettings()
    {
        if (transform.parent != null)
            return;

        rb.isKinematic = (PlayerGrab.instance.weapon == this) ? true : false;
        rb.interpolation = (PlayerGrab.instance.weapon == this) ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
        col.isTrigger = (PlayerGrab.instance.weapon == this);
    }

    public void Pickup(Transform weaponHolder)
    {
        PlayerGrab.instance.weapon = this;
        ChangeSettings();

        transform.parent = weaponHolder;

        Vector3 rotation = new Vector3(-90, 0, 90);

        transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
        transform.DOLocalRotate(rotation, .25f).SetUpdate(true);
    }

    public void Throw(float throwForce)
    {
        // not implemented here
    }

    public void Shoot()
    {
        // we can shoot here
        GameObject bullet = Instantiate(bulletPrefab, shootingPosition.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(shootingPosition.right * shootingSpeed, ForceMode.Impulse);
    }

    public TargetType getType()
    {
        return TargetType.Shootable;
    }
}
