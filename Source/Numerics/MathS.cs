// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Numerics
{
    public static class MathS
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointInTriangle(ScreenV t0, ScreenV t1, ScreenV t2, ScreenV p)
        {
            float w1 = ScreenV.Cross(t0, t1, p);
            float w2 = ScreenV.Cross(t1, t2, p);
            float w3 = ScreenV.Cross(t2, t0, p);

            bool hasNegative = (w1 < -Epsilon) || (w2 < -Epsilon) || (w3 < -Epsilon);
            bool hasPositive = (w1 > Epsilon) || (w2 > Epsilon) || (w3 > Epsilon);

            return !(hasNegative && hasPositive);
        }

        /// <inheritdoc cref="MathEx.GetClosestPointOnLine(UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector3)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV GetClosestPointOnLine(ScreenV lineStart, ScreenV lineEnd, ScreenV point)
        {
            float t = GetClosestPointPositionOnLine(lineStart, lineEnd, point);
            return lineStart + (lineEnd - lineStart) * t;
        }

        /// <inheritdoc cref="MathEx.GetClosestPointPositionOnLine(UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector3)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetClosestPointPositionOnLine(ScreenV lineStart, ScreenV lineEnd, ScreenV point)
        {
            ScreenV lineDirection = lineEnd - lineStart;
            float lineLengthSq = lineDirection.LengthSquared();

            if (lineLengthSq.IsNearlyZero(tolerance: Epsilon))
            {
                return 0f;
            }

            float dotProduct = ScreenV.Dot(point - lineStart, lineDirection) / lineLengthSq;

            return Math.Clamp(dotProduct, 0f, 1f);
        }

        const float Epsilon = 1e-4f;
    }
}
