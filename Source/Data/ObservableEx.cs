// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Data
{
    public static class ObservableEx
    {
        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new WhereObservable<T>(source, predicate);
        }
        public static IObservable<TResult> Select<TSource, TResult>(this IObservable<TSource> source, Func<TSource, TResult> predicate)
        {
            return new SelectObservable<TSource, TResult>(source, predicate);
        }
        public static IObservable<Empty> SelectEmpty<TSource>(this IObservable<TSource> source)
        {
            return new SelectObservable<TSource, Empty>(source, _ => Data.Empty.Default);
        }

        public static IObservable<T> Flatten<T>(this IObservable<IObservable<T>> source)
        {
            return new FlattenObservable<T>(source);
        }

        public static IObservable<T> Merge<T>(params IObservable<T>[] sources)
        {
            return new MergeObservable<T>(sources);
        }
        public static IObservable<T> Merge<T>(IReadOnlyList<IObservable<T>> sources)
        {
            return new MergeObservable<T>(sources);
        }

        public static IDisposable Link<T>(this IObservable<T> source, Action<T> action)
        {
            return source.Subscribe(new ActionObserver<T>(action));
        }

        public static IObservable<T> Empty<T>()
        {
            return EmptyObsavable<T>.Instance;
        }
    }

    class PassThroughObserver<T> : IObserver<T>
    {
        public IObserver<T> Downstream { get; }

        public PassThroughObserver(IObserver<T> downstream)
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

    class ActionObserver<T> : IObserver<T>
    {
        public Action<T> Action { get; }

        public ActionObserver(Action<T> action)
        {
            Action = action;
        }

        public void OnNext(T value)
        {
            Action(value);
        }

        void IObserver<T>.OnCompleted()
        {
        }
        void IObserver<T>.OnError(Exception error)
        {
        }
    }

    class EmptyObsavable<T> : IObservable<T>
    {
        public static readonly EmptyObsavable<T> Instance = new();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return EmptySubscription.Instance;
        }
    }
    class EmptySubscription : IDisposable
    {
        public static readonly EmptySubscription Instance = new();

        public void Dispose()
        {
        }
    }
}
