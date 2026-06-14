// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jih.Unity.Infrastructure.Numerics
{
    /// <seealso cref="ScreenSpaceConvert"/>
    /// <inheritdoc cref="ScreenSpaceConvert"/>
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = sizeof(float))]
    public struct ScreenR : IEquatable<ScreenR>, IFormattable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenR FromMinMax(ScreenV min, ScreenV max)
        {
            return new ScreenR(min, max - min);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenR FromCenterExtents(ScreenV center, ScreenV extents)
        {
            return new ScreenR(center - extents, extents + extents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenR Inflate(ScreenR rect, float x, float y)
        {
            ScreenR r = rect;
            r.Inflate(x, y);
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenR Intersect(ScreenR a, ScreenR b)
        {
            float x = MathF.Max(a.X, b.X);
            float right = MathF.Min(a.Right, b.Right);
            float y = MathF.Max(a.Y, b.Y);
            float bottom = MathF.Min(a.Bottom, b.Bottom);

            if (right >= x && bottom >= y)
            {
                return new ScreenR(x, y, right - x, bottom - y);
            }
            return Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenR Union(ScreenR a, ScreenR b)
        {
            if (a.IsEmpty || b.IsEmpty)
            {
                return Empty;
            }

            float x = MathF.Min(a.X, b.X);
            float right = MathF.Max(a.Right, b.Right);
            float y = MathF.Min(a.Y, b.Y);
            float bottom = MathF.Max(a.Bottom, b.Bottom);

            return new ScreenR(x, y, right - x, bottom - y);
        }

        public static ScreenR Empty => new(0f, 0f, 0f, 0f);

        [UnityEngine.SerializeField] public float X;
        [UnityEngine.SerializeField] public float Y;
        [UnityEngine.SerializeField] public float Width;
        [UnityEngine.SerializeField] public float Height;

        public ScreenV Location
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(X, Y);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public ScreenV Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(Width, Height);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public float Left
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => X;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => X = value;
        }
        public float Top
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => Y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Y = value;
        }
        public float Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => X + Width;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Width = value - X;
        }
        public float Bottom
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => Y + Height;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Height = value - Y;
        }

        public ScreenV Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(X + Width * 0.5f, Y + Height * 0.5f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X = value.X - Width * 0.5f;
                Y = value.Y - Height * 0.5f;
            }
        }
        public ScreenV Extents
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(Width * 0.5f, Height * 0.5f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X -= value.X - Width * 0.5f;
                Y -= value.Y - Height * 0.5f;
                Width = value.X + value.X;
                Height = value.Y + value.Y;
            }
        }

        public ScreenV Min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(X, Y);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public ScreenV Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(Right, Bottom);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        public readonly bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Width <= 0f || Height <= 0f;
        }

        public ScreenR(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public ScreenR(ScreenV location, ScreenV size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.X;
            Height = size.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMinMax(ScreenV min, ScreenV max)
        {
            this = FromMinMax(min, max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCenterExtents(ScreenV center, ScreenV extents)
        {
            this = FromCenterExtents(center, extents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(ScreenV point)
        {
            return Contains(point.X, point.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(float x, float y)
        {
            return X <= x && x < X + Width &&
                   Y <= y && y < Y + Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(ScreenR rect)
        {
            return X <= rect.X && rect.Right <= Right &&
                   Y <= rect.Y && rect.Bottom <= Bottom;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IntersectsWith(ScreenR rect)
        {
            return rect.X < Right && X < rect.Right &&
                   rect.Y < Bottom && Y < rect.Bottom;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Intersect(ScreenR rect)
        {
            this = Intersect(this, rect);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Offset(ScreenV pos)
        {
            Offset(pos.X, pos.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inflate(ScreenV size)
        {
            Inflate(size.X, size.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inflate(float width, float height)
        {
            X -= width;
            Y -= height;
            Width += 2f * width;
            Height += 2f * height;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ScreenR other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(ScreenR other)
        {
            return X == other.X && Y == other.Y &&
                   Width == other.Width && Height == other.Height;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }

        public readonly override string ToString()
        {
            return ToString(null, null);
        }
        public readonly string ToString(string? format)
        {
            return ToString(format, null);
        }
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F6";
            }

            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return $"[X={X.ToString(format, formatProvider)}, Y={Y.ToString(format, formatProvider)}, " +
                   $"Width={Width.ToString(format, formatProvider)}, Height={Height.ToString(format, formatProvider)}]";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ScreenR left, ScreenR right)
        {
            return left.Equals(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ScreenR left, ScreenR right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScreenR(UnityEngine.Rect r)
        {
            return new ScreenR(r.x, r.y, r.width, r.height);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UnityEngine.Rect(ScreenR r)
        {
            return new UnityEngine.Rect(r.X, r.Y, r.Width, r.Height);
        }
    }
}
