// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Runtime.Easing
{
    public abstract class Expo : IEase
    {
        public abstract double Evaluate(double p);
    }

    public class ExpoIn : Expo
    {
        public static ExpoIn Default { get; } = new ExpoIn();

        public override double Evaluate(double p)
        {
            return p <= 0d ? 0d : Math.Pow(2d, 10d * (p - 1d));
        }
    }

    public class ExpoOut : Expo
    {
        public static ExpoOut Default { get; } = new ExpoOut();

        public override double Evaluate(double p)
        {
            return p >= 1d ? 1d : 1d - Math.Pow(2d, -10d * p);
        }
    }

    public class ExpoInOut : Expo
    {
        public static ExpoInOut Default { get; } = new ExpoInOut();

        public override double Evaluate(double p)
        {
            if (p <= 0)
            {
                return 0d;
            }
            if (1 <= p)
            {
                return 1d;
            }
            return (p *= 2d) < 1d ?
                0.5 * Math.Pow(2d, 10d * (p - 1d)) :
                0.5 * (2d - Math.Pow(2d, -10d * (p - 1d)));
        }
    }
}
