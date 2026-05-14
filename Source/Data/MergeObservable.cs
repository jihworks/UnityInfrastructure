// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Data
{
    class MergeObservable<T> : IObservable<T>
    {
        public IReadOnlyList<IObservable<T>> Sources { get; }

        public MergeObservable(IReadOnlyList<IObservable<T>> sources)
        {
            Sources = sources;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            MergeObserver<T> mergeObserver = new(observer);

            IDisposable[] subscriptions = new IDisposable[Sources.Count];

            for (int i = 0; i < Sources.Count; i++)
            {
                subscriptions[i] = Sources[i].Subscribe(mergeObserver);
            }

            return new MergeDisposable(subscriptions);
        }
    }

    class MergeObserver<T> : IObserver<T>
    {
        public IObserver<T> Downstream { get; }

        public MergeObserver(IObserver<T> downstream)
        {
            Downstream = downstream;
        }

        public void OnNext(T value)
        {
            Downstream.OnNext(value);
        }

        void IObserver<T>.OnCompleted()
        {
        }
        void IObserver<T>.OnError(Exception error)
        {
        }
    }

    class MergeDisposable : IDisposable
    {
        readonly IDisposable?[] _subscriptions;

        public MergeDisposable(IDisposable?[] subscriptions)
        {
            _subscriptions = subscriptions;
        }

        public void Dispose()
        {
            for (int i = 0; i < _subscriptions.Length; i++)
            {
                DisposableEx.Dispose(ref _subscriptions[i]);
            }
        }
    }
}
