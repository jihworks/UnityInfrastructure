// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

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

        public static IObservable<T> Flatten<T>(this IObservable<IObservable<T>> source)
        {
            return new FlattenObservable<T>(source);
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
