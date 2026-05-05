// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Geometries;
using System.Collections.Generic;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Components
{
    public abstract class BaseConvexHullComponent : MonoBehaviour
    {
        [SerializeField] protected EmbeddableMesh? _convexHull;

        public bool HasValidConvexHull()
        {
            if (_convexHull is null)
            {
                return false;
            }
            if (_convexHull.SubMeshes.Count <= 0)
            {
                return false;
            }
            if (_convexHull.SubMeshes[0].Indices.Count < 3)
            {
                return false;
            }
            return true;
        }

        protected virtual void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            DebugDraw();
#endif
        }

#if UNITY_EDITOR
        void DebugDraw()
        {
            if (_convexHull is null)
            {
                return;
            }

            List<Vector3> vertices = _convexHull.Vertices;

            Matrix4x4 m = transform.localToWorldMatrix;

            Gizmos.color = Color.green;

            for (int s = 0; s < _convexHull.SubMeshes.Count; s++)
            {
                SerializableSubMesh subMesh = _convexHull.SubMeshes[s];
                List<int> indices = subMesh.Indices;

                for (int i = 0; i < indices.Count; i += 3)
                {
                    int i0 = indices[i];
                    int i1 = indices[i + 1];
                    int i2 = indices[i + 2];

                    Vector3 v0 = m.MultiplyPoint3x4(vertices[i0]);
                    Vector3 v1 = m.MultiplyPoint3x4(vertices[i1]);
                    Vector3 v2 = m.MultiplyPoint3x4(vertices[i2]);

                    Gizmos.DrawLine(v0, v1);
                    Gizmos.DrawLine(v1, v2);
                    Gizmos.DrawLine(v2, v0);
                }
            }
        }

        public EmbeddableMesh? Editor_GetConvexHull()
        {
            return _convexHull;
        }
        public void Editor_SetConvexHull(EmbeddableMesh? convexHull)
        {
            _convexHull = convexHull;
        }

        public static EmbeddableMesh Editor_Build(GameObject root, GenerateHullDelegate generateHull)
        {
            static void Populate(List<Vector3> dest, in Matrix4x4 parentToRootMatrix, Transform parent)
            {
                MeshFilter[] meshFilters = parent.gameObject.GetComponents<MeshFilter>();
                foreach (var meshFilter in meshFilters)
                {
                    if (meshFilter.sharedMesh == null)
                    {
                        continue;
                    }

                    Vector3[] vertices = meshFilter.sharedMesh.vertices;
                    foreach (var vertex in vertices)
                    {
                        dest.Add(parentToRootMatrix.MultiplyPoint3x4(vertex));
                    }
                }

                int childCount = parent.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = parent.GetChild(i);

                    Matrix4x4 localMatrix = Matrix4x4.TRS(
                        child.localPosition,
                        child.localRotation,
                        child.localScale
                    );

                    Matrix4x4 currentToRootMatrix = parentToRootMatrix * localMatrix;
                    Populate(dest, currentToRootMatrix, child);
                }
            }

            List<Vector3> allVertices = new();
            Populate(allVertices, Matrix4x4.identity, root.transform);

            if (allVertices.Count < 4)
            {
                return new EmbeddableMesh();
            }

            generateHull(allVertices.ToArray(), out List<Vector3> hullVertices, out List<int> hullTriangles);

            EmbeddableMesh serializableMesh = new();
            serializableMesh.SetVertices(hullVertices);
            SerializableSubMesh subMesh = new();
            subMesh.SetIndices(hullTriangles);
            serializableMesh.SetSubMeshes(subMesh);

            return serializableMesh;
        }

        public delegate void GenerateHullDelegate(Vector3[] points, out List<Vector3> hullVertices, out List<int> hullTriangles);
#endif
    }
}
