// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.Data
{
    class SelectObservable<TSource, TResult> : IObservable<TResult>
    {
        public IObservable<TSource> Source { get; }
        public Func<TSource, TResult> Selector { get; }

        public SelectObservable(IObservable<TSource> source, Func<TSource, TResult> selector)
        {
            Source = source;
            Selector = selector;
        }

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            SelectObserver<TSource, TResult> selectObserver = new(observer, Selector);
            return Source.Subscribe(selectObserver);
        }
    }

    class SelectObserver<TSource, TResult> : IObserver<TSource>
    {
        public IObserver<TResult> Downstream { get; }
        public Func<TSource, TResult> Selector { get; }

        public SelectObserver(IObserver<TResult> downstream, Func<TSource, TResult> selector)
        {
            Downstream = downstream;
            Selector = selector;
        }

        public void OnNext(TSource value)
        {
            TResult result = Selector(value);
            Downstream.OnNext(result);
        }

        void IObserver<TSource>.OnCompleted()
        {
        }
        void IObserver<TSource>.OnError(Exception error)
        {
        }
    }
}
