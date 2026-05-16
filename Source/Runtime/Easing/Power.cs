// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Runtime.Easing
{
    public abstract class Power : IEase
    {
        public abstract double Evaluate(double p);
    }

    public class Power0In : Power
    {
        public static Power0In Default { get; } = new Power0In();

        public override double Evaluate(double p)
        {
            return p;
        }
    }
    public class Power0Out : Power
    {
        public static Power0Out Default { get; } = new Power0Out();

        public override double Evaluate(double p)
        {
            return p;
        }
    }
    public class Power0InOut : Power
    {
        public static Power0InOut Default { get; } = new Power0InOut();

        public override double Evaluate(double p)
        {
            return p;
        }
    }

    public class Power1In : Power
    {
        public static Power1In Default { get; } = new Power1In();

        public override double Evaluate(double p)
        {
            return p * p;
        }
    }
    public class Power1Out : Power
    {
        public static Power1Out Default { get; } = new Power1Out();

        public override double Evaluate(double p)
        {
            return -p * (p - 2d);
        }
    }
    public class Power1InOut : Power
    {
        public static Power1InOut Default { get; } = new Power1InOut();

        public override double Evaluate(double p)
        {
            if ((p /= 0.5) < 1d)
            {
                return 0.5 * p * p;
            }
            return -0.5 * ((--p) * (p - 2d) - 1d);
        }
    }

    public class Power2In : Power
    {
        public static Power2In Default { get; } = new Power2In();

        public override double Evaluate(double p)
        {
            return p * p * p;
        }
    }
    public class Power2Out : Power
    {
        public static Power2Out Default { get; } = new Power2Out();

        public override double Evaluate(double p)
        {
            return (p -= 1d) * p * p + 1d;
        }
    }
    public class Power2InOut : Power
    {
        public static Power2InOut Default { get; } = new Power2InOut();

        public override double Evaluate(double p)
        {
            if ((p /= 0.5) < 1d)
            {
                return 0.5 * p * p * p;
            }
            return 0.5 * ((p -= 2d) * p * p + 2d);
        }
    }

    public class Power3In : Power
    {
        public static Power3In Default { get; } = new Power3In();

        public override double Evaluate(double p)
        {
            return p * p * p * p;
        }
    }
    public class Power3Out : Power
    {
        public static Power3Out Default { get; } = new Power3Out();

        public override double Evaluate(double p)
        {
            return -1d * ((p -= 1d) * p * p * p - 1d);
        }
    }
    public class Power3InOut : Power
    {
        public static Power3InOut Default { get; } = new Power3InOut();

        public override double Evaluate(double p)
        {
            if ((p /= 0.5) < 1d)
            {
                return 0.5 * p * p * p * p;
            }
            return -0.5 * ((p -= 2d) * p * p * p - 2d);
        }
    }

    public class Power4In : Power
    {
        public static Power4In Default { get; } = new Power4In();

        public override double Evaluate(double p)
        {
            return p * p * p * p * p;
        }
    }
    public class Power4Out : Power
    {
        public static Power4Out Default { get; } = new Power4Out();

        public override double Evaluate(double p)
        {
            return (p -= 1d) * p * p * p * p + 1d;
        }
    }
    public class Power4InOut : Power
    {
        public static Power4InOut Default { get; } = new Power4InOut();

        public override double Evaluate(double p)
        {
            if ((p /= 0.5) < 1d)
            {
                return 0.5 * p * p * p * p * p;
            }
            return 0.5 * ((p -= 2d) * p * p * p * p + 2d);
        }
    }
}
