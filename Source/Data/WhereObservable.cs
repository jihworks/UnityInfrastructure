// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.Data
{
    class WhereObservable<T> : IObservable<T>
    {
        public IObservable<T> Source { get; }
        public Func<T, bool> Predicate { get; }

        public WhereObservable(IObservable<T> source, Func<T, bool> predicate)
        {
            Source = source;
            Predicate = predicate;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            WhereObserver<T> whereObserver = new(observer, Predicate);
            return Source.Subscribe(whereObserver);
        }
    }

    class WhereObserver<T> : IObserver<T>
    {
        public IObserver<T> Downstream { get; }
        public Func<T, bool> Predicate { get; }

        public WhereObserver(IObserver<T> downstream, Func<T, bool> predicate)
        {
            Downstream = downstream;
            Predicate = predicate;
        }

        public void OnNext(T value)
        {
            if (Predicate(value))
            {
                Downstream.OnNext(value);
            }
        }

        void IObserver<T>.OnCompleted()
        {
        }
        void IObserver<T>.OnError(Exception error)
        {
        }
    }
}
