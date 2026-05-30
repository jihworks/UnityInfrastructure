// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure
{
    public struct ThicknessF
    {
        public float Left, Right, Top, Bottom;

        public readonly float Horizontal => Left + Right;
        public readonly float Vertical => Top + Bottom;

        public ThicknessF(float uniform)
            : this(uniform, uniform, uniform, uniform)
        {
        }
        public ThicknessF(float horizontal, float vertical)
            : this(horizontal, horizontal, vertical, vertical)
        {
        }
        public ThicknessF(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ThicknessF t && Equals(t);
        }
        public readonly bool Equals(ThicknessF other)
        {
            return Left == other.Left &&
                   Right == other.Right &&
                   Top == other.Top &&
                   Bottom == other.Bottom;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Left, Right, Top, Bottom);
        }

        public static bool operator ==(ThicknessF left, ThicknessF right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(ThicknessF left, ThicknessF right)
        {
            return !(left == right);
        }
    }
}
