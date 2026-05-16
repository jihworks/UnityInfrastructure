// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Runtime.Easing
{
    public abstract class Bounce : IEase
    {
        public abstract double Evaluate(double p);
    }

    public class BounceIn : Bounce
    {
        public static BounceIn Default { get; } = new BounceIn();

        public override double Evaluate(double p)
        {
            if ((p = 1d - p) < 1d / 2.75)
            {
                return 1d - (7.56 * p * p);
            }
            else if (p < 2d / 2.75)
            {
                return 1d - (7.56 * (p -= 1.5 / 2.75) * p + 0.75);
            }
            else if (p < 2.5 / 2.75)
            {
                return 1d - (7.56 * (p -= 2.25 / 2.75) * p + 0.93);
            }
            else
            {
                return 1d - (7.56 * (p -= 2.62 / 2.75) * p + 0.98);
            }
        }
    }

    public class BounceOut : Bounce
    {
        public static BounceOut Default { get; } = new BounceOut();

        public override double Evaluate(double p)
        {
            if (p < 1d / 2.75)
            {
                return 7.56 * p * p;
            }
            else if (p < 2d / 2.75)
            {
                return 7.56 * (p -= 1.5 / 2.75) * p + 0.75;
            }
            else if (p < 2.5 / 2.75)
            {
                return 7.56 * (p -= 2.25 / 2.75) * p + 0.93;
            }
            else
            {
                return 7.56 * (p -= 2.62 / 2.75) * p + 0.98;
            }
        }
    }

    public class BounceInOut : Bounce
    {
        public static BounceInOut Default { get; } = new BounceInOut();

        public override double Evaluate(double p)
        {
            bool inverse = false;
            if (p < 0.5)
            {
                inverse = true;
                p = 1d - (p * 2);
            }
            else
            {
                p = (p * 2) - 1d;
            }
            if (p < 1d / 2.75)
            {
                p = 7.56 * p * p;
            }
            else if (p < 2 / 2.75)
            {
                p = 7.56 * (p -= 1.5 / 2.75) * p + 0.75;
            }
            else if (p < 2.5 / 2.75)
            {
                p = 7.56 * (p -= 2.25 / 2.75) * p + 0.93;
            }
            else
            {
                p = 7.56 * (p -= 2.62 / 2.75) * p + 0.98;
            }
            return inverse ? (1d - p) * 0.5 : p * 0.5 + 0.5;
        }
    }
}
