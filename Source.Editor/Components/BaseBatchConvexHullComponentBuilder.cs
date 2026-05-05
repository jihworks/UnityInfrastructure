// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Components;
using Jih.Unity.Infrastructure.Geometries;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Editor.Components
{
    public abstract class BaseBatchConvexHullComponentBuilder
    {
        public void Execute()
        {
            Undo.SetCurrentGroupName("Batch ConvexHullComponent Builder");
            int undoGroup = Undo.GetCurrentGroup();
            try
            {
                int sceneCount = 0, prefabCount = 0;
                {
                    BaseConvexHullComponent[] components = Object.FindObjectsByType<BaseConvexHullComponent>(FindObjectsSortMode.None);

                    for (int i = 0; i < components.Length; i++)
                    {
                        BaseConvexHullComponent component = components[i];

                        EditorUtility.DisplayProgressBar("Processing...", component.gameObject.name, (float)i / components.Length);

                        Undo.RecordObject(component, "Build ConvexHull");

                        EmbeddableMesh convexHull = BaseConvexHullComponent.Editor_Build(component.gameObject, GenerateHull);
                        component.Editor_SetConvexHull(convexHull);
                        sceneCount++;

                        EditorUtility.SetDirty(component);
                    }

                    if (components.Length > 0)
                    {
                        EditorSceneManager.MarkAllScenesDirty();
                    }
                }
                {
                    string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

                    for (int i = 0; i < prefabGuids.Length; i++)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);

                        EditorUtility.DisplayProgressBar("Processing...", assetPath, (float)i / prefabGuids.Length);

                        GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                        if (prefabRoot == null) continue;

                        BaseConvexHullComponent[] components = prefabRoot.GetComponentsInChildren<BaseConvexHullComponent>(true);
                        bool isModified = false;

                        foreach (var component in components)
                        {
                            Undo.RecordObject(component, "Build ConvexHull");

                            EmbeddableMesh convexHull = BaseConvexHullComponent.Editor_Build(component.gameObject, GenerateHull);
                            component.Editor_SetConvexHull(convexHull);
                            prefabCount++;

                            EditorUtility.SetDirty(component);
                            isModified = true;
                        }

                        if (isModified)
                        {
                            PrefabUtility.SavePrefabAsset(prefabRoot);
                        }
                    }
                }

                AssetDatabase.SaveAssets();

                Debug.Log($"Built: Scene {sceneCount}, Prefab {prefabCount}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Undo.CollapseUndoOperations(undoGroup);
            }
        }

        protected abstract void GenerateHull(Vector3[] points, out List<Vector3> hullVertices, out List<int> hullTriangles);
    }
}
