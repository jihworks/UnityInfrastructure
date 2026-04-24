// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Deterministics
{
    /// <summary>
    /// Deterministic Perlin Noise generator using F32 (Q16.16).
    /// Safe to use across all platforms and CPU architectures.
    /// </summary>
    public static class FPNoise
    {
        /// <summary>
        /// 2D Perlin Noise
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>Value approximately in [0, 1] range.</returns>
        public static F32 PerlinNoise(F32 x, F32 y)
        {
            // Upper 16 bits. Integer part.
            int X = ((int)x >> F32.FractionalBits) & 255;
            int Y = ((int)y >> F32.FractionalBits) & 255;

            // Lower 16 bits. Fractional part.
            F32 xf = F32.FromRaw(x.RawValue & F32.FractionMask);
            F32 yf = F32.FromRaw(y.RawValue & F32.FractionMask);

            F32 u = Fade(xf);
            F32 v = Fade(yf);

            int aa = P[P[X] + Y];
            int ab = P[P[X] + Y + 1];
            int ba = P[P[X + 1] + Y];
            int bb = P[P[X + 1] + Y + 1];

            F32 res = FPMath.Lerp(
                FPMath.Lerp(Grad(aa, xf, yf), Grad(ba, xf - F32.One, yf), u),
                FPMath.Lerp(Grad(ab, xf, yf - F32.One), Grad(bb, xf - F32.One, yf - F32.One), u),
                v
            );

            // The res in approx. [-0.707, 0.707] range.
            // Scaling up to [0, 1] range.
            F32 finalValue = (res + F32.One) * F32.Half;
            return finalValue.Clamp01();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static F32 Fade(F32 t)
        {
            // 6t^5 - 15t^4 + 10t^3 
            // = t * t * t * (t * (t * 6 - 15) + 10)
            return t * t * t * (t * (t * F32_6 - F32_15) + F32_10);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static F32 Grad(int hash, F32 x, F32 y)
        {
            int h = hash & 3;
            F32 u = (h & 2) == 0 ? x : -x;
            F32 v = (h & 1) == 0 ? y : -y;
            return u + v;
        }

        // Ken Perlin's origianal hash table. (512 length)
        static readonly int[] P = new int[512]
        {
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142,
            8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117,
            35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71,
            134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41,
            55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
            18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
            250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182,
            189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43,
            172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97,
            228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
            49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138,
            236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
            // (Repeat 0~255)
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142,
            8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117,
            35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71,
            134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41,
            55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
            18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
            250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182,
            189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43,
            172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97,
            228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
            49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138,
            236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
        };

        static readonly F32 F32_6 = F32.FromInt(6);
        static readonly F32 F32_15 = F32.FromInt(15);
        static readonly F32 F32_10 = F32.FromInt(10);
    }
}
