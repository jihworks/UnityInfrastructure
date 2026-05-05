// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Geometries
{
    [Serializable]
    public class EmbeddableMesh
    {
        [SerializeField] List<Vector3> _vertices = new();
        public List<Vector3> Vertices => _vertices;

        [SerializeField] List<SerializableSubMesh> _subMeshes = new();
        public List<SerializableSubMesh> SubMeshes => _subMeshes;

        public void SetVertices(List<Vector3> vertices)
        {
            _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        }
        public void SetVertices(IEnumerable<Vector3> vertices)
        {
            _vertices.Clear();
            _vertices.AddRange(vertices);
        }

        public void SetSubMeshes(SerializableSubMesh subMesh)
        {
            _subMeshes.Clear();
            _subMeshes.Add(subMesh);
        }
        public void SetSubMeshes(List<SerializableSubMesh> subMeshes)
        {
            _subMeshes = subMeshes ?? throw new ArgumentNullException(nameof(subMeshes));
        }
        public void SetSubMeshes(IEnumerable<SerializableSubMesh> subMeshes)
        {
            _subMeshes.Clear();
            _subMeshes.AddRange(subMeshes);
        }

        public Mesh ToUnityMesh()
        {
            Mesh mesh = new();
            mesh.SetVertices(_vertices);

            for (int i = 0; i < _subMeshes.Count; i++)
            {
                mesh.SetTriangles(_subMeshes[i].Indices, i);
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
