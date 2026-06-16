// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Numerics;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public readonly struct TileOrientation
    {
        public readonly ScreenV Origin;
        /// <summary>
        /// Full size of a cell.
        /// </summary>
        /// <remarks>
        /// <b>Not</b> half-size or <b>not</b> extents.
        /// </remarks>
        public readonly ScreenV Size;

        /// <summary>
        /// Horizontal distance from center of a cell to another center of a cell.
        /// </summary>
        public float ScreenHorizontalSpacing => Size.X;
        /// <summary>
        /// Vertical distance from center of a cell to another center of a cell.
        /// </summary>
        public float ScreenVerticalSpacing => Size.Y;

        public TileOrientation(ScreenV origin, ScreenV size)
        {
            Origin = origin;
            Size = size;
        }

        public readonly ScreenV TileToScreen(TileCoordF coord)
        {
            return Origin + new ScreenV(coord.X, coord.Y) * Size;
        }

        public readonly TileCoordF ScreenToTile(ScreenV point)
        {
            point -= Origin;
            point /= Size;
            return new TileCoordF(point.X, point.Y);
        }

        public readonly ScreenR ScreenBounds(int startX, int startY, int width, int height)
        {
            return new ScreenR(
                Origin.X + startX * Size.X,
                Origin.Y + startY * Size.Y,
                width * Size.X,
                height * Size.Y);
        }
    }
}
