using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private LayerMask healingLayer;
    [SerializeField] private int healthAmount = 10;
    [SerializeField] private GameObject[] healParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & healingLayer) != 0)
        {
            //It matched one
            Heal(other.gameObject);
        }
    }

    private void Heal(GameObject player)
    {
        IHealth health = player.GetComponent<IHealth>();
        if (health != null)
        {
            health.GiveHealth(healthAmount, Vector3.zero);
        }

        SoundManager.instance.Play("Heal");

        Destroy(gameObject);
    }
}
