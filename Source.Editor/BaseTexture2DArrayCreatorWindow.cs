// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Editor
{
    public class BaseTexture2DArrayCreatorWindow : EditorWindow
    {
        [SerializeField] List<Texture2D?> _sourceTextures = new();
        SerializedObject? _serializedObject;
        SerializedProperty? _sourceTexturesProperty;

        string _saveAssetPath = "Assets/TextureArray.asset";

        bool _mipMaps = true;
        TextureWrapMode _wrapMode = TextureWrapMode.Repeat;
        FilterMode _filterMode = FilterMode.Bilinear;

        Vector2 _scrollPosition;

        protected virtual void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _sourceTexturesProperty = _serializedObject.FindProperty(nameof(_sourceTextures));
        }

        protected virtual void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.HelpBox("Have to match texture size and format for all textures.\nAnd have to set 'Read/Write' option for all textures.", MessageType.Info);
            EditorGUILayout.Space();

            _serializedObject?.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_sourceTexturesProperty, new GUIContent("Source Textures"), true);
            EditorGUI.EndChangeCheck();
            _serializedObject?.ApplyModifiedProperties();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            _mipMaps = EditorGUILayout.Toggle("Generate Mipmap", _mipMaps);
            _wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Wrap Mode", _wrapMode);
            _filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", _filterMode);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save Path", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _saveAssetPath = EditorGUILayout.TextField(_saveAssetPath);
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save TextureArray", "TextureArray", "asset", "Select save file location.");
                if (!string.IsNullOrEmpty(path))
                {
                    _saveAssetPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Create"))
            {
                CreateTextureArray();
            }

            EditorGUILayout.EndScrollView();
        }

        void CreateTextureArray()
        {
            List<Texture2D>? validTextures = ValidateTextures();
            if (validTextures is null)
            {
                return;
            }

            string directory = Path.GetDirectoryName(_saveAssetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Texture2D firstValidTexture = validTextures[0];
            int width = firstValidTexture.width;
            int height = firstValidTexture.height;

            Texture2DArray textureArray = new(width, height, validTextures.Count, TextureFormat.RGBA32, _mipMaps)
            {
                wrapMode = _wrapMode,
                filterMode = _filterMode,
            };

            for (int i = 0; i < validTextures.Count; i++)
            {
                textureArray.SetPixels32(validTextures[i].GetPixels32(), i);
            }

            textureArray.Apply(_mipMaps);

            AssetDatabase.CreateAsset(textureArray, _saveAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Saved: "+ _saveAssetPath);

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2DArray>(_saveAssetPath);
        }

        List<Texture2D>? ValidateTextures()
        {
            bool hasTexture = false;
            for (int i = 0; i < _sourceTextures.Count; i++)
            {
                if (_sourceTextures[i] != null)
                {
                    hasTexture = true;
                    break;
                }
            }
            if (!hasTexture)
            {
                EditorUtility.DisplayDialog("Error", "Have to input at least one texture.", "OK");
                return null;
            }

            Texture2D? firstValidTexture = null;
            for (int i = 0; i < _sourceTextures.Count; i++)
            {
                if (_sourceTextures[i] != null)
                {
                    firstValidTexture = _sourceTextures[i];
                    break;
                }
            }
            if (firstValidTexture == null)
            {
                return null;
            }

            List<Texture2D> result = new();
            for (int i = 0; i < _sourceTextures.Count; i++)
            {
                Texture2D? other = _sourceTextures[i];
                if (other == null)
                {
                    continue;
                }

                if (other.width != firstValidTexture.width ||
                    other.height != firstValidTexture.height)
                {
                    EditorUtility.DisplayDialog("Error",
                        $"Size of the all textures must be the same. Texture size at {i} (0-based index) is different with first valid one.",
                        "OK");
                    return null;
                }
                if (!other.isReadable)
                {
                    EditorUtility.DisplayDialog("Error",
                        $"Texture at {i} (0-based index) is not readable.\nSet 'Read/Write' option in the texture's inspector.",
                        "OK");
                    return null;
                }

                result.Add(other);
            }

            return result;
        }
    }
}
