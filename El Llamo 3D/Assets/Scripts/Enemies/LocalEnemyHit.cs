using System.Collections;
using UnityEngine;
using Llamo.Health;

public class LocalEnemyHit : MonoBehaviour
{
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float timeToReset = 0.5f;
    [SerializeField] private int damage = 10;

    private bool hitEnabled = true;

    private void Update()
    {
        if (!hitEnabled)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, attackLayer);
        if (colliders != null && colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                // we give priority to the shield
                IHealth corpseHealth = col.transform.root.GetComponentInChildren<IHealth>(true);
                if (corpseHealth != null)
                {
                    corpseHealth.PerformDamage(damage, col.transform.position);
                    hitEnabled = false;

                    StartCoroutine(ReEnableHit());
                    return;
                }
            }
        }
    }

    private IEnumerator ReEnableHit()
    {
        yield return new WaitForSeconds(timeToReset);
        hitEnabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
