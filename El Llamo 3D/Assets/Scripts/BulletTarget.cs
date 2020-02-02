using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTarget : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckLayerMask(collision.gameObject))
        {
            Debug.Log(gameObject.name + " is dying!");
        }
    }

    private bool CheckLayerMask(GameObject target)
    {
        return (targetLayerMask & 1 << target.layer) == 1 << target.layer;
    }
}
