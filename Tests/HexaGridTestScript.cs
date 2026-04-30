// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure;
using Jih.Unity.Infrastructure.HexaGrid;
using NUnit.Framework;
using UnityEngine;

public class HexaGridTestScript
{
    [Test]
    public void VertexCoordAlternativeTest()
    {
        HexaMap map = new(128, 128, null, null, null);

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                HexaCell cell = map[new HexaIndex(x, y)];

                Vector2 cellCenter = o.HexaToScreen(cell.Coord);

                for (int p = 0; p < 6; p++)
                {
                    HexaVertexPosition position = (HexaVertexPosition)p;

                    float angle = position.GetRadiusDegrees();
                    Vector2 radiusVector = MathEx.RadiusVector(angle.ToRadians());
                    Vector2 centeredLocation = cellCenter + radiusVector;

                    HexaVertex vertex = cell.GetVertex(position);
                    Vector2 coordedLocation = o.HexaToScreen(vertex.Coord);

                    Assert.IsTrue(centeredLocation.IsNearly(coordedLocation));
                }
            }
        }
    }

    [Test]
    public void AnchorCoordAlternativeTest()
    {
        HexaMap map = new(128, 128, null, null, null);

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                HexaIndex index = new(x, y);

                HexaCell cell = map[index];
                HexaAnchor anchor = new(null, (HexaCoord)index);

                for (int p = 0; p < 6; p++)
                {
                    HexaVertexPosition position = (HexaVertexPosition)p;

                    HexaVertex vertex = cell.GetVertex(position);
                    Vector2 coordedLocation = o.HexaToScreen(vertex.Coord);

                    Vector2 anchoredLocation = o.HexaToScreen(anchor.GetVertexCoord(position));

                    Assert.IsTrue(anchoredLocation.IsNearly(coordedLocation));
                }
            }
        }
    }

    static readonly HexaOrientation o = new(Vector2.zero, Vector2.one);
}
