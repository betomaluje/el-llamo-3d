using UnityEngine;

public class ExplodeCorpse : MonoBehaviour
{
    [SerializeField] private float force = 600f;
    [SerializeField] private float radius = 4f;

    private void Start()
    {
        Explode();
    }

    private void Explode()
    {
        Rigidbody[] childs = GetComponentsInChildren<Rigidbody>(true);

        foreach (Rigidbody rb in childs)
        {
            Vector3 direction = -rb.transform.forward;
            rb.AddExplosionForce(force, direction, radius);
        }
    }
}
