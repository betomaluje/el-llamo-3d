using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private LayerMask healingLayer;
    [SerializeField] private int healthAmount = 10;
    [SerializeField] private GameObject[] healParticles;
    [SerializeField] private bool instantiateNew = true;

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

        SoundManager.instance.Play("Heal");

        foreach (GameObject particle in healParticles)
        {
            if (instantiateNew)
            {
                Instantiate(particle, Camera.main.transform.position, Quaternion.identity);
            }
            else
            {
                particle.SetActive(true);
            }
        }

        Destroy(gameObject);
    }
}
