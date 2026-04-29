// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#ifndef HEXA_GRID_INTEROP_INCLUDED
#define HEXA_GRID_INTEROP_INCLUDED

void GetNearestHexaVertexIndex_float(float3 worldLocation, float2 hexaOrigin, float2 hexaRadius, out float2 vertexIndex, out float vertexSector)
{
    // HexaOrientation Constants for Pointy-topped.
    float F0 = 1.7320508; // sqrt(3)
    float F1 = 0.8660254; // sqrt(3)/2
    float F3 = 1.5;       // 3/2
    float B0 = 0.5773503; // sqrt(3)/3
    float B1 = -0.3333333;// -1/3
    float B3 = 0.6666667; // 2/3

    // Convert Unity Space to Screen Space.
    float2 P = float2(worldLocation.x, -worldLocation.z) / hexaRadius;

    // Screen to HexaCoordF.
    float a = B0 * P.x + B1 * P.y;
    float b = B3 * P.y;
    float c = -a - b;

    // Round HexaCoordF.
    int rA = round(a);
    int rB = round(b);
    int rC = round(c);
    float d_a = abs(rA - a);
    float d_b = abs(rB - b);
    float d_c = abs(rC - c);

    if (d_a > d_b && d_a > d_c)
    {
        rA = -rB - rC;
    }
    else if (d_b > d_c)
    {
        rB = -rA - rC;
    }

    // HexaCoord to HexaIndex.
    int parity = rB & 1;
    int indexX = rA + ((rB - parity) >> 1); // Faster operation than divide by int 2.
    int indexY = rB;

    // Calculate the center location of the HexaCell in Unity Space.
    float centerScreenX = hexaOrigin.x + (F0 * rA + F1 * rB) * hexaRadius.x;
    float centerScreenY = hexaOrigin.y + (F3 * rB) * hexaRadius.y;

    float centerWorldX = centerScreenX;
    float centerWorldZ = -centerScreenY;

    // Calculate direction vector from cell center to given Unity location.
    float diffX = worldLocation.x - centerWorldX;
    float diffZ = worldLocation.z - centerWorldZ;

    // Calculate angle in radians.
    // atan2(x, z) instead of atan2(y, x) maps 0 rad to +Z and increases CW.
    float angle = atan2(diffX, diffZ);

    // Vertices are exactly at 0, 60, 120 degrees. 
    // We add 30 degrees (PI/6 = 0.52359877 rad) to shift the sector boundaries.
    // So that the vertex itself sits in the middle of its corresponding Voronoi sector.
    float shiftedAngle = angle + 0.52359877;

    // Normalize angle to [0, 2PI)
    if (shiftedAngle < 0.0)
    {
        shiftedAngle += 6.2831853;
    }
    shiftedAngle = fmod(shiftedAngle, 6.2831853);

    // Divide by 60 degrees (PI/3 = 1.04719755 rad) to get Sector ID. (0 to 5)
    int sector = floor(shiftedAngle / 1.04719755);
    vertexSector = sector;

    // Map Sector ID to HexaVertexIndex.
    int baseX = indexX * 2 + parity;

    // Mapping: D0=0, D60=1, D120=2, D180=3, D240=4, D300=5
    int offsetX[6] = { 1, 2, 2, 1, 0, 0, };
    int offsetY[6] = { 0, 0, 1, 1, 1, 0, };

    vertexIndex.x = baseX + offsetX[sector];
    vertexIndex.y = indexY + offsetY[sector];
}

void GetHexaVertexIndexWorldLocation_float(float2 hexaVertexIndex, float2 hexaOrigin, float2 hexaRadius, out float3 worldLocation)
{
    int vertexX = (int)hexaVertexIndex.x;
    int vertexY = (int)hexaVertexIndex.y;

    // Determine D0 or D300 and HexaIndex. (cellX, cellY)
    int parity = vertexY & 1;
    int vx_minus_p = vertexX - parity;

    int rem = vx_minus_p & 1;
    int cellX = vx_minus_p >> 1;
    int cellY = vertexY;

    // HexaIndex (cellX, cellY) -> HexaCoord (A, B)
    int cellParity = cellY & 1;
    int a = cellX - ((cellY - cellParity) >> 1); // Faster operation than divide by int 2.
    int b = cellY;

    // Calculate Screen / Unity center coordinate of the cell.
    float F0 = 1.7320508; // sqrt(3)
    float F1 = 0.8660254; // sqrt(3)/2
    float F3 = 1.5;       // 3/2

    float centerScreenX = hexaOrigin.x + (F0 * a + F1 * b) * hexaRadius.x;
    float centerScreenY = hexaOrigin.y + (F3 * b) * hexaRadius.y;

    float centerWorldX = centerScreenX;
    float centerWorldZ = -centerScreenY;

    // Add offset of D0 or D300 to cell center.
    float offsetX = 0.0;
    float offsetZ = 0.0;
    if (rem == 1)
    {
        // Case D0 (12 o'clock, 0 degrees)
        offsetX = 0.0;
        offsetZ = 1.0 * hexaRadius.y;
    }
    else
    {
        // Case D300 (10 o'clock, 300 degrees)
        // sin(300 deg) = -0.8660254, cos(300 deg) = 0.5
        offsetX = -0.8660254 * hexaRadius.x;
        offsetZ = 0.5 * hexaRadius.y;
    }

    worldLocation = float3(centerWorldX + offsetX, 0.0, centerWorldZ + offsetZ);
}

#endif
