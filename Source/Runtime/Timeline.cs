// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure;
using Jih.Unity.Runtime.Easing;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Jih.Unity.Runtime
{
    [DebuggerDisplay("{GetType().Name}, From={From}, To={To}, Value={Value}")]
    public class Timeline
    {
        public event UpdateHandler? Update;
        public event FinishedHandler? Finished;

        public bool IsRunning { get; private set; } = false;

        float _duration = 1f;
        /// <summary>
        /// In seconds.
        /// </summary>
        public float Duration
        {
            get => _duration;
            set
            {
                if (value < 0f)
                {
                    throw new ArgumentException("Duration cannot be negative.");
                }
                _duration = value;
            }
        }

        public bool IsLooping { get; set; } = false;
        public bool IsReversing { get; set; } = false;

        /// <summary>
        /// Applied before <see cref="AnimationCurve"/>.
        /// </summary>
        public IEase? Ease { get; set; }
        /// <summary>
        /// Applied after <see cref="Ease"/>.
        /// </summary>
        public AnimationCurve? AnimationCurve { get; set; }

        /// <summary>
        /// In seconds.
        /// </summary>
        public float Position { get; private set; }
        /// <summary>
        /// Linear [0, 1] by <see cref="Position"/>.
        /// </summary>
        public float Alpha => Position.SafeDivide(Duration);

        public float From { get; set; } = 0f;
        public float To { get; set; } = 1f;

        public float Value => Evaluate(From, To);

        float speed = 1f;
        public float Speed
        {
            get => speed;
            set
            {
                if (value < 0f)
                {
                    throw new ArgumentException("Speed cannot be negative.");
                }
                speed = value;
            }
        }

        public Action? FinishedCallback { get; set; }

        public Timeline()
        {

        }
        public Timeline(float duration, float from = 0f, float to = 1f, Action? finishedCallback = null)
        {
            Duration = duration;
            From = from;
            To = to;
            FinishedCallback = finishedCallback;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                Tick(0f);
            }
        }
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
            }
        }
        public void Reset(float position = 0f)
        {
            if (position < 0f || Duration < position)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }
            Position = position;
        }
        public void Restart(float position = 0f)
        {
            Reset(position);
            Start();
        }

        public void Tick(float deltaTime)
        {
            if (!IsRunning)
            {
                return;
            }
            if (Duration <= 0f)
            {
                Position = 0f;
                OnUpdate();
                Stop();
                OnFinished();
                return;
            }

            if (IsReversing)
            {
                if (Position > 0f)
                {
                    Position -= deltaTime * speed;
                }
                else
                {
                    if (IsLooping)
                    {
                        Position %= _duration;
                        Position += _duration;
                    }
                    else
                    {
                        Position = 0f;
                        OnUpdate(); // Callback before stop.
                        if (Position <= 0f) // User can Reset in above callback.
                        {
                            Stop();
                            OnFinished();
                        }
                    }
                }
            }
            else
            {
                if (Position < _duration)
                {
                    Position += deltaTime * speed;
                }
                else
                {
                    if (IsLooping)
                    {
                        Position %= _duration;
                    }
                    else
                    {
                        Position = _duration;
                        OnUpdate(); // Callback before stop.
                        if (Position >= _duration) // User can Reset in above callback.
                        {
                            Stop();
                            OnFinished();
                        }
                    }
                }
            }

            if (IsRunning) // Not stopped.
            {
                OnUpdate();
            }
        }

        public float Evaluate(FromTo fromTo)
        {
            return Evaluate(fromTo.From, fromTo.To);
        }
        /// <summary>
        /// Evaluate current-value from given from-value and to-value.
        /// </summary>
        /// <remarks>
        /// This method will ignore <see cref="From"/> and <see cref="To"/>.
        /// </remarks>
        public float Evaluate(float @from, float to)
        {
            float alpha = Alpha;
            if (Ease is not null)
            {
                alpha = (float)Ease.Evaluate(alpha);
            }
            if (AnimationCurve is not null)
            {
                alpha = AnimationCurve.Evaluate(alpha);
            }
            return MathEx.Lerp(@from, to, alpha);
        }

        protected virtual void OnUpdate()
        {
            Update?.Invoke(this);
        }
        protected virtual void OnFinished()
        {
            Finished?.Invoke(this);
            if (FinishedCallback is not null)
            {
                Action cb = FinishedCallback;
                FinishedCallback = null;
                cb();
            }
        }

        public delegate void UpdateHandler(Timeline timeline);
        public delegate void FinishedHandler(Timeline timeline);
    }
}
