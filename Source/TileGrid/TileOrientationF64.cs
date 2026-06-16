// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Deterministics;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public readonly struct TileOrientationF64
    {
        public readonly Vector2F64 Origin;
        /// <inheritdoc cref="TileOrientation.Size"/>
        public readonly Vector2F64 Size;

        /// <inheritdoc cref="TileOrientation.ScreenHorizontalSpacing"/>
        public F64 ScreenHorizontalSpacing => Size.X;
        /// <inheritdoc cref="TileOrientation.ScreenVerticalSpacing"/>
        public F64 ScreenVerticalSpacing => Size.Y;

        public TileOrientationF64(Vector2F64 origin, Vector2F64 size)
        {
            Origin = origin;
            Size = size;
        }

        public readonly Vector2F64 TileToScreen(TileCoordF64 coord)
        {
            return Origin + new Vector2F64(coord.X, coord.Y) * Size;
        }

        public readonly TileCoordF64 ScreenToTile(Vector2F64 point)
        {
            point -= Origin;
            point /= Size;
            return new TileCoordF64(point.X, point.Y);
        }
    }
}
