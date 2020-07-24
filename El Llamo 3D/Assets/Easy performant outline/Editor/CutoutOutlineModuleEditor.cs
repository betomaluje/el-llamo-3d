using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EPOOutline
{
    [CustomEditor(typeof(CutoutOutlineModule))]
    public class CutoutOutlineModuleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            var property = serializedObject.FindProperty("cutoutTexture");
            if (property.objectReferenceValue == null)
                EditorGUILayout.HelpBox(new GUIContent("Using property texture for cutout if not set different one."));

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(property);

            var module = target as CutoutOutlineModule;

            var renderer = module.GetComponent<Renderer>();
            var textureProperty = serializedObject.FindProperty("textureId");

            if (GUILayout.Button(textureProperty.stringValue, EditorStyles.layerMaskField) && renderer != null)
            {
                var material = renderer.sharedMaterial;
                if (material != null)
                {
                    var shader = material.shader;
                    if (shader != null)
                    {
                        var menu = new GenericMenu();
                        var propertiesCount = ShaderUtil.GetPropertyCount(shader);
                        for (var index = 0; index < propertiesCount; index++)
                        {
                            var propertyType = ShaderUtil.GetPropertyType(shader, index);
                            if (propertyType != ShaderUtil.ShaderPropertyType.TexEnv)
                                continue;

                            var currentName = ShaderUtil.GetPropertyName(shader, index);
                            var currentDescription = ShaderUtil.GetPropertyDescription(shader, index);
                            menu.AddItem(new GUIContent(currentDescription + "(" + currentName + ")"), currentName == textureProperty.stringValue, () =>
                                {
                                    textureProperty.stringValue = currentName;
                                    serializedObject.ApplyModifiedProperties();
                                });
                        }

                        menu.ShowAsContext();
                    }

                }
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
