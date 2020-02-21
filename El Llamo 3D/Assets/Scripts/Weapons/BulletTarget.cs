using UnityEngine;

public class BulletTarget : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;

    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
    }

    public void PerformDamage(int damage)
    {
        health.ModifyHealth(-damage);
    }

    private bool CheckLayerMask(GameObject target)
    {
        return (targetLayerMask & 1 << target.layer) == 1 << target.layer;
    }
}
