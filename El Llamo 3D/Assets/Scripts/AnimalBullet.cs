﻿using UnityEngine;
using DG.Tweening;

public class AnimalBullet : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayer;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float animationDuration = 1.5f;

    private bool startMoving = false;

    private void Awake()
    {
        transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0);
    }

    private void OnEnable()
    {
        transform.DOScale(new Vector3(1f, 1f, 1f), animationDuration);
    }

    // Update is called once per frame
    void Update()
    {
        if (startMoving)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.Rotate(0f, -1f, 0f);            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckLayerMask(collision.gameObject) && !startMoving)
        {
            startMoving = true;
        }
    }

    private bool CheckLayerMask(GameObject target)
    {
        return (triggerLayer & 1 << target.layer) == 1 << target.layer;
    }
}
