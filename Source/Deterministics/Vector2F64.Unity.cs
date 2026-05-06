// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using UnityEngine;

namespace Jih.Unity.Infrastructure.Deterministics
{
    partial struct Vector2F64
    {
        /// <remarks>
        /// Deterministic-safe.
        /// </remarks>
        public static explicit operator Vector2F64(Vector2 vector)
        {
            return new Vector2F64((F64)vector.x, (F64)vector.y);
        }
        /// <remarks>
        /// <b>NOT</b> deterministic-safe.
        /// </remarks>
        public static explicit operator Vector2(Vector2F64 vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }
    }
}
