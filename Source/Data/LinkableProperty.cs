// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jih.Unity.Infrastructure.Data
{
    public class LinkableProperty<T> : IObservable<T>
    {
        T _value = default!;
        public T Value
        {
            get
            {
                CheckThread();
                return _value;
            }
            set
            {
                CheckThread();

                if (EqualityComparer<T>.Default.Equals(value!, _value!))
                {
                    return;
                }
                _value = value;

                _invokeBuffer.Clear();
                _invokeBuffer.AddRange(_observers);
                for (int i = 0; i < _invokeBuffer.Count; i++)
                {
                    _invokeBuffer[i].OnNext(value!);
                }
                _invokeBuffer.Clear();
            }
        }

        readonly List<IObserver<T>> _observers = new();
        readonly List<IObserver<T>> _invokeBuffer = new();

        readonly int _ownerThreadId;

        public LinkableProperty()
        {
            _ownerThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            CheckThread();

            _observers.Add(observer);
            observer.OnNext(_value);

            Subscription subscription = _subscriptionPool.Get();
            subscription.Set(this, observer);
            return subscription;
        }

        void Remove(IObserver<T> observer)
        {
            CheckThread();

            _observers.Remove(observer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != _ownerThreadId)
            {
                throw new InvalidOperationException("Owner thread mismatched.");
            }
        }

        class Subscription : IDisposable
        {
            public LinkableProperty<T> Owner { get; private set; } = null!;
            public IObserver<T> Observer { get; private set; } = null!;

            internal void Set(LinkableProperty<T> owner, IObserver<T> observer)
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
