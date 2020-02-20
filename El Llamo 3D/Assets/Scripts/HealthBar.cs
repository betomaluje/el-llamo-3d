using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private float updateSpeedSeconds = 0.2f;
    [SerializeField] private float yAmount = 6f;
    [SerializeField] private float hideTime = 1f;

    private Vector3 originalPosition;
    private bool isShown = false;
    private Transform mainCameraPos;
    
    private void Awake()
    {
        GetComponentInParent<Health>().OnHealthChanged += HandleHealth;
        originalPosition = transform.position;        

        Vector3 hidePos = originalPosition;
        hidePos.y -= yAmount;
        transform.position = hidePos;

        foregroundImage.DOFade(0, 0);
        backgroundImage.DOFade(0, 0);
    }

    private void OnEnable()
    {
        mainCameraPos = Camera.main.transform;
    }

    private void HandleHealth(float healthPercentage)
    {
        StartCoroutine(ChangePercentage(healthPercentage));
    }

    private IEnumerator ChangePercentage(float healthPercentage)
    {
        if (!isShown)
        {
            Show();
        }

        float prePercentage = foregroundImage.fillAmount;
        float elapsed = 0f;        

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(prePercentage, healthPercentage, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = healthPercentage;

        if (healthPercentage <= 0)
        {
            Hide();
        }
    }

    private void LateUpdate()
    {
        if (mainCameraPos == null)
        {
            return;
        }

        transform.LookAt(transform.position + mainCameraPos.forward);        

        //AlignTransform(transform);
    }

    private void Show()
    {
        isShown = true;

        transform.DOMove(originalPosition, hideTime);
        foregroundImage.DOFade(1, 0.4f);
        backgroundImage.DOFade(1, 0.4f);       
    }

    private void Hide()
    {
        if (isShown)
        {           
            foregroundImage.DOFade(0, hideTime);
            backgroundImage.DOFade(0, hideTime);          
        }        
    }

    private void AlignTransform(Transform transform)
    {
        Vector3 sample = SampleNormal(transform.position);

        Vector3 proj = transform.forward - (Vector3.Dot(transform.forward, sample)) * sample;
        transform.rotation = Quaternion.LookRotation(proj, sample);
    }

    private Vector3 SampleNormal(Vector3 position)
    {
        Terrain terrain = Terrain.activeTerrain;
        var terrainLocalPos = position - terrain.transform.position;
        var normalizedPos = new Vector2(
            Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
            Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z)
        );
        var terrainNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        return terrainNormal;
    }
}
