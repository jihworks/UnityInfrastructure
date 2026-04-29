// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#ifndef __INFRASTRUCTURE_HEXA_GRID_INTEROP_INCLUDED__
#define __INFRASTRUCTURE_HEXA_GRID_INTEROP_INCLUDED__

int2 HexaCoord_ToHexaIndex(int3 hexaCoord)
{
    int parity = hexaCoord.y & 1;
    int c = hexaCoord.x + ((hexaCoord.y - parity) >> 1);
    return int2(c, hexaCoord.y);
}
int3 HexaIndex_ToHexaCoord(int2 hexaIndex)
{
    int parity = hexaIndex.y & 1;
    int a = hexaIndex.x - ((hexaIndex.y - parity) >> 1);
    int b = hexaIndex.y;
    return int3(a, b, -a - b);
}

int3 HexaCoordF_Round(float3 hexaCoordF)
{
    float a = hexaCoordF.x;
    float b = hexaCoordF.y;
    float c = hexaCoordF.z;

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

    return int3(rA, rB, rC);
}

struct HexaOrientation
{
    float F0, F1, F2, F3;
    float B0, B1, B2, B3;

    float2 Origin, Radius;
};

HexaOrientation HexaOrientation_Create(float2 hexaOrigin, float2 hexaRadius)
{
    HexaOrientation r = (HexaOrientation)0;
    
    r.F0 = 1.7320508; // sqrt(3)
    r.F1 = 0.8660254; // sqrt(3)/2
    r.F2 = 0.0;
    r.F3 = 1.5;       // 3/2
    r.B0 = 0.5773503; // sqrt(3)/3
    r.B1 = -0.3333333;// -1/3
    r.B2 = 0.0;
    r.B3 = 0.6666667; // 2/3

    r.Origin = hexaOrigin;
    r.Radius = hexaRadius;

    return r;
}

float3 HexaOrientation_ScreenToHexa(HexaOrientation o, float2 screenLocation)
{
    float2 pt = float2(
        (screenLocation.x - o.Origin.x) / o.Radius.x,
        (screenLocation.y - o.Origin.y) / o.Radius.y);
    float a = o.B0 * pt.x + o.B1 * pt.y;
    float b = o.B2 * pt.x + o.B3 * pt.y;
    return float3(a, b, -a - b);
}

float2 HexaOrientation_HexaToScreen(HexaOrientation o, float3 hexaCoord)
{
    float x = (o.F0 * hexaCoord.x + o.F1 * hexaCoord.y) * o.Radius.x;
    float y = (o.F2 * hexaCoord.x + o.F3 * hexaCoord.y) * o.Radius.y;
    return float2(x + o.Origin.x, y + o.Origin.y);
}

#endif
