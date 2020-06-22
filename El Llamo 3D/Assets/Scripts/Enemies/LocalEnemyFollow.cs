using UnityEngine;
using UnityEngine.AI;

public class LocalEnemyFollow : MonoBehaviour
{
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float radius = 20f;
    [SerializeField] private float wonderingDistance = 5f;

    protected NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        WonderAround();
    }

    protected virtual void FixedUpdate()
    {
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, attackLayer);
        if (colliders != null && colliders.Length > 0)
        {
            // follow the player
            agent.destination = colliders[0].transform.root.position;
        }
        else
        {
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                // here we need to make enemy wonder around
                WonderAround();
            }
        }
    }

    private Vector3 GetRandomPoint()
    {
        float rangeX = Random.Range(-wonderingDistance, wonderingDistance);
        float rangeZ = Random.Range(-wonderingDistance, wonderingDistance);
        Vector3 newPosition = transform.position;
        newPosition.x = newPosition.x + rangeX;
        newPosition.z = newPosition.z + rangeZ;

        return newPosition;
    }

    private void WonderAround()
    {
        // Set the agent to go to the currently selected destination.
        agent.destination = GetRandomPoint();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
