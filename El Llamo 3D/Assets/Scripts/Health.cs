using System;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject dieBloodPrefab;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float timeChange = 1f;

    private int currentHealth;
    private Renderer meshRenderer;
    private Color[] originalColors;
    private float t = 0; // color lerp control variable

    private bool isDying = false;

    public event Action<float> OnHealthChanged = delegate { };

    private Rigidbody rb;
    private Collider col;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();

        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<Renderer>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInParent<Renderer>();
        }

        if (meshRenderer != null)
        {
            int materialsLength = meshRenderer.materials.Length;
            originalColors = new Color[materialsLength];

            for (int i = 0; i < materialsLength; i++)
            {
                originalColors[i] = meshRenderer.materials[i].color;
            }
        }

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (isDying)
        {
            int i = 0;
            foreach (var material in meshRenderer.materials)
            {
                Color color = originalColors[i];
                Color alpha = color;
                alpha.a = t;

                material.SetColor("_BaseColor", Color.Lerp(color, alpha, t));
                i++;
            }

            if (t < 1)
            {
                t += Time.deltaTime / timeChange;
            }
            else
            {
                t = 0;
                isDying = false;
            }
        }
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

        // fade out
        isDying = true;

        foreach (var material in meshRenderer.materials)
        {
            material.DOFade(0, timeChange);            
        }

        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        col.isTrigger = true;
    }
}
