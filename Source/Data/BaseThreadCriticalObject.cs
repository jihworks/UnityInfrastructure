// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jih.Unity.Infrastructure.Data
{
    public abstract class BaseThreadCriticalObject
    {
        readonly int _ownerThreadId;

        protected BaseThreadCriticalObject()
        {
            _ownerThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CheckThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != _ownerThreadId)
            {
                throw new InvalidOperationException($"Owner thread is mismatched for object '{GetType()}'.");
            }
        }
    }
}
