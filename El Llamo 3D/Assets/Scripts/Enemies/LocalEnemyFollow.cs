using UnityEngine;
using UnityEngine.AI;

public class LocalEnemyFollow : MonoBehaviour
{
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float radius = 20f;

    //Transform that NPC has to follow
    private Transform transformToFollow;

    protected NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, attackLayer);
        if (colliders != null && colliders.Length > 0)
        {
            transformToFollow = colliders[0].transform.root;
        }
        else
        {
            transformToFollow = null;
        }

        if (transformToFollow != null)
        {
            // follow the player
            agent.destination = transformToFollow.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
