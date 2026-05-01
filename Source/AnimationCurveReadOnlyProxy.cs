// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using UnityEngine;

namespace Jih.Unity.Infrastructure
{
    public readonly struct AnimationCurveReadOnlyProxy
    {
        public readonly int Length => _source.length;

        readonly AnimationCurve _source;

        public AnimationCurveReadOnlyProxy(AnimationCurve source)
        {
            _source = source;
        }

        public readonly void GetKeys(Span<Keyframe> destination)
        {
            _source.GetKeys(destination);
        }

        public readonly float Evaluate(float time)
        {
            return _source.Evaluate(time);
        }
    }
}
