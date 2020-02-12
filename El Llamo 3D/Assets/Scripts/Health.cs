using System;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public event Action<float> OnHealthChanged = delegate { };

    private Rigidbody rb;
    private Collider col;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }    

    public void ModifyHealth(int amount)
    {
        currentHealth += amount;

        float healthPercentage = (float)currentHealth / (float)maxHealth;
        OnHealthChanged(healthPercentage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(dieBloodPrefab, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

        Vector3 currentRotation = transform.position;
        currentRotation.z = UnityEngine.Random.Range(0, 2) == 0 ? -90 : 90;
        currentRotation.x = 0;
        
        float deadY = 0.5f;

        transform.DOMoveY(deadY, 1f);

        Sequence s = DOTween.Sequence();
        s.Append(transform.DORotate(currentRotation, 1f)).SetUpdate(true);
        s.AppendCallback(() => MakeUntouchable());        
    }

    private void MakeUntouchable()
    {
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        col.isTrigger = true;
    }
}
