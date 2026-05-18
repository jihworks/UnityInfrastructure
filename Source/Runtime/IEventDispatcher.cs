// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Infrastructure.Runtime
{
    public interface IEventDispatcher<THandler> where THandler : class
    {
        void Dispatch(THandler handler, ref bool isHandled);
    }
}
