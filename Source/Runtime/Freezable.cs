// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jih.Unity.Infrastructure.Runtime
{
    public abstract class Freezable
    {
        public static void Release<TFreezable>(ref FreezableRef<TFreezable> r) where TFreezable : Freezable
        {
            if (!r.TryGetTarget(out TFreezable? target))
            {
                return;
            }
            target.Release();
            r = default;
        }

        public IFreezablePool? Pool { get; private set; }

        public int OwnerThreadId { get; private set; }
        public uint Generation { get; private set; }

        public bool IsFrozen { get; private set; }
        public bool IsInPool { get; private set; }

        public void Freeze()
        {
            if (IsFrozen)
            {
                return;
            }

            if (IsInPool)
            {
                throw new InvalidOperationException($"'{GetType()}' is in pool but doing '{nameof(Freeze)}'.");
            }
            if (Thread.CurrentThread.ManagedThreadId != OwnerThreadId)
            {
                throw new InvalidOperationException($"'{GetType()}' denied '{nameof(Freeze)}' because accessing thread mismatched.");
            }

            DoBeforeFreeze();
            IsFrozen = true;
        }

        public void Release()
        {
            if (Pool is null)
            {
                throw new InvalidOperationException($"Cannot release because '{GetType()}' wast not created from pool.");
            }
            Pool.Release(this);
        }

        protected virtual void DoBeforeFreeze()
        {
        }

        protected abstract void ResetFreezable();

        protected void CheckFreezableAccess([CallerMemberName] string context = "")
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException($"'{GetType()}' is frozen but doing '{context}'.");
            }
            if (IsInPool)
            {
                throw new InvalidOperationException($"'{GetType()}' is in pool but doing '{context}'.");
            }
            if (Thread.CurrentThread.ManagedThreadId != OwnerThreadId)
            {
                throw new InvalidOperationException($"'{GetType()}' denied access because accessing thread mismatched.");
            }
        }

        internal readonly struct Proxy
        {
            public readonly Freezable Target;

            public IFreezablePool? Pool { get => Target.Pool; set => Target.Pool = value; }
            public int OwnerThreadId { get => Target.OwnerThreadId; set => Target.OwnerThreadId = value; }
            public uint Generation { get => Target.Generation; set => Target.Generation = value; }
            public bool IsFrozen { get => Target.IsFrozen; set => Target.IsFrozen = value; }
            public bool IsInPool { get => Target.IsInPool; set => Target.IsInPool = value; }

            public Proxy(Freezable target)
            {
                Target = target;
            }

            public void ResetFreezable()
            {
                Target.ResetFreezable();
            }
        }
    }

    public interface IFreezablePool
    {
        void Release(Freezable freezable);
    }

    public class FreezablePool<TFreezable> : ObjectPool<TFreezable>, IFreezablePool where TFreezable : Freezable, new()
    {
        public int OwnerThreadId { get; private set; }

        public FreezablePool() : base(DefaultInitialCollectionCapacity, isThreadSafe: false)
        {
            OwnerThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public new FreezableRef<TFreezable> Get()
        {
            return new FreezableRef<TFreezable>(base.Get());
        }

        protected override TFreezable Create()
        {
            Check();

            TFreezable result = base.Create();
            new Freezable.Proxy(result)
            {
                Pool = this,
                OwnerThreadId = OwnerThreadId,
                Generation = 1u,
            };

            return result;
        }
        protected override void Destroy(TFreezable obj)
        {
            Check();
            base.Destroy(obj);
        }

        protected override void Activate(TFreezable obj)
        {
            Check();

            new Freezable.Proxy(obj)
            {
                IsFrozen = false,
                IsInPool = false,
            };
        }
        protected override void Deactivate(TFreezable obj)
        {
            Check();

            Freezable.Proxy proxy = new(obj);
            proxy.ResetFreezable();
            proxy.IsFrozen = false;
            proxy.IsInPool = true;
            proxy.Generation = Math.Max(1u, unchecked(proxy.Generation + 1u));
        }

        void Check([CallerMemberName] string context = "")
        {
            if (Thread.CurrentThread.ManagedThreadId != OwnerThreadId)
            {
                throw new InvalidOperationException($"Cannot {context} '{typeof(TFreezable)}' because getting thread mismatched.");
            }
        }

        void IFreezablePool.Release(Freezable freezable)
        {
            Release((TFreezable)freezable);
        }
    }

    public readonly struct FreezableRef<TFreezable> where TFreezable : Freezable
    {
        readonly TFreezable _target;
        public readonly TFreezable Target
        {
            get
            {
                if (!CanAccess)
                {
                    throw new InvalidOperationException($"Cannot ref target '{typeof(TFreezable)}' because generation mismatched.");
                }
                return _target;
            }
        }

        public readonly uint Generation;

#if INFRASTRUCTURE_USE_NULL_STATES
        [MemberNotNullWhen(true, nameof(Target))]
#endif
        public readonly bool CanAccess => _target is not null && _target.Generation == Generation;

        public FreezableRef(TFreezable target)
        {
            _target = target;
            Generation = target.Generation;
        }

        public readonly bool TryGetTarget([NotNullWhen(true)] out TFreezable? target)
        {
            if (CanAccess)
            {
                target = _target;
                return true;
            }
            target = default;
            return false;
        }

        public static explicit operator TFreezable(FreezableRef<TFreezable> r)
        {
            return r.Target;
        }
    }
}
