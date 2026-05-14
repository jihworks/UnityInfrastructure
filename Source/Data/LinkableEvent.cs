// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Runtime;
using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Data
{
    public class LinkableEvent<T> : BaseThreadCriticalObject, IObservable<T>, IObserver<T>
    {
        readonly List<IObserver<T>> _observers = new();
        readonly List<IObserver<T>> _invokeBuffer = new();

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            CheckThread();

            _observers.Add(observer);
            // Do not call OnNext() this time.

            Subscription subscription = _subscriptionPool.Get();
            subscription.Set(this, observer);
            return subscription;
        }

        public void OnNext(T value)
        {
            CheckThread();

            _invokeBuffer.Clear();
            _invokeBuffer.AddRange(_observers);
            for (int i = 0; i < _invokeBuffer.Count; i++)
            {
                _invokeBuffer[i].OnNext(value);
            }
            _invokeBuffer.Clear();
        }

        void IObserver<T>.OnCompleted()
        {
        }
        void IObserver<T>.OnError(Exception error)
        {
        }

        void Remove(IObserver<T> observer)
        {
            CheckThread();

            _observers.Remove(observer);
        }

        class Subscription : IDisposable
        {
            public LinkableEvent<T> Owner { get; private set; } = null!;
            public IObserver<T> Observer { get; private set; } = null!;

            internal void Set(LinkableEvent<T> owner, IObserver<T> observer)
            {
                Owner = owner;
                Observer = observer;
            }

            internal void Clear()
            {
                Owner = null!;
                Observer = null!;
            }

            public void Dispose()
            {
                Owner.Remove(Observer);

                _subscriptionPool.Release(this);
            }
        }

        class SubscriptionPool : ObjectPool<Subscription>
        {
            public SubscriptionPool() : base(isThreadSafe: true)
            {
            }

            protected override void Activate(Subscription obj)
            {
                base.Activate(obj);
                obj.Clear();
            }
            protected override void Deactivate(Subscription obj)
            {
                obj.Clear();
                base.Deactivate(obj);
            }
        }

        static readonly SubscriptionPool _subscriptionPool = new();
    }
}
