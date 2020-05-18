using UnityEngine;

public class EnemyGrab : MonoBehaviour
{
    private Collider mainCollider;
    private Collider[] allColliders;

    private Rigidbody rb;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        mainCollider = GetComponent<Collider>();
        allColliders = GetComponentsInChildren<Collider>(true);

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        DoRagdoll(false);
    }

    public void DoRagdoll(bool isRagdoll)
    {
        foreach (Collider col in allColliders)
        {
            col.enabled = isRagdoll;
        }

        mainCollider.enabled = !isRagdoll;

        rb.useGravity = !isRagdoll;
        animator.enabled = !isRagdoll;
    }
}
