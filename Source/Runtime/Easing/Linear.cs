// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Runtime.Easing
{
    public class Linear : IEase
    {
        public static Linear Default { get; } = new Linear();

        public double Evaluate(double p)
        {
            return p;
        }
    }
}
