// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Geometries
{
    public interface ISerializableMesh
    {
        List<Vector3> Vertices { get; }
        List<SerializableSubMesh> SubMeshes { get; }

        void SetVertices(List<Vector3> vertices);
        void SetVertices(IEnumerable<Vector3> vertices);

        void SetSubMeshes(SerializableSubMesh subMesh);
        void SetSubMeshes(List<SerializableSubMesh> subMeshes);
        void SetSubMeshes(IEnumerable<SerializableSubMesh> subMeshes);
    }
}
