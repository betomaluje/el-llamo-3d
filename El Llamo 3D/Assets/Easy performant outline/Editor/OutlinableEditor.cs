using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EPOOutline
{
    [CustomEditor(typeof(Outlinable))]
    public class OutlinableEditor : Editor
    {
        UnityEditorInternal.ReorderableList list;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var dilate = serializedObject.FindProperty("dilateShift");
            var blur = serializedObject.FindProperty("blurShift");
            var renderers = serializedObject.FindProperty("renderers");

            dilate.isExpanded = EditorGUILayout.Foldout(dilate.isExpanded, "Info buffer related settings");

            if (dilate.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.HelpBox(new GUIContent("The options in this group will only take affect if info buffer is enabled on Outliner component/Renderer feature"));
                EditorGUILayout.PropertyField(dilate);
                EditorGUILayout.PropertyField(blur);
                EditorGUI.indentLevel = 0;
            }

            if (list == null)
            {
                list = new UnityEditorInternal.ReorderableList(serializedObject, renderers);

                list.drawHeaderCallback = position => EditorGUI.LabelField(position, "Renderers. All renderers that will be included to outline rendering should be in the list.");

                list.drawElementCallback = (position, item, isActive, isFocused) => EditorGUI.PropertyField(position, renderers.GetArrayElementAtIndex(item), GUIContent.none);

                list.onAddDropdownCallback = (buttonRect, targetList) =>
                    {
                        var outlinable = target as Outlinable;
                        var items = outlinable.gameObject.GetComponentsInChildren<Renderer>(true);
                        var menu = new GenericMenu();

                        foreach (var item in items)
                        {
                            var found = false;
                            for (var index = 0; index < renderers.arraySize; index++)
                            {
                                var element = renderers.GetArrayElementAtIndex(index);
                                if (element.objectReferenceValue == item)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            var path = string.Empty;
                            if (item.transform != outlinable.transform)
                            {
                                var parent = item.transform;
                                do
                                {
                                    path = string.Format("{0}/{1}", parent.ToString(), path);
                                    parent = parent.transform.parent;
                                }
                                while (parent != outlinable.transform);

                                path = string.Format("{0}/{1}", parent.ToString(), path);

                                path = path.Substring(0, path.Length - 1);
                            }
                            else
                                path = item.ToString();

                            GenericMenu.MenuFunction function = () =>
                                {
                                    var index = renderers.arraySize;
                                    renderers.InsertArrayElementAtIndex(index);
                                    var arrayItem = renderers.GetArrayElementAtIndex(index);
                                    arrayItem.objectReferenceValue = item;
                                    serializedObject.ApplyModifiedProperties();
                                };

                            if (found)
                                function = null;

                            menu.AddItem(new GUIContent("Add all"), false, () =>
                                {
                                    (target as Outlinable).AddAllChildMeshesToRenderingList();

                                    EditorUtility.SetDirty(target);
                                });

                            menu.AddItem(new GUIContent(path), found, function);
                        }

                        menu.ShowAsContext();
                    };

                list.onRemoveCallback = (targetList) =>
                    {
                        renderers.DeleteArrayElementAtIndex(targetList.index);
                        renderers.DeleteArrayElementAtIndex(targetList.index);

                        serializedObject.ApplyModifiedProperties();
                    };
            }

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}