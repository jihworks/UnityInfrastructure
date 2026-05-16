// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Runtime.Easing
{
    public abstract class Sine : IEase
    {
        public abstract double Evaluate(double p);
    }

    public class SineIn : Sine
    {
        public static SineIn Default { get; } = new SineIn();

        public override double Evaluate(double p)
        {
            return -Math.Cos(p * Math.PI * 0.5) + 1d;
        }
    }

    public class SineOut : Sine
    {
        public static SineOut Default { get; } = new SineOut();

        public override double Evaluate(double p)
        {
            return Math.Sin(p * Math.PI * 0.5);
        }
    }

    public class SineInOut : Sine
    {
        public static SineInOut Default { get; } = new SineInOut();

        public override double Evaluate(double p)
        {
            return -0.5 * (Math.Cos(Math.PI * p) - 1d);
        }
    }
}
