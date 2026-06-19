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
    public interface IVertexData
    {
        Vector3 GetPosition();
        Color GetColor();
        Vector4 GetUV(int i);
        Vector3 GetNormal();
        Vector4 GetTangent();
        IReadOnlyList<BoneWeight1>? GetBoneWeightList();
    }

    public struct VertexData : IVertexData
    {
        public Vector3 Position;
        public Color Color;
        public Vector4 UV0, UV1, UV2, UV3, UV4, UV5, UV6, UV7;
        public Vector3 Normal;
        public Vector4 Tangent;
        public IReadOnlyList<BoneWeight1>? BoneWeightList;

        public VertexData(Vector3 position) : this()
        {
            Position = position;
        }
        public VertexData(Vector3 position, Color color) : this(position)
        {
            Color = color;
        }
        public VertexData(Vector3 position, Vector4 uv0) : this(position)
        {
            UV0 = uv0;
        }
        public VertexData(Vector3 position, Color color, Vector4 uv0) : this(position, color)
        {
            UV0 = uv0;
        }
        public VertexData(Vector3 position, Vector4 uv0, Vector3 normal) : this()
        {
            Position = position;
            UV0 = uv0;
            Normal = normal;
        }
        public VertexData(Vector3 position, Color color, Vector4 uv0, Vector3 normal) : this()
        {
            Position = position;
            Color = color;
            UV0 = uv0;
            Normal = normal;
        }

        public readonly Vector4 GetUV(int i)
        {
            return i switch
            {
                0 => UV0,
                1 => UV1,
                2 => UV2,
                3 => UV3,
                4 => UV4,
                5 => UV5,
                6 => UV6,
                7 => UV7,
                _ => throw new ArgumentOutOfRangeException(nameof(i)),
            };
        }
        public void SetUV(int i, Vector4 value)
        {
            switch (i)
            {
                case 0: UV0 = value; break;
                case 1: UV1 = value; break;
                case 2: UV2 = value; break;
                case 3: UV3 = value; break;
                case 4: UV4 = value; break;
                case 5: UV5 = value; break;
                case 6: UV6 = value; break;
                case 7: UV7 = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(i));
            }
        }

        readonly Vector3 IVertexData.GetPosition()
        {
            return Position;
        }
        readonly Color IVertexData.GetColor()
        {
            return Color;
        }
        readonly Vector4 IVertexData.GetUV(int i)
        {
            if (MaxUVSetCount <= i)
            {
                return default;
            }
            return GetUV(i);
        }
        readonly Vector3 IVertexData.GetNormal()
        {
            return Normal;
        }
        readonly Vector4 IVertexData.GetTangent()
        {
            return Tangent;
        }
        readonly IReadOnlyList<BoneWeight1>? IVertexData.GetBoneWeightList()
        {
            return BoneWeightList;
        }

        public const int MaxUVSetCount = 8;
    }
}
