// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Runtime.Easing
{
    public abstract class Elastic : IEase
    {
        public const double DefaultAmplitude = 1d;
        public const double DefaultPeriod = 0.3;

        public double Amplitude { get; }
        public double Period { get; }

        protected readonly double v;

        public Elastic(double amplitude, double period)
        {
            if (amplitude < 1d || period == 0d)
            {
                throw new ArgumentException();
            }
            Amplitude = amplitude;
            Period = period;
            v = Period / Math.PI * 2d * Math.Asin(1d / Amplitude);
        }

        public abstract double Evaluate(double p);
    }

    public class ElasticIn : Elastic
    {
        public static ElasticIn Default { get; } = new ElasticIn();

        public ElasticIn(double amplitude = DefaultAmplitude, double period = DefaultPeriod) : base(amplitude, period)
        {
        }

        public override double Evaluate(double p)
        {
            return -(Amplitude * Math.Pow(2d, 10d * (p -= 1d)) * Math.Sin((p - v) * Math.PI * 2d / Period));
        }
    }

    public class ElasticOut : Elastic
    {
        public static ElasticOut Default { get; } = new ElasticOut();

        public ElasticOut(double amplitude = DefaultAmplitude, double period = DefaultPeriod) : base(amplitude, period)
        {
        }

        public override double Evaluate(double p)
        {
            return Amplitude * Math.Pow(2d, -10d * p) * Math.Sin((p - v) * Math.PI * 2d / Period) + 1d;
        }
    }

    public class ElasticInOut : Elastic
    {
        public static ElasticInOut Default { get; } = new ElasticInOut();

        public ElasticInOut(double amplitude = DefaultAmplitude, double period = DefaultPeriod) : base(amplitude, period)
        {
        }

        public override double Evaluate(double p)
        {
            return ((p *= 2d) < 1d) ?
                -0.5 * (Amplitude * Math.Pow(2d, 10d * (p -= 1d)) * Math.Sin((p - v) * Math.PI * 2d / Period)) :
                Amplitude * Math.Pow(2d, -10d * (p -= 1d)) * Math.Sin((p - v) * Math.PI * 2d / Period) * 0.5 + 1d;
        }
    }
}
