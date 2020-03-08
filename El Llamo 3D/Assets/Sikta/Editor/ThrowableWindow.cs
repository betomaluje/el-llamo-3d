using UnityEditor;
using UnityEngine;

namespace BetoMaluje.Sikta
{
    public class ThrowableWindow : EditorWindow
    {
        [SerializeField] private GameObject lockTargetPrefab;

        [SerializeField] private LayerMask targetMask;

        [SerializeField] private TargetType type;

        [MenuItem("Tools/Sikta/Make Target")]
        public static void OpenWindow()
        {
            GetWindow<ThrowableWindow>("Target");
        }

        private void OnGUI()
        {
            GUILayout.Label("You need to select at least one Game Object");
            EditorGUILayout.Space();

            type = (TargetType)EditorGUILayout.EnumPopup("Type of Target:", type);

            targetMask = EditorGUILayout.LayerField("Layer for Target:", targetMask);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Hover Prefab");
            lockTargetPrefab = EditorGUILayout.ObjectField(lockTargetPrefab, typeof(GameObject), true) as GameObject;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Make Target"))
            {
                if (Selection.gameObjects.Length <= 0)
                {
                    EditorGUILayout.HelpBox("You need to select at least one Game Object first", MessageType.Warning);
                }
                else
                {
                    MakeThrowable();
                }
            }
        }

        private void MakeThrowable()
        {
            Vector3 rotation = Vector3.zero;
            rotation.x = 90;
            rotation.z = 0;

            GameObject target = null;

            foreach (GameObject selectedGO in Selection.gameObjects)
            {
                // step 0: check for any collider
                if (selectedGO.GetComponent<Collider>() == null)
                {
                    ShowNotification(new GUIContent(selectedGO.name + " doesn't have a collider. Skipping"));
                    continue;
                }

                // step 1: add rigidbody

                if (selectedGO.GetComponent<Rigidbody>() == null)
                {
                    selectedGO.AddComponent<Rigidbody>();
                }

                // step 2: add the change material script

                if (selectedGO.GetComponent<MaterialColorChanger>() == null)
                {
                    selectedGO.AddComponent<MaterialColorChanger>();
                }

                // step 3: add the target lock script

                if (lockTargetPrefab != null)
                {
                    target = PrefabUtility.InstantiatePrefab(lockTargetPrefab) as GameObject;
                    target.transform.parent = selectedGO.transform;

                    target.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                    target.transform.localPosition = Vector3.zero;

                    AddTargetAsPrefab(selectedGO, target);

                    target.SetActive(false);
                }

                // step 4: add the throwable script 

                if (type.Equals(TargetType.Throwable))
                {
                    if (selectedGO.GetComponent<ThrowableTarget>() == null)
                    {
                        selectedGO.AddComponent<ThrowableTarget>();
                    }
                }
                else if (type.Equals(TargetType.Shootable))
                {
                    if (selectedGO.GetComponent<Gun>() == null)
                    {
                        selectedGO.AddComponent<Gun>();
                    }
                }

                // step 5: change the objects layer
                if (target != null)
                {
                    ChangeGOLayers(selectedGO, target);
                }
            }
        }

        private void AddTargetAsPrefab(GameObject selectedGO, GameObject targetLock)
        {
            MaterialColorChanger colorChanger = selectedGO.GetComponent<MaterialColorChanger>();

            if (colorChanger != null)
            {
                colorChanger.targetLock = targetLock;
            }
        }

        private void ChangeGOLayers(GameObject parentGO, GameObject targetLock)
        {
            parentGO.layer = targetMask;

            foreach (Transform child in parentGO.transform)
            {
                if (child.gameObject != targetLock)
                {
                    // we change the objects layer
                    child.gameObject.layer = targetMask;
                }
            }
        }

        private void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}
