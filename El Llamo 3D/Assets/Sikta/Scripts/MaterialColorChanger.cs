using UnityEngine;
using DG.Tweening;

namespace BetoMaluje.Sikta
{
    public class MaterialColorChanger : MonoBehaviour
    {
        [Header("Color settings")]
        [SerializeField] private Color targetColor;
        [SerializeField] private float timeChange = 0.5f;

        [Space]
        [Header("Prefab settings")]
        public GameObject targetLock;
        [SerializeField] private float targetAnimationDuration = 0.2f;
        private Vector3 originalPos;

        private Renderer meshRenderer;
        private Color[] originalColors;

        private bool isTargetOn = false;
        private bool isTargetOff = false;
    
        private float t = 0; // color lerp control variable

        public bool isEnabled = true;

        private void Awake()
        {
            Health healthScript = GetComponent<Health>();
            if (healthScript != null)
            {
                healthScript.OnHealthChanged += HandleHealth;
            }
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

            originalPos = targetLock.transform.position;
        }

        private void Update()
        {
            // check if it's not enabled and reset color and prefab if it's on
            if (!isEnabled)
            {
                int i = 0;
                foreach (var material in meshRenderer.materials)
                {
                    material.SetColor("_BaseColor", Color.Lerp(material.color, originalColors[i], t));
                    i++;
                }

                if (t < 1)
                {
                    t += Time.deltaTime / timeChange;
                }

                targetLock.SetActive(false);

                return;
            }

            if (isTargetOn)
            {
                int i = 0;
                foreach (var material in meshRenderer.materials)
                {
                    material.SetColor("_BaseColor", Color.Lerp(originalColors[i], targetColor, t));
                    i++;
                }
            
                if (t < 1)
                {
                    t += Time.deltaTime / timeChange;
                } else
                {
                    t = 0;
                    isTargetOn = false;
                }
            }

            if (isTargetOff)
            {
                int i = 0;
                foreach (var material in meshRenderer.materials)
                {
                    material.SetColor("_BaseColor", Color.Lerp(targetColor, originalColors[i], t));
                    i++;
                }

                if (t < 1)
                {
                    t += Time.deltaTime / timeChange;
                }
                else
                {
                    t = 0;
                    isTargetOff = false;
                }
            }
        }

        private void HandleHealth(float healthPercentage)
        {
            if (healthPercentage <= 0)
            {
                isEnabled = false;
            }
        }

        public void TargetOn()
        {
            PrefabLockTarget();
            isTargetOn = true;
        }

        public void TargetOff()
        {
            ResetPrefabLockTarget();
            isTargetOff = true;
        }

        private void PrefabLockTarget()
        {
            if (targetLock == null) return;

            targetLock.SetActive(true);
            Vector3 rotation = Vector3.zero;
            rotation.x = 90;
            rotation.z = 180;

            Vector3 position = transform.position;
            position.y = 0.5f;

            targetLock.transform.DOMove(position, targetAnimationDuration).SetEase(Ease.OutBack).SetUpdate(true);
            targetLock.transform.DOLocalRotate(rotation, targetAnimationDuration).SetUpdate(true);
        }

        private void ResetPrefabLockTarget()
        {
            if (targetLock == null) return;

            Vector3 rotation = Vector3.zero;
            rotation.x = 90;
            rotation.z = 0;

            Sequence s = DOTween.Sequence();
            s.Append(targetLock.transform.DOLocalRotate(rotation, targetAnimationDuration)).SetUpdate(true);
            s.Append(targetLock.transform.DOMove(originalPos, targetAnimationDuration).SetEase(Ease.OutBack)).SetUpdate(true);
            s.AppendCallback(() => targetLock.SetActive(false));
        }
    }
}

