// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Jih.Unity.Infrastructure
{
    public static class ColorEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromHexRgba(uint hex)
        {
            return FromHexRgba32(hex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromHexRgba(string hex)
        {
            return FromHexRgba32(hex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromHexRgb(uint hex)
        {
            return FromHexRgb32(hex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromHexRgb(string hex)
        {
            return FromHexRgb32(hex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 FromHexRgba32(uint hex)
        {
            if (hex > 0xFFFFFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(hex), $"Hex value 0x{hex:X} is too large to fit in a RGBA Color32.");
            }

            byte r = (byte)((hex >> 24) & 0xFF);
            byte g = (byte)((hex >> 16) & 0xFF);
            byte b = (byte)((hex >> 8) & 0xFF);
            byte a = (byte)(hex & 0xFF);

            return new Color32(r, g, b, a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 FromHexRgba32(string hex)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex[1..];
            }
            uint hexValue = uint.Parse(hex, NumberStyles.HexNumber);
            return FromHexRgba32(hexValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 FromHexRgb32(uint hex)
        {
            if (hex > 0xFFFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(hex), $"Hex value 0x{hex:X} is too large to fit in a RGB Color32.");
            }

            byte r = (byte)((hex >> 16) & 0xFF);
            byte g = (byte)((hex >> 8) & 0xFF);
            byte b = (byte)(hex & 0xFF);

            return new Color32(r, g, b, byte.MaxValue);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 FromHexRgb32(string hex)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex[1..];
            }
            uint hexValue = uint.Parse(hex, NumberStyles.HexNumber);
            return FromHexRgb32(hexValue);
        }
    }
}
