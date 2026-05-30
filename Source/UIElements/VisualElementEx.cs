// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
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

        public static int GetAncestors(this VisualElement element, List<VisualElement> buffer, bool includeSelf)
        {
            int count = 0;
            VisualElement? parent = includeSelf ? element : element.parent;
            while (parent is not null)
            {
                buffer.Add(parent);
                count++;

                parent = parent.parent;
            }
            return count;
        }

        /// <param name="toolTip">Root element of the tool-tip. This element should provide whole size of the tool-tip</param>
        /// <param name="root">Full-screen element. This element should provide screen size in UI space.</param>
        /// <param name="anchorPoint">Mouse(or other pointer) location in <b>UI space</b>.</param>
        /// <param name="toolTipOffset">Offset between <paramref name="anchorPoint"/> and <paramref name="toolTip"/> in UI space.</param>
        /// <param name="screenMargin">Margin around the screen to push the tool-tip in UI space.</param>
        /// <param name="result">Value for (style.left, style.right) in UI space.</param>
        /// <remarks>
        /// Assumming the <paramref name="toolTip"/> using <see cref="Position.Absolute"/>.<br/>
        /// Assumming the <paramref name="root"/> using Reference Resolution with Scale Mode in Panel Settings.<br/>
        /// <br/>
        /// The UI space means the Reference Resolution space. Not actual screen space.<br/>
        /// For example, the actual screen size is 2560x1440(QHD) and the Reference Resolution is 1920x1080(FHD), this method requires 1920x1080(FHD).<br/>
        /// Therefore, the <paramref name="anchorPoint"/> must converted from mouse location by proper API such as <see cref="RuntimePanelUtils.ScreenToPanel(IPanel, Vector2)"/>.<br/>
        /// Note that, coordinate system of UI Toolkit and Input are different. Have to invert Y-value before use it.
        /// </remarks>
        public static bool TryGetToolTipLocation(
            VisualElement toolTip,
            VisualElement root,
            Vector2 anchorPoint,
            ThicknessF toolTipOffset,
            ThicknessF screenMargin,
            out Vector2 result)
        {
            float screenWidth = root.resolvedStyle.width;
            float screenHeight = root.resolvedStyle.height;

            float toolTipWidth = toolTip.resolvedStyle.width;
            float toolTipHeight = toolTip.resolvedStyle.height;

            // Have to check the layout had been updated.
            if (float.IsNaN(screenWidth) || float.IsNaN(screenHeight) ||
                float.IsNaN(toolTipWidth) || float.IsNaN(toolTipHeight))
            {
                result = new Vector2(
                    anchorPoint.x + toolTipOffset.Right,
                    anchorPoint.y + toolTipOffset.Bottom);
                return false;
            }

            float minX = screenMargin.Left;
            float maxX = screenWidth - screenMargin.Right;
            float minY = screenMargin.Top;
            float maxY = screenHeight - screenMargin.Bottom;

            float xPos, yPos;

            // --- Y-Axis ---
            // Try lower side.
            if (anchorPoint.y + toolTipOffset.Bottom + toolTipHeight <= maxY)
            {
                yPos = anchorPoint.y + toolTipOffset.Bottom;
            }
            // Upper side.
            else
            {
                yPos = anchorPoint.y - toolTipOffset.Top - toolTipHeight;
            }

            // Clamping by margin.
            if (yPos < minY)
            {
                yPos = minY;
            }
            else if (yPos + toolTipHeight > maxY)
            {
                yPos = maxY - toolTipHeight;
            }

            // --- X-Axis ---
            // Try right side.
            if (anchorPoint.x + toolTipOffset.Right + toolTipWidth <= maxX)
            {
                xPos = anchorPoint.x + toolTipOffset.Right;
            }
            // Left side.
            else
            {
                xPos = anchorPoint.x - toolTipOffset.Left - toolTipWidth;
            }

            // Clamping by margin.
            if (xPos < minX)
            {
                xPos = minX;
            }
            else if (xPos + toolTipWidth > maxX)
            {
                xPos = maxX - toolTipWidth;
            }

            result = new Vector2(xPos, yPos);
            return true;
        }
    }
}
