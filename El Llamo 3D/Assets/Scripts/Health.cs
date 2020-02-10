using System;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float timeChange = 1f;

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
        Debug.Log(gameObject.name + " is dead!");
        Instantiate(dieBloodPrefab, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

        Vector3 currentPos = transform.position;
        currentPos.z = UnityEngine.Random.Range(0, 2) == 0 ? -90 : 90;
        currentPos.x = 0;

        Sequence s = DOTween.Sequence();
        s.Append(transform.DORotate(currentPos, 1f)).SetUpdate(true);
        s.AppendCallback(() => MakeUntouchable());

        
    }

    private void MakeUntouchable()
    {
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        col.isTrigger = true;
    }
}
