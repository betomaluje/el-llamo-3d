using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private LayerMask healingLayer;
    [SerializeField] private int healthAmount = 10;
    [SerializeField] private GameObject healParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Heal(other.gameObject);
        }
    }

    private void Heal(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            health.GiveHealth(healthAmount);
        }

        Instantiate(healParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
