// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using UnityEngine;

namespace Jih.Unity.Infrastructure
{
    namespace ScreenSpace
    {
        namespace XZ
        {
            /// <summary>
            /// * Standard Computer Graphics Screen Space Coordinate:<br/>
            /// Origin: Left-top corner.<br/>
            /// +X: Right direction.<br/>
            /// +Y: Down direction.<br/>
            /// Positive-Rotation: Clockwise(CW).<br/>
            /// 0-Rotation: +X(right direction).
            /// </summary>
            /// <seealso cref="ScreenV"/>
            /// <seealso cref="ScreenM"/>
            public static class ScreenSpaceEx
            {
                public static Vector3 ScreenToUnity(this ScreenV v, float y = 0f)
                {
                    return new Vector3(v.X, y, -v.Y);
                }
                public static ScreenV UnityToScreen(this Vector3 v)
                {
                    return new ScreenV(v.x, -v.z);
                }
            }
        }
        namespace XY
        {
            /// <inheritdoc cref="XZ.ScreenSpaceEx"/>
            public static class ScreenSpaceEx
            {
                public static Vector3 ScreenToUnity(this ScreenV v, float z = 0f)
                {
                    return new Vector3(v.X, -v.Y, z);
                }
                public static ScreenV UnityToScreen(this Vector3 v)
                {
                    return new ScreenV(v.x, -v.y);
                }
            }
        }
    }
}
