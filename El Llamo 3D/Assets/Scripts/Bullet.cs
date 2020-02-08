using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionParticles;
    public int maxDamage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
