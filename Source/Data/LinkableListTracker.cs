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
    public class LinkableListTracker<TItem, TEvent> : BaseThreadCriticalObject, IObservable<(TItem Item, TEvent EventData)>, IDisposable
    {
        public IReadOnlyLinkableList<TItem> Target { get; }
        public Func<TItem, IObservable<TEvent>> EventSelector { get; }

        IDisposable? _listSubscription;
        readonly List<IDisposable> _itemSubscriptions = new();

        readonly LinkableEvent<(TItem, TEvent)> _onItemEvent = new();

        public LinkableListTracker(IReadOnlyLinkableList<TItem> target, Func<TItem, IObservable<TEvent>> eventSelector)
        {
            Target = target;
            EventSelector = eventSelector;
            _listSubscription = target.OnChanged.Link(OnListChanged);

            for (int i = 0; i < target.Count; i++)
            {
                _itemSubscriptions.Add(CreateHook(target[i]));
            }
        }

        void OnListChanged(ListChangeArgs<TItem> args)
        {
            CheckThread();

            switch (args.Type)
            {
                case ListChangeType.Add:
                    _itemSubscriptions.Insert(args.Index, CreateHook(args.Value));
                    break;

                case ListChangeType.Remove:
                    _itemSubscriptions[args.Index].Dispose();
                    _itemSubscriptions.RemoveAt(args.Index);
                    break;

                case ListChangeType.Replace:
                    _itemSubscriptions[args.Index].Dispose();
                    _itemSubscriptions[args.Index] = CreateHook(args.Value);
                    break;

                case ListChangeType.Clear:
                    ClearAllHooks();
                    break;
            }
        }

        IDisposable CreateHook(TItem item)
        {
            return EventSelector(item).Link(eventData =>
            {
                _onItemEvent.OnNext((item, eventData));
            });
        }

        void ClearAllHooks()
        {
            for (int i = 0; i < _itemSubscriptions.Count; i++)
            {
                _itemSubscriptions[i].Dispose();
            }
            _itemSubscriptions.Clear();
        }

        public IDisposable Subscribe(IObserver<(TItem Item, TEvent EventData)> observer)
        {
            CheckThread();
            return ((IObservable<(TItem, TEvent)>)_onItemEvent).Subscribe(observer);
        }

        public void Dispose()
        {
            CheckThread();

            DisposableEx.Dispose(ref _listSubscription);
            ClearAllHooks();
        }
    }
}
