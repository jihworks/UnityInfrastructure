// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Runtime.Easing
{
    public abstract class Back : IEase
    {
        public const double DefaultStrenth = 1.7;

        public double Strength { get; }

        public Back(double strength)
        {
            Strength = strength;
        }

        public abstract double Evaluate(double p);
    }

    public class BackIn : Back
    {
        public static BackIn Default { get; } = new BackIn();

        public BackIn(double strength = DefaultStrenth) : base(strength)
        {
        }

        public override double Evaluate(double p)
        {
            return p * p * ((Strength + 1d) * p - Strength);
        }
    }

    public class BackOut : Back
    {
        public static BackOut Default { get; } = new BackOut();

        public BackOut(double strength = DefaultStrenth) : base(strength)
        {
        }

        public override double Evaluate(double p)
        {
            return (--p) * p * ((Strength + 1d) * p + Strength) + 1d;
        }
    }

    public class BackInOut : Back
    {
        public static BackInOut Default { get; } = new BackInOut();

        public BackInOut(double strength = DefaultStrenth) : base(strength)
        {
        }

        public override double Evaluate(double p)
        {
            double str = Strength * 1.5;
            return (p *= 2d) < 1d ?
                0.5 * p * p * ((str + 1d) * p - str) :
                0.5 * ((p -= 2d) * p * ((str + 1d) * p + str) + 2d);
        }
    }
}
