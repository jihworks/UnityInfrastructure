// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Infrastructure.Numerics
{
    public static class MathS
    {
        public static bool IsPointInTriangle(ScreenV t0, ScreenV t1, ScreenV t2, ScreenV p)
        {
            float w1 = ScreenV.Cross(t0, t1, p);
            float w2 = ScreenV.Cross(t1, t2, p);
            float w3 = ScreenV.Cross(t2, t0, p);

            bool hasNegative = (w1 < -Epsilon) || (w2 < -Epsilon) || (w3 < -Epsilon);
            bool hasPositive = (w1 > Epsilon) || (w2 > Epsilon) || (w3 > Epsilon);

            return !(hasNegative && hasPositive);
        }

        const float Epsilon = 1e-4f;
    }
}
