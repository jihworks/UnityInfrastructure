// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Geometries;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Editor.Geometries
{
    public abstract class BaseConvexHullGeneratorWindow : EditorWindow
    {
        [SerializeField] List<Mesh?> _sourceMeshes = new();
        [SerializeField] List<GameObject?> _sourceQuickAdds = new();
        [SerializeField] List<GameObject?> _sourceGameObjects = new();
        SerializedObject? _serializedObject;
        SerializedProperty? _sourceMeshesProperty, _sourceQuickAddsProperty, _sourceGameObjectsProperty;

        string _meshesSaveDirectory = "Assets";
        string _meshesFileNameFormat = "{0} ConvexHull";

        bool _meshesPreviewConvexHull = true;
        GameObject? _meshesPreviewRoot;

        readonly List<Stats> _meshesStats = new();
        bool _meshesShowStats = false;

        string _objectsSaveDirectory = "Assets";
        string _objectsFileNameFormat = "{0} ConvexHull";

        Vector2 _scrollPosition;

        Material? _convexHullPreviewMaterial, _sourceMeshPreviewMaterial;

        protected abstract void CreatePreviewMaterials(out Material convexHullPreviewMaterial, out Material sourceMeshPreviewMaterial);
        protected abstract void GenerateHull(Vector3[] points, out List<Vector3> hullVertices, out List<int> hullTriangles);

        protected virtual void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _sourceMeshesProperty = _serializedObject.FindProperty(nameof(_sourceMeshes));
            _sourceQuickAddsProperty = _serializedObject.FindProperty(nameof(_sourceQuickAdds));
            _sourceGameObjectsProperty = _serializedObject.FindProperty(nameof(_sourceGameObjects));

            CreatePreviewMaterials(out _convexHullPreviewMaterial, out _sourceMeshPreviewMaterial);
            Meshes_ResetStats();
        }

        protected virtual void OnDisable()
        {
            Meshes_DestroyPreview();
        }

        protected virtual void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _serializedObject?.Update();

            GUILayout.Label("From Meshes", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_sourceMeshesProperty, new GUIContent("Source Meshes"), true);
            if (EditorGUI.EndChangeCheck())
            {
                Meshes_ResetStats();
                Meshes_DestroyPreview();
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_sourceQuickAddsProperty, new GUIContent("Quick Adds"), true);
            if (GUILayout.Button("Quick Add"))
            {
                Meshes_QuickAdd();
            }
            _serializedObject?.ApplyModifiedProperties();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Save Directory", GUILayout.Width(110));
            _meshesSaveDirectory = EditorGUILayout.TextField(_meshesSaveDirectory, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                Meshes_BrowseSaveDirectory();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Name Format", GUILayout.Width(110));
            _meshesFileNameFormat = EditorGUILayout.TextField(_meshesFileNameFormat, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("'{0}' will be replaced by source mesh name.\nExample: '{0} ConvexHull' -> 'SourceMeshName ConvexHull.asset'", MessageType.Info);

            EditorGUILayout.Space();
            _meshesPreviewConvexHull = EditorGUILayout.Toggle("Preview Convex Hull", _meshesPreviewConvexHull);

            EditorGUILayout.Space();
            bool hasValidMesh = _sourceMeshes.Exists(m => m != null);
            EditorGUI.BeginDisabledGroup(!hasValidMesh);
            if (GUILayout.Button("Save Convex Hulls"))
            {
                Meshes_SaveConvexHulls();
            }
            EditorGUI.EndDisabledGroup();

            if (hasValidMesh && _meshesPreviewConvexHull)
            {
                if (GUILayout.Button("Update Preview"))
                {
                    Meshes_UpdatePreview();
                }
            }

            if (_meshesPreviewRoot != null)
            {
                if (GUILayout.Button("Clear Preview"))
                {
                    Meshes_DestroyPreview();
                }
            }

            MeshesDrawEfficiencyStatsList();

            EditorGUILayout.EndVertical();

            GUILayout.Label("From Objects", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(_sourceGameObjectsProperty, new GUIContent("Source Objects"), true);
            _serializedObject?.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Save Directory", GUILayout.Width(110));
            _objectsSaveDirectory = EditorGUILayout.TextField(_objectsSaveDirectory, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                Objects_BrowseSaveDirectory();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Name Format", GUILayout.Width(110));
            _objectsFileNameFormat = EditorGUILayout.TextField(_objectsFileNameFormat, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("'{0}' will be replaced by source mesh name.\nExample: '{0} ConvexHull' -> 'SourceMeshName ConvexHull.asset'", MessageType.Info);

            EditorGUILayout.Space();
            bool hasValidObject = _sourceGameObjects.Exists(m => m != null);
            EditorGUI.BeginDisabledGroup(!hasValidObject);
            if (GUILayout.Button("Save Convex Hulls"))
            {
                Objects_SaveConvexHulls();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        void Meshes_QuickAdd()
        {
            List<Mesh?> result = new(_sourceMeshes);

            foreach (var go in _sourceQuickAdds)
            {
                if (go == null)
                {
                    continue;
                }

                foreach (var childTransform in go.transform.EnumerateChildrenTree(true))
                {
                    MeshFilter[] meshFilters = childTransform.gameObject.GetComponents<MeshFilter>();
                    foreach (var meshFilter in meshFilters)
                    {
                        if (meshFilter.sharedMesh != null && !result.Contains(meshFilter.sharedMesh))
                        {
                            result.Add(meshFilter.sharedMesh);
                        }
                    }
                }
            }

            _sourceMeshes = result;
        }

        void MeshesDrawEfficiencyStatsList()
        {
            if (!_meshesShowStats || _meshesStats.Count == 0)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Efficiency Statistics", EditorStyles.boldLabel);

            foreach (var stats in _meshesStats)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField($"Target: {stats.MeshName}", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Original", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"Verts: {stats.OriginalVertices}");
                EditorGUILayout.LabelField($"Tris: {stats.OriginalTriangles}");
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Convex Hull", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"Verts: {stats.HullVertices}");
                EditorGUILayout.LabelField($"Tris: {stats.HullTriangles}");
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Efficiency", EditorStyles.miniBoldLabel);

                GUIStyle vStyle = new(EditorStyles.label) { normal = { textColor = GetEfficiencyColor(stats.VertexReduction), }, };
                GUIStyle tStyle = new(EditorStyles.label) { normal = { textColor = GetEfficiencyColor(stats.TriangleReduction), }, };

                EditorGUILayout.LabelField($"-{stats.VertexReduction:F1}% (V)", vStyle);
                EditorGUILayout.LabelField($"-{stats.TriangleReduction:F1}% (T)", tStyle);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }
        }

        void Meshes_BrowseSaveDirectory()
        {
            string path = EditorUtility.OpenFolderPanel("Select Save Directory", _meshesSaveDirectory, "");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogWarning("Save directory must be inside the project's Assets folder.");
                return;
            }

            _meshesSaveDirectory = "Assets" + path[Application.dataPath.Length..];
        }
        void Objects_BrowseSaveDirectory()
        {
            string path = EditorUtility.OpenFolderPanel("Select Save Directory", _objectsSaveDirectory, "");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogWarning("Save directory must be inside the project's Assets folder.");
                return;
            }

            _objectsSaveDirectory = "Assets" + path[Application.dataPath.Length..];
        }

        void Meshes_SaveConvexHulls()
        {
            if (!Directory.Exists(_meshesSaveDirectory))
            {
                Directory.CreateDirectory(_meshesSaveDirectory);
            }

            Meshes_ResetStats();

            for (int i = 0; i < _sourceMeshes.Count; i++)
            {
                Mesh? mesh = _sourceMeshes[i];
                if (mesh == null)
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("Generating", $"Processing {mesh.name}", (float)i / _sourceMeshes.Count);

                GenerateHull(mesh.vertices, out List<Vector3> hullVertices, out List<int> hullTriangles);

                _meshesStats.Add(CreateStats(mesh, hullVertices, hullTriangles));

                SerializableMesh serializableMesh = CreateInstance<SerializableMesh>();
                SetSerializableMeshData(serializableMesh, hullVertices, hullTriangles);

                string savePath = Path.Combine(_meshesSaveDirectory, string.Format(_meshesFileNameFormat, mesh.name) + ".asset").Replace("\\", "/");
                AssetDatabase.CreateAsset(serializableMesh, savePath);

                Debug.Log("Saved: " + savePath);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            _meshesShowStats = true;

            if (_meshesPreviewConvexHull)
            {
                Meshes_UpdatePreview();
            }
        }
        void Objects_SaveConvexHulls()
        {
            if (!Directory.Exists(_objectsSaveDirectory))
            {
                Directory.CreateDirectory(_objectsSaveDirectory);
            }

            for (int i = 0; i < _sourceGameObjects.Count; i++)
            {
                GameObject? go = _sourceGameObjects[i];
                if (go == null)
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("Generating", $"Processing {go.name}", (float)i / _sourceGameObjects.Count);

                List<Vector3> allVertices = new();
                foreach (var childTransform in go.transform.EnumerateChildrenTree(true))
                {
                    MeshFilter[] meshFilters = childTransform.gameObject.GetComponents<MeshFilter>();
                    foreach (var meshFilter in meshFilters)
                    {
                        if (meshFilter.sharedMesh == null)
                        {
                            continue;
                        }
                        Vector3[] vertices = meshFilter.sharedMesh.vertices;
                        Matrix4x4 localToWorld = meshFilter.transform.localToWorldMatrix;
                        foreach (var vertex in vertices)
                        {
                            allVertices.Add(localToWorld.MultiplyPoint3x4(vertex));
                        }
                    }
                }
                GenerateHull(allVertices.ToArray(), out List<Vector3> hullVertices, out List<int> hullTriangles);

                SerializableMesh serializableMesh = CreateInstance<SerializableMesh>();
                SetSerializableMeshData(serializableMesh, hullVertices, hullTriangles);

                string savePath = Path.Combine(_objectsSaveDirectory, string.Format(_objectsFileNameFormat, go.name) + ".asset").Replace("\\", "/");
                AssetDatabase.CreateAsset(serializableMesh, savePath);

                Debug.Log("Saved: " + savePath);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }

        static Stats CreateStats(Mesh source, IReadOnlyList<Vector3> hullVertices, IReadOnlyList<int> hullTriangles)
        {
            int originalV = source.vertexCount;
            int originalT = source.triangles.Length / 3;
            int hullV = hullVertices.Count;
            int hullT = hullTriangles.Count / 3;

            return new Stats(source.name, originalV, originalT, hullV, hullT,
                100f * (1f - (float)hullV / originalV),
                100f * (1f - (float)hullT / originalT));
        }

        void Meshes_UpdatePreview()
        {
            Meshes_DestroyPreview();
            Meshes_ResetStats();

            if (_sourceMeshes.Count <= 0)
            {
                return;
            }

            _meshesPreviewRoot = new GameObject("ConvexHull_Preview_Root") { hideFlags = HideFlags.DontSave, };

            foreach (var mesh in _sourceMeshes)
            {
                if (mesh == null)
                {
                    continue;
                }

                GenerateHull(mesh.vertices, out List<Vector3> hullVertices, out List<int> hullTriangles);
                _meshesStats.Add(CreateStats(mesh, hullVertices, hullTriangles));

                string previewName = $"Preview_{mesh.name}";

                Mesh previewMesh = new() { name = previewName, };
                previewMesh.SetVertices(hullVertices);
                previewMesh.SetTriangles(hullTriangles, 0);
                previewMesh.RecalculateNormals();

                {
                    GameObject gameObject = new(previewName);
                    gameObject.transform.SetParent(_meshesPreviewRoot.transform, false);

                    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = previewMesh;

                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = _convexHullPreviewMaterial;
                }
                {
                    GameObject gameObject = new($"Source_{mesh.name}");
                    gameObject.transform.SetParent(_meshesPreviewRoot.transform, false);

                    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = mesh;

                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = _sourceMeshPreviewMaterial;
                }
            }

            _meshesShowStats = true;
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        void SetSerializableMeshData(SerializableMesh mesh, List<Vector3> vertices, List<int> triangles)
        {
            mesh.SetVertices(vertices);
            SerializableSubMesh subMesh = new();
            subMesh.SetIndices(triangles);
            mesh.SetSubMeshes(subMesh);
        }

        static Color GetEfficiencyColor(float reduction)
        {
            if (reduction >= 90f)
            {
                return Color.green;
            }
            if (reduction >= 70f)
            {
                return Color.Lerp(Color.yellow, Color.green, (reduction - 70f) / 20f);
            }
            if (reduction >= 30f)
            {
                return Color.Lerp(Color.red, Color.yellow, (reduction - 30f) / 40f);
            }
            return Color.red;
        }

        void Meshes_ResetStats()
        {
            _meshesStats.Clear();
            _meshesShowStats = false;
        }

        void Meshes_DestroyPreview()
        {
            if (_meshesPreviewRoot != null)
            {
                DestroyImmediate(_meshesPreviewRoot);
                _meshesPreviewRoot = null;
            }
        }

        struct Stats
        {
            public string MeshName;
            public int OriginalVertices;
            public int OriginalTriangles;
            public int HullVertices;
            public int HullTriangles;
            public float VertexReduction;
            public float TriangleReduction;

            public Stats(string meshName, int originalVertices, int originalTriangles, int hullVertices, int hullTriangles, float vertexReduction, float triangleReduction)
            {
                MeshName = meshName;
                OriginalVertices = originalVertices;
                OriginalTriangles = originalTriangles;
                HullVertices = hullVertices;
                HullTriangles = hullTriangles;
                VertexReduction = vertexReduction;
                TriangleReduction = triangleReduction;
            }
        }
    }
}
