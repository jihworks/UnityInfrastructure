// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.Data
{
    class FlattenObservable<T> : IObservable<T>
    {
        public IObservable<IObservable<T>> Source { get; }

        public FlattenObservable(IObservable<IObservable<T>> source)
        {
            Source = source;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Source.Subscribe(new FlattenObserver<T>(observer));
        }
    }

    class FlattenObserver<T> : IObserver<IObservable<T>>
    {
        public IObserver<T> Downstream { get; }

        IDisposable? _currentSubscription;

        public FlattenObserver(IObserver<T> downstream)
        {
            Downstream = downstream;
        }

        public void OnNext(IObservable<T> value)
        {
            _currentSubscription?.Dispose();

            PassThroughObserver<T> passThroughObserver = new(Downstream);
            _currentSubscription = value.Subscribe(passThroughObserver);
        }

        void IObserver<IObservable<T>>.OnCompleted()
        {
        }
        void IObserver<IObservable<T>>.OnError(Exception error)
        {
        }
    }
}
