// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using UnityEngine.UIElements;

namespace Jih.Unity.Infrastructure.UIElements
{
    public static class VisualElementEx
    {
        public static TElement QT<TElement>(this VisualElement element, string? name = null, string? className = null) where TElement : VisualElement
        {
            return element.Q<TElement>(name, className) ?? throw new NullReferenceException($"Visual element not found. Name: '{name}', ClassName: '{className}'");
        }
        public static VisualElement QT(this VisualElement element, string? name = null, string? className = null)
        {
            return element.Q(name, className) ?? throw new NullReferenceException($"Visual element not found. Name: '{name}', ClassName: '{className}'");
        }
    }
}
