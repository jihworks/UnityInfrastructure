// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Collections;
using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Data
{
    public class LinkableDictionaryTracker<TKey, TValue, TEvent> : BaseThreadCriticalObject, IObservable<(TKey Key, TEvent EventData)>, IDisposable where TKey : notnull
    {
        public IReadOnlyLinkableDictionary<TKey, TValue> Target { get; }
        public Func<TValue, IObservable<TEvent>> EventSelector { get; }

        IDisposable? _dictSubscription;
        readonly Dictionary<TKey, IDisposable> _itemSubscriptions = new();

        readonly LinkableEvent<(TKey, TEvent)> _onItemEvent = new();

        public LinkableDictionaryTracker(IReadOnlyLinkableDictionary<TKey, TValue> target, Func<TValue, IObservable<TEvent>> eventSelector)
        {
            Target = target;
            EventSelector = eventSelector;

            _dictSubscription = target.OnChanged.Link(OnDictChanged);

            foreach (var pair in target)
            {
                HookItem(pair.Key, pair.Value);
            }
        }

        void OnDictChanged(DictionaryChangeArgs<TKey, TValue> args)
        {
            CheckThread();

            switch (args.Type)
            {
                case DictionaryChangeType.Add:
                    HookItem(args.Key, args.Value);
                    break;

                case DictionaryChangeType.Remove:
                    UnhookItem(args.Key);
                    break;

                case DictionaryChangeType.Update:
                    UnhookItem(args.Key);
                    HookItem(args.Key, args.Value);
                    break;

                case DictionaryChangeType.Clear:
                    ClearAllHooks();
                    break;
            }
        }

        void HookItem(TKey key, TValue item)
        {
            IObservable<TEvent> itemEventObservable = EventSelector(item);

            IDisposable subscription = itemEventObservable.Link(eventData =>
            {
                _onItemEvent.OnNext((key, eventData));
            });
            _itemSubscriptions[key] = subscription;
        }

        void UnhookItem(TKey key)
        {
            if (_itemSubscriptions.TryGetValue(key, out IDisposable subscription))
            {
                subscription.Dispose();
                _itemSubscriptions.Remove(key);
            }
        }

        void ClearAllHooks()
        {
            foreach (var subscription in _itemSubscriptions.Values)
            {
                subscription.Dispose();
            }
            _itemSubscriptions.Clear();
        }

        public IDisposable Subscribe(IObserver<(TKey Key, TEvent EventData)> observer)
        {
            CheckThread();

            return ((IObservable<(TKey, TEvent)>)_onItemEvent).Subscribe(observer);
        }

        public void Dispose()
        {
            CheckThread();

            DisposableEx.Dispose(ref _dictSubscription);
            ClearAllHooks();
        }
    }
}
