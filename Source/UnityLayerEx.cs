// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Jih.Unity.Infrastructure
{
    public static class UnityLayerEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIdOrThrow(string layerName)
        {
            int layerId = LayerMask.NameToLayer(layerName);
            if (layerId < 0)
            {
                throw new ArgumentException($"Layer '{layerName}' does not exist.");
            }
            return layerId;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIdOrThrow(string layerName, out int mask)
        {
            int layerId = LayerMask.NameToLayer(layerName);
            if (layerId < 0)
            {
                throw new ArgumentException($"Layer '{layerName}' does not exist.");
            }
            mask = 1 << layerId;
            return layerId;
        }
    }
}
