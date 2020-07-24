using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EPOOutline
{
    [ExecuteAlways]
    public class Outlinable : MonoBehaviour
    {
        public enum OutlineMask
        {
            None,
            Front,
            Back
        }

        private static Material basicOutlineMaterial;

        private static Material frontMask, backMask;

        private static HashSet<Outlinable> activeOutlinables = new HashSet<Outlinable>();

        [SerializeField]
        private int layer = 0;

        public int Layer
        {
            get
            {
                return layer;
            }

            set
            {
                layer = value;
            }
        }

        public float DilateShift
        {
            get
            {
                return dilateShift;
            }

            set
            {
                dilateShift = value;
            }
        }

        public float BlurShift
        {
            get
            {
                return blurShift;
            }

            set
            {
                blurShift = value;
            }
        }

        public Material OutlineMaterial
        {
            get
            {
                CheckMaterials();
                return basicOutlineMaterial;
            }
        }

        public Color OutlineColor
        {
            get
            {
                return outlineColor;
            }

            set
            {
                outlineColor = value;
            }
        }

        public OutlineMask Mask
        {
            get
            {
                return mask;
            }

            set
            {
                mask = value;
            }
        }

        public Material MaskMaterial
        {
            get
            {
                CheckMaterials();
                switch (mask)
                {
                    case OutlineMask.None:
                        return null;
                    case OutlineMask.Back:
                        return backMask;
                    case OutlineMask.Front:
                        return frontMask;
                    default:
                        return null;
                }
            }
        }

        public List<Renderer> Renderers
        {
            get
            {
                return renderers;
            }
        }

        [SerializeField]
        private OutlineMask mask;

        [SerializeField]
        private Color outlineColor = Color.green;

        [NonSerialized]
        private Material outlineMaterialInstance;

        [SerializeField]
        [HideInInspector]
        [Range(0.0f, 2.0f)]
        private float dilateShift = 1.0f;

        [SerializeField]
        [HideInInspector]
        [Range(0.0f, 2.0f)]
        private float blurShift = 1.0f;

        [SerializeField]
        [HideInInspector]
        private List<Renderer> renderers = new List<Renderer>();

        private int visiblePartsCount = 0;

        private void Awake()
        {
            CheckMaterials();

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                    continue;

                var listener = renderer.gameObject.GetComponent<VisibilityListener>();
                if (listener == null)
                    listener = renderer.gameObject.AddComponent<VisibilityListener>();

                listener.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable | HideFlags.HideInInspector;

                listener.OnVisibilityChanged = newState =>
                    {
                        if (newState)
                            visiblePartsCount++;
                        else
                            visiblePartsCount--;

                        if (visiblePartsCount == 0)
                            OnBecameInvisible();
                        else
                            OnBecameVisible();
                    };
            }
        }

        private void CheckMaterials()
        {
            if (!basicOutlineMaterial)
                basicOutlineMaterial = Resources.Load<Material>("Outline/Materials/Outline");

            if (!frontMask)
                frontMask = Resources.Load<Material>("Outline/Materials/Front mask");

            if (!backMask)
                backMask = Resources.Load<Material>("Outline/Materials/Back mask");
        }

        private void Reset()
        {
            CheckMaterials();
            AddAllChildMeshesToRenderingList();
        }

        [ContextMenu("Add all child meshes to rendering list")]
        public void AddAllChildMeshesToRenderingList()
        {
            renderers.Clear();
            GetComponentsInChildren<Renderer>(true, renderers);
        }

        public void GetRenderers(List<Renderer> result)
        {
            result.Clear();
            result.AddRange(renderers);
        }

        public static void GetAllActiveOutlinables(List<Outlinable> outlinables)
        {
            outlinables.Clear();
            outlinables.AddRange(activeOutlinables);
        }

        private void OnEnable()
        {
            activeOutlinables.Add(this);
        }

        private void OnDisable()
        {
            activeOutlinables.Remove(this);
        }

        private void OnBecameInvisible()
        {
            activeOutlinables.Remove(this);
        }

        private void OnBecameVisible()
        {
            if (!enabled)
                return;

            activeOutlinables.Add(this);
        }

        private void OnValidate()
        {
            CheckMaterials();
        }
    }
}