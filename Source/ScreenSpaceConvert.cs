// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Jih.Unity.Infrastructure
{
    /// <summary>
    /// * Standard Computer Graphics Screen Coordinate Space:<br/>
    /// Origin: Left-top corner.<br/>
    /// +X: Right direction.<br/>
    /// +Y: Down direction.<br/>
    /// Positive-Rotation: Clockwise(CW).<br/>
    /// 0-Rotation: +X(right direction).
    /// </summary>
    /// <seealso cref="ScreenV"/>
    /// <seealso cref="ScreenM"/>
    /// <seealso cref="ScreenR"/>
    public static class ScreenSpaceConvert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ScreenToUnityXZ(ScreenV v, float y = 0f)
        {
            return new Vector3(v.X, y, -v.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV UnityXZToScreen(Vector3 v)
        {
            return new ScreenV(v.x, -v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ScreenToUnityXY(ScreenV v, float z = 0f)
        {
            return new Vector3(v.X, -v.Y, z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV UnityXYToScreen(Vector3 v)
        {
            return new ScreenV(v.x, -v.y);
        }
    }
}
