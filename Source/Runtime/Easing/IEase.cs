// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Runtime.Easing
{
    public interface IEase
    {
        double Evaluate(double p);
    }
}
