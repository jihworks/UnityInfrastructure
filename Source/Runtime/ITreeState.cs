// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Infrastructure.Runtime
{
    public interface ITreeState : IState
    {
        void Update();
        void FixedUpdate();
        void LateUpdate();

        void RouteEvent<THandler, TDispatcher>(in TDispatcher dispatcher, ref bool isHandled)
            where THandler : class
            where TDispatcher : struct, IEventDispatcher<THandler>;
    }

    public abstract class BaseTreeState<TSelf, TOwner, TChildState> : ITreeState
        where TSelf : IState
        where TOwner : IStateOwner<TSelf>
        where TChildState : class, ITreeState
    {
        public TOwner Owner { get; }

        StateStorage<TChildState> _state;
        public TChildState? CurrentState { get => _state.Current; protected set => _state.Current = value; }

        protected BaseTreeState(TOwner owner)
        {
            Owner = owner;
            _state = new StateStorage<TChildState>(GetType().Name);
        }

        public virtual void Begin(IState? prev)
        {
        }
        public virtual void End(IState? next)
        {
            CurrentState = null;
        }

        public virtual void Update()
        {
            CurrentState?.Update();
        }
        public virtual void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }
        public virtual void LateUpdate()
        {
            CurrentState?.LateUpdate();
        }

        public virtual void RouteEvent<THandler, TDispatcher>(in TDispatcher dispatcher, ref bool isHandled)
            where THandler : class
            where TDispatcher : struct, IEventDispatcher<THandler>
        {
            if (isHandled)
            {
                return;
            }

            if (CurrentState is not null)
            {
                CurrentState.RouteEvent<THandler, TDispatcher>(in dispatcher, ref isHandled);

                if (isHandled)
                {
                    return;
                }
            }

            if (this is THandler handler)
            {
                dispatcher.Dispatch(handler, ref isHandled);
            }
        }
    }

    public sealed class EmptyTreeState : ITreeState
    {
        private EmptyTreeState()
        {
        }

        void IState.Begin(IState? prev)
        {
        }
        void IState.End(IState? next)
        {
        }

        void ITreeState.Update()
        {
        }
        void ITreeState.FixedUpdate()
        {
        }
        void ITreeState.LateUpdate()
        {
        }

        void ITreeState.RouteEvent<THandler, TDispatcher>(in TDispatcher dispatcher, ref bool isHandled)
        {
        }
    }
}
