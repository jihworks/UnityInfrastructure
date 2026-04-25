// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

using Jih.Unity.Infrastructure.Deterministics;

namespace Jih.Unity.Infrastructure.HexaGrid
{
    public readonly struct HexaOrientationF64
    {
        public readonly Vector2F64 Origin, Radius;

        readonly F64 F0, F1, F2, F3;
        readonly F64 B0, B1, B2, B3;

        /// <summary>
        /// Horizontal distance from center of a cell to another center of a cell.
        /// </summary>
        public F64 ScreenHorizontalSpacing => Radius.X * F0;
        /// <summary>
        /// Vertical distance from center of a cell to another center of a cell.
        /// </summary>
        public F64 ScreenVerticalSpacing => Radius.Y * F3;

        /// <param name="origin">A coordinate for center of the origin cell in screen space. Commonly <see cref="Vector2F64.zero"/>.</param>
        /// <param name="radius">Radius of circumcircle of a cell in screen space. Commonly <see cref="Vector2F64.one"/>.</param>
        public HexaOrientationF64(Vector2F64 origin, Vector2F64 radius)
            : this(origin, radius,
                  FPMath.Sqrt(3), FPMath.Sqrt(3) / 2, F64.Zero, 3 / (F64)2,
                  FPMath.Sqrt(3) / 3, -1 / (F64)3, F64.Zero, 2 / (F64)3)
        {
        }
        private HexaOrientationF64(Vector2F64 origin, Vector2F64 radius, F64 f0, F64 f1, F64 f2, F64 f3, F64 b0, F64 b1, F64 b2, F64 b3)
        {
            Origin = origin;
            Radius = radius;
            F0 = f0;
            F1 = f1;
            F2 = f2;
            F3 = f3;
            B0 = b0;
            B1 = b1;
            B2 = b2;
            B3 = b3;
        }

        public Vector2F64 HexaToScreen(HexaCoordF64 h)
        {
            F64 x = (F0 * h.A + F1 * h.B) * Radius.X;
            F64 y = (F2 * h.A + F3 * h.B) * Radius.Y;
            return new Vector2F64(x + Origin.X, y + Origin.Y);
        }

        public HexaCoordF64 ScreenToHexa(Vector2F64 p)
        {
            Vector2F64 pt = new((p.X - Origin.X) / Radius.X, (p.Y - Origin.Y) / Radius.Y);
            F64 a = B0 * pt.X + B1 * pt.Y;
            F64 b = B2 * pt.X + B3 * pt.Y;
            return new HexaCoordF64(a, b, -a - b);
        }
    }
}
