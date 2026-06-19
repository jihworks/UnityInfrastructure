// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using UnityEngine.UIElements;

namespace Jih.Unity.Infrastructure.UIElements
{
    public static class PointerEventEx
    {
        public static UnityMouseButton GetUnityMouseButton<T>(this PointerEventBase<T> e) where T : PointerEventBase<T>, new()
        {
            return (UnityMouseButton)e.button;
        }
    }

    public enum UnityMouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
    }
}
