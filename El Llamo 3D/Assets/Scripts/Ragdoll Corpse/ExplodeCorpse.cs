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
        Transform[] childs = GetComponentsInChildren<Transform>();

        foreach (Transform t in childs)
        {
            Rigidbody rb = t.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = -t.forward;
                rb.AddExplosionForce(force, direction, radius);
            }
        }
    }
}
