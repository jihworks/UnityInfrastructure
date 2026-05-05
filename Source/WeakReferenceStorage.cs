// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure
{
    public struct WeakReferenceStorage<T> where T : class
    {
        WeakReference<T>? _ref;

        public WeakReferenceStorage(T target)
        {
            _ref = new WeakReference<T>(target);
        }

        public void Set(T? target)
        {
            if (target is null)
            {
                Clear();
                return;
            }

            if (_ref is not null)
            {
                _ref.SetTarget(target);
            }
            else
            {
                _ref = new WeakReference<T>(target);
            }
        }

        public T? Get()
        {
            if (_ref is null)
            {
                return null;
            }
            if (!_ref.TryGetTarget(out T target))
            {
                _ref = null;
                return null;
            }
            return target;
        }

        public void Clear()
        {
            _ref = null;
        }
    }
}
