// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using UnityEngine;

namespace Jih.Unity.Infrastructure.Deterministics
{
    partial struct Vector3F64
    {
        /// <remarks>
        /// Deterministic-safe.
        /// </remarks>
        public static explicit operator Vector3F64(Vector3 vector)
        {
            return new Vector3F64((F64)vector.x, (F64)vector.y, (F64)vector.z);
        }
        /// <remarks>
        /// <b>NOT</b> deterministic-safe.
        /// </remarks>
        public static explicit operator Vector3(Vector3F64 vector)
        {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }
    }
}
