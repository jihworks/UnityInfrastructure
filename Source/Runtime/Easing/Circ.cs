// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Runtime.Easing
{
    public abstract class Circ : IEase
    {
        public abstract double Evaluate(double p);
    }

    public class CircIn : Circ
    {
        public static CircIn Default { get; } = new CircIn();

        public override double Evaluate(double p)
        {
            return -(Math.Sqrt(1d - (p * p)) - 1d);
        }
    }

    public class CircOut : Circ
    {
        public static CircOut Default { get; } = new CircOut();

        public override double Evaluate(double p)
        {
            return Math.Sqrt(1d - (--p) * p);
        }
    }

    public class CircInOut : Circ
    {
        public static CircInOut Default { get; } = new CircInOut();

        public override double Evaluate(double p)
        {
            return (p *= 2d) < 1d ?
                -0.5 * (Math.Sqrt(1d - p * p) - 1d) :
                0.5 * (Math.Sqrt(1d - (p -= 2d) * p) + 1d);
        }
    }
}
