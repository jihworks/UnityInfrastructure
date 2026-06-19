// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Jih.Unity.Infrastructure.Geometries
{
    /// <remarks>
    /// <b>NOT</b> thread-safe.<br/>
    /// <br/>
    /// Currently, focusing on vertex attributes only. The bindposes are not supported.<br/>
    /// <br/>
    /// There is <c>NO</c> coordinate conversion. The class will handle the data as-is.
    /// </remarks>
    public class MeshCollector
    {
        public static MeshCollector Load(Mesh mesh, AdditionalAttributes additionalAttributes, LoadImperfectStrategy imperfectStrategy = LoadImperfectStrategy.RemoveAndUnsetFlag, VertexData fallback = default, int subMeshLod = 0)
        {
            if (!mesh.isReadable)
            {
                throw new ArgumentException($"The mesh '{mesh.name}' is not readable but trying to load.");
            }
            if (subMeshLod < 0 || mesh.lodCount <= subMeshLod)
            {
                throw new ArgumentOutOfRangeException(nameof(subMeshLod));
            }

            int subMeshCount = mesh.subMeshCount;
            if (subMeshCount <= 0)
            {
                throw new ArgumentException($"The mesh '{mesh.name} has not any sub-mesh to load.");
            }

            int vertexCount = mesh.vertexCount;

            AttributeCollection<Vector3> positions = new(vertexCount);
            mesh.GetVertices(positions);

            if (positions.Count < vertexCount)
            {
                switch (imperfectStrategy)
                {
                    case LoadImperfectStrategy.RemoveAndUnsetFlag:
                    case LoadImperfectStrategy.ThrowException:
                        throw new InvalidOperationException($"Mesh '{mesh.name}' has imperfect positions.");

                    case LoadImperfectStrategy.FillWithFallback:
                        positions.AddMultiple(fallback.Position, vertexCount - positions.Count);
                        break;

                    default: throw new NotImplementedException();
                }
            }

            AttributeCollection<Color>? colors = null;
            if (additionalAttributes.Has(AdditionalAttributes.Color))
            {
                colors = new AttributeCollection<Color>(vertexCount);
                mesh.GetColors(colors);

                if (colors.Count < vertexCount)
                {
                    switch (imperfectStrategy)
                    {
                        case LoadImperfectStrategy.RemoveAndUnsetFlag:
                            colors = null;
                            additionalAttributes = additionalAttributes.Unset(AdditionalAttributes.Color);
                            break;

                        case LoadImperfectStrategy.ThrowException:
                            throw new InvalidOperationException($"Mesh '{mesh.name}' has imperfect colors.");

                        case LoadImperfectStrategy.FillWithFallback:
                            colors.AddMultiple(fallback.Color, vertexCount - colors.Count);
                            break;

                        default: throw new NotImplementedException();
                    }
                }
            }

            UVSet?[] uvSets = new UVSet[MaxUVSetCount];
            for (int u = 0; u < MaxUVSetCount; u++)
            {
                AdditionalAttributes uvSetFlag = (AdditionalAttributes)((uint)AdditionalAttributes.UV0 << u);
                if (!additionalAttributes.Has(uvSetFlag))
                {
                    continue;
                }

                UVSet uvSet = new(vertexCount);
                uvSets[u] = uvSet;

                AttributeCollection<Vector4> texCoords = uvSet._texCoords;
                mesh.GetUVs(u, texCoords);

                if (texCoords.Count < vertexCount)
                {
                    switch (imperfectStrategy)
                    {
                        case LoadImperfectStrategy.RemoveAndUnsetFlag:
                            uvSets[u] = null;
                            additionalAttributes = additionalAttributes.Unset(uvSetFlag);
                            break;

                        case LoadImperfectStrategy.ThrowException:
                            throw new InvalidOperationException($"Mesh '{mesh.name}' has imperfect UV channel {u}.");

                        case LoadImperfectStrategy.FillWithFallback:
                            texCoords.AddMultiple(fallback.GetUV(u), vertexCount - texCoords.Count);
                            break;

                        default: throw new NotImplementedException();
                    }
                }
            }

            AttributeCollection<Vector3>? normals = null;
            if (additionalAttributes.Has(AdditionalAttributes.Normal))
            {
                normals = new AttributeCollection<Vector3>(vertexCount);
                mesh.GetNormals(normals);

                if (normals.Count < vertexCount)
                {
                    switch (imperfectStrategy)
                    {
                        case LoadImperfectStrategy.RemoveAndUnsetFlag:
                            normals = null;
                            additionalAttributes = additionalAttributes.Unset(AdditionalAttributes.Normal);
                            break;

                        case LoadImperfectStrategy.ThrowException:
                            throw new InvalidOperationException($"Mesh '{mesh.name}' has imperfect normals.");

                        case LoadImperfectStrategy.FillWithFallback:
                            normals.AddMultiple(fallback.Normal, vertexCount - normals.Count);
                            break;

                        default: throw new NotImplementedException();
                    }
                }
            }

            AttributeCollection<Vector4>? tangents = null;
            if (additionalAttributes.Has(AdditionalAttributes.Tangent))
            {
                tangents = new AttributeCollection<Vector4>(vertexCount);
                mesh.GetTangents(tangents);

                if (tangents.Count < vertexCount)
                {
                    switch (imperfectStrategy)
                    {
                        case LoadImperfectStrategy.RemoveAndUnsetFlag:
                            tangents = null;
                            additionalAttributes = additionalAttributes.Unset(AdditionalAttributes.Tangent);
                            break;

                        case LoadImperfectStrategy.ThrowException:
                            throw new InvalidOperationException($"Mesh '{mesh.name}' has imperfect tangents.");

                        case LoadImperfectStrategy.FillWithFallback:
                            tangents.AddMultiple(fallback.Tangent, vertexCount - tangents.Count);
                            break;

                        default: throw new NotImplementedException();
                    }
                }
            }

            AttributeCollection<IReadOnlyList<BoneWeight1>>? boneWeights = null;
            if (additionalAttributes.Has(AdditionalAttributes.BoneWeight))
            {
                boneWeights = new AttributeCollection<IReadOnlyList<BoneWeight1>>(vertexCount);

                NativeArray<byte> srcCounts = mesh.GetBonesPerVertex();
                NativeArray<BoneWeight1> srcWeights = mesh.GetAllBoneWeights();

                int srcBoneCount = 0;
                for (int i = 0; i < srcCounts.Length; i++)
                {
                    srcBoneCount += srcCounts[i];
                }

                if (srcCounts.Length < vertexCount || srcWeights.Length < srcBoneCount)
                {
                    switch (imperfectStrategy)
                    {
                        case LoadImperfectStrategy.RemoveAndUnsetFlag:
                            boneWeights = null;
                            additionalAttributes = additionalAttributes.Unset(AdditionalAttributes.BoneWeight);
                            break;

                        case LoadImperfectStrategy.ThrowException:
                            throw new InvalidOperationException($"Mesh '{mesh.name}' has imperfect bone weights.");

                        case LoadImperfectStrategy.FillWithFallback:
                            boneWeights.AddMultiple(fallback.BoneWeightList ?? Array.Empty<BoneWeight1>(), vertexCount - boneWeights.Count);
                            break;

                        default: throw new NotImplementedException();
                    }
                }
                else
                {
                    int w = 0;
                    for (int i = 0; i < vertexCount; i++)
                    {
                        int boneCount = srcCounts[i];

                        List<BoneWeight1> weightList = new(boneCount);
                        boneWeights.Add(weightList);

                        for (int j = 0; j < boneCount; j++)
                        {
                            weightList.Add(srcWeights[w++]);
                        }
                    }
                }
            }

            List<SubMesh> subMeshes = new(subMeshCount);

            for (int s = 0; s < subMeshCount; s++)
            {
                SubMeshDescriptor descriptor = mesh.GetSubMesh(s);
                // Need not to check topology.

                SubMesh subMesh = new(descriptor.indexCount);
                subMeshes.Add(subMesh);

                mesh.GetIndices(subMesh._indices, s, subMeshLod, applyBaseVertex: true);
            }

            return new MeshCollector(additionalAttributes, positions, colors, uvSets, normals, tangents, boneWeights, subMeshes, 0);
        }

        /// <summary>
        /// Flags representing vertex attributes to collect other than positions.
        /// </summary>
        public AdditionalAttributes AdditionalAttributes { get; }

        public int VertexCount => _positions.Count;

        readonly AttributeCollection<Vector3> _positions;
        public IAttributeCollection<Vector3> Positions => _positions;

        readonly AttributeCollection<Color>? _colors;
        public IAttributeCollection<Color>? Colors => _colors;

        readonly UVSet?[] _uvSets = new UVSet?[MaxUVSetCount];
        public IReadOnlyList<UVSet?> UVSets => _uvSets;

        readonly AttributeCollection<Vector3>? _normals;
        public IAttributeCollection<Vector3>? Normals => _normals;

        readonly AttributeCollection<Vector4>? _tangents;
        public IAttributeCollection<Vector4>? Tangents => _tangents;

        readonly AttributeCollection<IReadOnlyList<BoneWeight1>>? _boneWeightLists;
        public IAttributeCollection<IReadOnlyList<BoneWeight1>>? BoneWeightLists => _boneWeightLists;

        readonly List<SubMesh> _subMeshes = new();
        public IReadOnlyList<SubMesh> SubMeshes => _subMeshes;

        public int CurrentSubMeshIndex { get; private set; }
        public SubMesh CurrentSubMesh => _subMeshes[CurrentSubMeshIndex];

        private MeshCollector(AdditionalAttributes additionalAttributes, AttributeCollection<Vector3> positions, AttributeCollection<Color>? colors, UVSet?[] uvSets, AttributeCollection<Vector3>? normals, AttributeCollection<Vector4>? tangents, AttributeCollection<IReadOnlyList<BoneWeight1>>? boneWeightLists, List<SubMesh> subMeshes, int currentSubMeshIndex)
        {
            AdditionalAttributes = additionalAttributes;
            _positions = positions;
            _colors = colors;
            _uvSets = uvSets;
            _normals = normals;
            _tangents = tangents;
            _boneWeightLists = boneWeightLists;
            _subMeshes = subMeshes;
            CurrentSubMeshIndex = currentSubMeshIndex;
        }

        /// <param name="additionalAttributes">Flags representing vertex attributes to collect other than positions.</param>
        /// <param name="subMeshCount">At least 1.</param>
        /// <param name="vertexCapacity">Initial capacity of vertex attribute buffers. The buffers are <c>NOT</c> fixed size.</param>
        /// <param name="indexCapacity">Initial capacity of index buffer of sub-meshes. The buffers are <c>NOT</c> fixed size.</param>
        public MeshCollector(AdditionalAttributes additionalAttributes, int subMeshCount = 1, int vertexCapacity = 3, int indexCapacity = 3)
        {
            if (vertexCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCapacity), "Vertex capacity cannot be negative.");
            }
            if (indexCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCapacity), "Index capacity cannot be negative.");
            }
            if (subMeshCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(subMeshCount), "Sub mesh count cannot be less than 1.");
            }

            AdditionalAttributes = additionalAttributes;

            _positions = new AttributeCollection<Vector3>(vertexCapacity);
            if (additionalAttributes.Has(AdditionalAttributes.Color))
            {
                _colors = new AttributeCollection<Color>(vertexCapacity);
            }
            for (int i = 0; i < MaxUVSetCount; i++)
            {
                AdditionalAttributes checkingFlag = (AdditionalAttributes)((uint)AdditionalAttributes.UV0 << i);

                if (additionalAttributes.Has(checkingFlag))
                {
                    _uvSets[i] = new UVSet(vertexCapacity);
                }
            }
            if (additionalAttributes.Has(AdditionalAttributes.Normal))
            {
                _normals = new AttributeCollection<Vector3>(vertexCapacity);
            }
            if (additionalAttributes.Has(AdditionalAttributes.Tangent))
            {
                _tangents = new AttributeCollection<Vector4>(vertexCapacity);
            }
            if (additionalAttributes.Has(AdditionalAttributes.BoneWeight))
            {
                _boneWeightLists = new AttributeCollection<IReadOnlyList<BoneWeight1>>(vertexCapacity);
            }

            for (int i = 0; i < subMeshCount; i++)
            {
                _subMeshes.Add(new SubMesh(indexCapacity));
            }

            CurrentSubMeshIndex = 0;
        }

        public void SetCurrentSubMeshIndex(int index)
        {
            if (index < 0 || _subMeshes.Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            CurrentSubMeshIndex = index;
        }

        public void AddSubMesh(int indexCapacity = 3, bool makeCurrent = true)
        {
            if (indexCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCapacity), "Index capacity cannot be negative.");
            }

            _subMeshes.Add(new SubMesh(indexCapacity));

            if (makeCurrent)
            {
                CurrentSubMeshIndex = _subMeshes.Count - 1;
            }
        }
        public void RemoveSubMesh(int index)
        {
            if (index < 0 || _subMeshes.Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            _subMeshes.RemoveAt(index);
        }

        public void Append(MeshCollector other, AppendSubMeshStrategy subMeshStrategy = AppendSubMeshStrategy.MatchedOnly, VertexData fallback = default)
        {
            if (other == this)
            {
                throw new InvalidOperationException("Cannot append itself.");
            }

            int otherVertexCount = other._positions.Count;

            int baseIndex = _positions.Count;
            _positions.AddAll(other._positions);

            if (_colors is not null)
            {
                if (other._colors is not null)
                {
                    _colors.AddAll(other._colors);
                }
                else
                {
                    _colors.AddMultiple(fallback.Color, otherVertexCount);
                }
            }

            for (int u = 0; u < MaxUVSetCount; u++)
            {
                UVSet? destUVSet = _uvSets[u];
                if (destUVSet is null)
                {
                    continue;
                }

                List<Vector4> destTexCoords = destUVSet._texCoords;

                UVSet? srcUVSet = other._uvSets[u];
                if (srcUVSet is not null)
                {
                    destTexCoords.AddAll(srcUVSet._texCoords);
                }
                else
                {
                    destTexCoords.AddMultiple(fallback.GetUV(u), otherVertexCount);
                }
            }

            if (_normals is not null)
            {
                if (other._normals is not null)
                {
                    _normals.AddAll(other._normals);
                }
                else
                {
                    _normals.AddMultiple(fallback.Normal, otherVertexCount);
                }
            }

            if (_tangents is not null)
            {
                if (other._tangents is not null)
                {
                    _tangents.AddAll(other._tangents);
                }
                else
                {
                    _tangents.AddMultiple(fallback.Tangent, otherVertexCount);
                }
            }

            if (_boneWeightLists is not null)
            {
                if (other._boneWeightLists is not null)
                {
                    _boneWeightLists.AddAll(other._boneWeightLists);
                }
                else
                {
                    _boneWeightLists.AddMultiple(fallback.BoneWeightList ?? Array.Empty<BoneWeight1>(), otherVertexCount);
                }
            }

            if (subMeshStrategy is AppendSubMeshStrategy.AddSubMeshes)
            {
                foreach (var srcSubMesh in other.SubMeshes)
                {
                    List<int> srcIndices = srcSubMesh._indices;

                    SubMesh destSubMesh = new(srcIndices.Count);
                    _subMeshes.Add(destSubMesh);

                    List<int> destIndices = destSubMesh._indices;
                    for (int x = 0; x < srcIndices.Count; x++)
                    {
                        destIndices.Add(baseIndex + srcIndices[x]);
                    }
                }
            }
            else
            {
                for (int s = 0; s < _subMeshes.Count; s++)
                {
                    SubMesh destSubMesh = _subMeshes[s];

                    if (s < other._subMeshes.Count)
                    {
                        SubMesh srcSubMesh = other._subMeshes[s];
                        List<int> srcIndices = srcSubMesh._indices;

                        List<int> destIndices = destSubMesh._indices;
                        destIndices.SecureCapacity(destIndices.Count + srcIndices.Count);

                        for (int x = 0; x < srcIndices.Count; x++)
                        {
                            destIndices.Add(baseIndex + srcIndices[x]);
                        }
                        continue;
                    }

                    if (subMeshStrategy is AppendSubMeshStrategy.MatchedOnly)
                    {
                        break;
                    }
                    else if (subMeshStrategy is AppendSubMeshStrategy.ThrowException)
                    {
                        throw new InvalidOperationException($"Source mesh missing submesh {s}.");
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        public void Append<TVertexData>(IReadOnlyList<TVertexData> vertices, IReadOnlyList<int> indices) where TVertexData : struct, IVertexData
        {
            int baseIndex = _positions.Count;

            for (int i = 0; i < vertices.Count; i++)
            {
                AddVertex(vertices[i]);
            }

            SubMesh destSubMesh = CurrentSubMesh;

            List<int> destIndices = destSubMesh._indices;
            destIndices.SecureCapacity(destIndices.Count + indices.Count);

            for (int x = 0; x < indices.Count; x++)
            {
                int index = indices[x];
                if (index < 0 || vertices.Count <= index)
                {
                    throw new ArgumentOutOfRangeException(nameof(indices));
                }

                destIndices.Add(baseIndex + index);
            }
        }

        public void Append(ISerializableMesh other, AppendSubMeshStrategy subMeshStrategy = AppendSubMeshStrategy.MatchedOnly, VertexData fallback = default)
        {
            int otherVertexCount = other.Vertices.Count;

            int baseIndex = _positions.Count;
            _positions.AddAll(other.Vertices);

            _colors?.AddMultiple(fallback.Color, otherVertexCount);

            for (int u = 0; u < MaxUVSetCount; u++)
            {
                UVSet? destUVSet = _uvSets[u];
                if (destUVSet is null)
                {
                    continue;
                }

                List<Vector4> destTexCoords = destUVSet._texCoords;
                destTexCoords.AddMultiple(fallback.GetUV(u), otherVertexCount);
            }

            _normals?.AddMultiple(fallback.Normal, otherVertexCount);

            _tangents?.AddMultiple(fallback.Tangent, otherVertexCount);

            _boneWeightLists?.AddMultiple(fallback.BoneWeightList ?? Array.Empty<BoneWeight1>(), otherVertexCount);

            if (subMeshStrategy is AppendSubMeshStrategy.AddSubMeshes)
            {
                foreach (var srcSubMesh in other.SubMeshes)
                {
                    List<int> srcIndices = srcSubMesh.Indices;

                    SubMesh destSubMesh = new(srcIndices.Count);
                    _subMeshes.Add(destSubMesh);

                    List<int> destIndices = destSubMesh._indices;
                    for (int x = 0; x < srcIndices.Count; x++)
                    {
                        destIndices.Add(baseIndex + srcIndices[x]);
                    }
                }
            }
            else
            {
                for (int s = 0; s < _subMeshes.Count; s++)
                {
                    SubMesh destSubMesh = _subMeshes[s];

                    if (s < other.SubMeshes.Count)
                    {
                        SerializableSubMesh srcSubMesh = other.SubMeshes[s];
                        List<int> srcIndices = srcSubMesh.Indices;

                        List<int> destIndices = destSubMesh._indices;
                        destIndices.SecureCapacity(destIndices.Count + srcIndices.Count);

                        for (int x = 0; x < srcIndices.Count; x++)
                        {
                            destIndices.Add(baseIndex + srcIndices[x]);
                        }
                        continue;
                    }

                    if (subMeshStrategy is AppendSubMeshStrategy.MatchedOnly)
                    {
                        break;
                    }
                    else if (subMeshStrategy is AppendSubMeshStrategy.ThrowException)
                    {
                        throw new InvalidOperationException($"Source mesh missing submesh {s}.");
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        /// <summary>
        /// Append a CW triangle.
        /// </summary>
        /// <remarks>
        /// <code>
        /// 0 - 1
        ///  \ /
        ///   2
        /// </code>
        /// </remarks>
        public RangeInt AppendTriangle<TVertexData>(in TVertexData v0, in TVertexData v1, in TVertexData v2) where TVertexData : struct, IVertexData
        {
            int baseIndex = _positions.Count;
            AddVertex(v0);
            AddVertex(v1);
            AddVertex(v2);

            SubMesh subMesh = CurrentSubMesh;
            List<int> indices = subMesh._indices;
            indices.Add(baseIndex);
            indices.Add(baseIndex + 1);
            indices.Add(baseIndex + 2);

            return new RangeInt(baseIndex, 3);
        }

        /// <summary>
        /// Append two CW triangles for a quad.
        /// </summary>
        /// <remarks>
        /// <code>
        /// 0 - 2
        /// | / |
        /// 1 - 3
        /// </code>
        /// </remarks>
        public RangeInt AppendQuad<TVertexData>(in TVertexData v0, in TVertexData v1, in TVertexData v2, in TVertexData v3) where TVertexData : struct, IVertexData
        {
            int baseIndex = _positions.Count;
            AddVertex(v0);
            AddVertex(v1);
            AddVertex(v2);
            AddVertex(v3);

            SubMesh subMesh = CurrentSubMesh;
            List<int> indices = subMesh._indices;
            indices.Add(baseIndex);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 1);
            indices.Add(baseIndex + 1);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 3);

            return new RangeInt(baseIndex, 4);
        }

        /// <summary>
        /// Append CW triangles for a fan.
        /// </summary>
        /// <remarks>
        /// Example with 4-vertices.<br/>
        /// <c>c</c> means center vertex and <c>1~4</c> means CW vertices on the fan.
        /// <code>
        ///   2 - 3
        ///  / \ / \
        /// 1 - c - 4
        /// </code>
        /// </remarks>
        public RangeInt AppendFan<TVertexData>(in TVertexData center, ReadOnlySpan<TVertexData> cwVertices) where TVertexData : struct, IVertexData
        {
            return AppendNGon_Impl(center, cwVertices, addLast: false, "Fan");
        }

        /// <summary>
        /// Append CW triangles for a N-gon.
        /// </summary>
        /// <remarks>
        /// Example for a hexagon.<br/>
        /// <c>c</c> means center vertex and <c>1~6</c> means CW vertices on the N-gon.
        /// <code>
        ///   1 - 2
        ///  / \ / \
        /// 6 - c - 3
        ///  \ / \ /
        ///   5 - 4
        /// </code>
        /// </remarks>
        public RangeInt AppendNGon<TVertexData>(in TVertexData center, ReadOnlySpan<TVertexData> cwVertices) where TVertexData : struct, IVertexData
        {
            return AppendNGon_Impl(center, cwVertices, addLast: true, "N-gon");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        RangeInt AppendNGon_Impl<TVertexData>(in TVertexData center, ReadOnlySpan<TVertexData> cwVertices, bool addLast, string context) where TVertexData : struct, IVertexData
        {
            int centerIndex = _positions.Count;
            AddVertex(center);

            int baseIndex = _positions.Count;
            int count = 0;
            foreach (var v in cwVertices)
            {
                AddVertex(v);
                count++;
            }

            if (count < 2)
            {
                throw new ArgumentException($"{context}'s vertices count must be at least 2. Got {count}.", nameof(cwVertices));
            }

            SubMesh subMesh = CurrentSubMesh;
            List<int> indices = subMesh._indices;
            for (int i = 0; i < count - 1; i++)
            {
                indices.Add(centerIndex);
                indices.Add(baseIndex + i);
                indices.Add(baseIndex + i + 1);
            }
            if (addLast)
            {
                indices.Add(centerIndex);
                indices.Add(baseIndex + count - 1);
                indices.Add(baseIndex);
            }

            return new RangeInt(centerIndex, count + 1);
        }

        /// <param name="recalculateNormals">If normals are collected, this flag has no effect.</param>
        /// <param name="recalculateTangents">If tangents are collected, this flag has no effect.</param>
        /// <remarks>
        /// This method does not close the returned <see cref="Mesh"/>. It means the <see cref="Mesh.isReadable"/> is <c>true</c>.<br/>
        /// The caller may need to call <see cref="Mesh.UploadMeshData(bool)"/> with <c>true</c> to close the <see cref="Mesh"/>.
        /// </remarks>
        public Mesh ToTrianglesMesh(bool recalculateNormals = false, bool recalculateTangents = false, bool force32BitIndices = false)
        {
            Mesh mesh = new();
            ToTrianglesMesh(mesh, recalculateNormals, recalculateTangents, force32BitIndices);
            return mesh;
        }
        /// <inheritdoc cref="ToTrianglesMesh(bool, bool, bool)"/>
        public void ToTrianglesMesh(Mesh mesh, bool recalculateNormals = false, bool recalculateTangents = false, bool force32BitIndices = false)
        {
            mesh.Clear(keepVertexLayout: true);

            IndexFormat indexFormat;
            if (force32BitIndices)
            {
                indexFormat = IndexFormat.UInt32;
            }
            else
            {
                indexFormat = _positions.Count > ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            }

            mesh.indexFormat = indexFormat;
            mesh.subMeshCount = _subMeshes.Count;

            mesh.SetVertices(_positions, 0, _positions.Count, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

            for (int i = 0; i < _subMeshes.Count; i++)
            {
                mesh.SetIndices(_subMeshes[i]._indices, MeshTopology.Triangles, i, calculateBounds: false);
            }

            mesh.RecalculateBounds();

            if (_colors is not null)
            {
                mesh.SetColors(_colors, 0, _colors.Count, MeshUpdateFlags.DontValidateIndices);
            }

            for (int i = 0; i < MaxUVSetCount; i++)
            {
                UVSet? uv = _uvSets[i];
                if (uv is null)
                {
                    continue;
                }

                mesh.SetUVs(i, uv._texCoords, 0, uv._texCoords.Count, MeshUpdateFlags.DontValidateIndices);
            }

            if (_normals is not null)
            {
                mesh.SetNormals(_normals, 0, _normals.Count, MeshUpdateFlags.DontValidateIndices);
            }
            else if (recalculateNormals)
            {
                mesh.RecalculateNormals();
            }

            if (_tangents is not null)
            {
                mesh.SetTangents(_tangents, 0, _tangents.Count, MeshUpdateFlags.DontValidateIndices);
            }
            else if (recalculateTangents)
            {
                mesh.RecalculateTangents();
            }

            if (_boneWeightLists is not null)
            {
                int totalWeightCount = 0;
                for (int i = 0; i < _boneWeightLists.Count; i++)
                {
                    totalWeightCount += _boneWeightLists[i].Count;
                }

                NativeArray<byte> counts = new(_boneWeightLists.Count, Allocator.Temp);
                try
                {
                    NativeArray<BoneWeight1> weights = new(totalWeightCount, Allocator.Temp);
                    try
                    {
                        int w = 0;
                        for (int i = 0; i < _boneWeightLists.Count; i++)
                        {
                            IReadOnlyList<BoneWeight1> list = _boneWeightLists[i];

                            if (list.Count > byte.MaxValue)
                            {
                                throw new InvalidOperationException("Bone weight count for a vertex cannot greater than 255(byte.MaxValue). It is Unity's limitation.");
                            }
                            counts[i] = (byte)list.Count;

                            for (int j = 0; j < list.Count; j++)
                            {
                                weights[w++] = list[j];
                            }
                        }

                        mesh.SetBoneWeights(counts, weights);
                    }
                    finally
                    {
                        weights.Dispose();
                    }
                }
                finally
                {
                    counts.Dispose();
                }
            }
        }

        public void Clear(bool keepSubMeshes = false)
        {
            _positions.Clear();
            _colors?.Clear();
            for (int i = 0; i < MaxUVSetCount; i++)
            {
                _uvSets[i]?._texCoords.Clear();
            }
            _normals?.Clear();
            _tangents?.Clear();
            _boneWeightLists?.Clear();

            if (!keepSubMeshes)
            {
                _subMeshes.RemoveRange(1, _subMeshes.Count - 1);
            }
            for (int s = 0; s < _subMeshes.Count; s++)
            {
                _subMeshes[s]._indices.Clear();
            }

            CurrentSubMeshIndex = 0;
        }

        public void SecureVertexCapacity(int desiredCapacity)
        {
            _positions.SecureCapacity(desiredCapacity);
            _colors?.SecureCapacity(desiredCapacity);
            for (int i = 0; i < MaxUVSetCount; i++)
            {
                _uvSets[i]?.SecureTexCoordCapacity(desiredCapacity);
            }
            _normals?.SecureCapacity(desiredCapacity);
            _tangents?.SecureCapacity(desiredCapacity);
            _boneWeightLists?.SecureCapacity(desiredCapacity);
        }

        public void SecureIndexCapacity(int desiredCapacity)
        {
            for (int s = 0; s < _subMeshes.Count; s++)
            {
                _subMeshes[s].SecureIndexCapacity(desiredCapacity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddVertex<TVertexData>(in TVertexData vertexData) where TVertexData : struct, IVertexData
        {
            _positions.Add(vertexData.GetPosition());
            _colors?.Add(vertexData.GetColor());
            for (int i = 0; i < MaxUVSetCount; i++)
            {
                _uvSets[i]?._texCoords.Add(vertexData.GetUV(i));
            }
            _normals?.Add(vertexData.GetNormal());
            _tangents?.Add(vertexData.GetTangent());
            _boneWeightLists?.Add(vertexData.GetBoneWeightList() ?? Array.Empty<BoneWeight1>());
        }

        public interface IAttributeCollection<T> : IReadOnlyList<T>
        {
            new T this[int index] { get; set; }
        }

        internal class AttributeCollection<T> : List<T>, IAttributeCollection<T>
        {
            public AttributeCollection(int capacity) : base(capacity)
            {
            }
        }

        public class UVSet
        {
            internal readonly AttributeCollection<Vector4> _texCoords;
            public IAttributeCollection<Vector4> TexCoords => _texCoords;

            internal UVSet(int capacity)
            {
                _texCoords = new AttributeCollection<Vector4>(capacity);
            }

            public void SecureTexCoordCapacity(int desiredCapacity)
            {
                _texCoords.SecureCapacity(desiredCapacity);
            }
        }
        public class SubMesh
        {
            internal readonly List<int> _indices;
            public IReadOnlyList<int> Indices => _indices;

            internal SubMesh(int capacity)
            {
                _indices = new List<int>(capacity);
            }

            public void SecureIndexCapacity(int desiredCapacity)
            {
                _indices.SecureCapacity(desiredCapacity);
            }
        }

        /// <returns>Return <c>false</c> to abort. The <paramref name="result"/> will be ignored when <c>false</c> returned.</returns>
        public delegate bool EditDelegate<T>(int index, T source, out T result);

        public enum LoadImperfectStrategy
        {
            RemoveAndUnsetFlag,
            ThrowException,
            FillWithFallback,
        }

        public enum AppendSubMeshStrategy
        {
            MatchedOnly,
            AddSubMeshes,
            ThrowException,
        }

        public const int MaxUVSetCount = 8;
    }

    [Flags]
    public enum AdditionalAttributes : uint
    {
        None = 0,
        Color = 1 << 1,
        UV0 = 1 << 8,
        UV1 = 1 << 9,
        UV2 = 1 << 10,
        UV3 = 1 << 11,
        UV4 = 1 << 12,
        UV5 = 1 << 13,
        UV6 = 1 << 14,
        UV7 = 1 << 15,
        Normal = 1 << 24,
        Tangent = 1 << 25,
        BoneWeight = 1 << 26,
        All = 0xffffffff,
    }

    public static class AdditionalAttributesEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(this AdditionalAttributes left, AdditionalAttributes right)
        {
            return (left & right) != 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AdditionalAttributes Set(this AdditionalAttributes left, AdditionalAttributes right)
        {
            return left | right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AdditionalAttributes Unset(this AdditionalAttributes left, AdditionalAttributes right)
        {
            return left & (~right);
        }
    }
}
