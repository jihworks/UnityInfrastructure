// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Deterministics;
using System;

namespace Jih.Unity.Infrastructure.HexaGrid
{
    public readonly struct HexaCoordF64 : IEquatable<HexaCoordF64>
    {
        public static HexaCoordF64 Add(in HexaCoordF64 left, in HexaCoordF64 right)
        {
            return new(left.A + right.A, left.B + right.B, left.C + right.C);
        }
        public static HexaCoordF64 Subtract(in HexaCoordF64 left, in HexaCoordF64 right)
        {
            return new(left.A - right.A, left.B - right.B, left.C - right.C);
        }
        public static HexaCoordF64 Multiply(in HexaCoordF64 left, int scale)
        {
            return new(left.A * scale, left.B * scale, left.C * scale);
        }

        public static HexaCoord Round(in HexaCoordF64 coord)
        {
            int a = (int)FPMath.Round(coord.A);
            int b = (int)FPMath.Round(coord.B);
            int c = (int)FPMath.Round(coord.C);
            F64 da = FPMath.Abs(a - coord.A);
            F64 db = FPMath.Abs(b - coord.B);
            F64 dc = FPMath.Abs(c - coord.C);
            if (da > db && da > dc)
            {
                a = -b - c;
            }
            else if (db > dc)
            {
                b = -a - c;
            }
            else
            {
                c = -a - b;
            }
            return new HexaCoord(a, b, c);
        }

        public static F64 Distance(in HexaCoordF64 left, in HexaCoordF64 right)
        {
            return Subtract(left, right).GetLength();
        }

        public static HexaCoordF64 Lerp(in HexaCoordF64 left, in HexaCoordF64 right, F64 alpha)
        {
            return new(FPMath.Lerp(left.A, right.A, alpha), FPMath.Lerp(left.B, right.B, alpha), FPMath.Lerp(left.C, right.C, alpha));
        }

        public readonly F64 A, B, C;

        public HexaCoordF64(F64 a, F64 b, F64 c)
        {
            if (FPMath.Round(a + b + c) != F64.Zero)
            {
                throw new InvalidOperationException("Invalid hexa coordinate.");
            }
            A = a;
            B = b;
            C = c;
        }

        public readonly F64 GetLength()
        {
            return (FPMath.Abs(A) + FPMath.Abs(B) + FPMath.Abs(C)) / 2;
        }

        public readonly override string ToString()
        {
            return $"<{A}, {B}, {C}>";
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is HexaCoordF64 f && Equals(f);
        }

        public readonly bool Equals(HexaCoordF64 other)
        {
            return A == other.A &&
                   B == other.B &&
                   C == other.C;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(A, B, C);
        }

        public static HexaCoordF64 operator +(HexaCoordF64 left, HexaCoordF64 right)
        {
            return Add(left, right);
        }
        public static HexaCoordF64 operator -(HexaCoordF64 left, HexaCoordF64 right)
        {
            return Subtract(left, right);
        }
        public static HexaCoordF64 operator *(HexaCoordF64 left, int scale)
        {
            return Multiply(left, scale);
        }

        /// <summary>
        /// Using round method.
        /// </summary>
        /// <seealso cref="Round(in HexaCoordF64)"/>
        public static explicit operator HexaCoord(HexaCoordF64 hexCoord)
        {
            return Round(hexCoord);
        }

        public static bool operator ==(HexaCoordF64 left, HexaCoordF64 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(HexaCoordF64 left, HexaCoordF64 right)
        {
            return !(left == right);
        }
    }
}
