using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float radius = 5f;

    private const string KEY_ATTACK = "Attacking";

    private void Update()
    {
        bool attacking = Physics.CheckSphere(transform.position, radius, attackLayer);
        animator.SetBool(KEY_ATTACK, attacking);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
