// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jih.Unity.Infrastructure.UIElements
{
    public class WindowService
    {
        public IWindowServiceListener? Listener { get; private set; }

        VisualElement? _rootContainer;
        public VisualElement RootContainer => _rootContainer.ThrowIfNull(nameof(RootContainer));

        VisualElement? _modelessConatiner;
        VisualElement ModelessContainer => _modelessConatiner.ThrowIfNull(nameof(ModelessContainer));

        VisualElement? _fullScreenContainer;
        VisualElement FullScreenContainer => _fullScreenContainer.ThrowIfNull(nameof(FullScreenContainer));

        readonly Dictionary<Window, WindowState> _windows = new();

        public WindowService()
        {
        }
        public WindowService(VisualElement rootContainer)
        {
            Initialize(rootContainer);
        }

        public void Initialize(VisualElement rootContainer)
        {
            _rootContainer = rootContainer;

            _rootContainer.pickingMode = PickingMode.Ignore;

            _rootContainer.style.position = Position.Absolute;
            _rootContainer.style.left = 0f;
            _rootContainer.style.right = 0f;
            _rootContainer.style.top = 0f;
            _rootContainer.style.bottom = 0f;

            _modelessConatiner = new VisualElement()
            {
                pickingMode = PickingMode.Ignore,
            };
            _modelessConatiner.style.position = Position.Absolute;
            _modelessConatiner.style.left = 0f;
            _modelessConatiner.style.right = 0f;
            _modelessConatiner.style.top = 0f;
            _modelessConatiner.style.bottom = 0f;
            _rootContainer.Add(_modelessConatiner);

            _fullScreenContainer = new VisualElement()
            {
                pickingMode = PickingMode.Ignore,
            };
            _fullScreenContainer.style.position = Position.Absolute;
            _fullScreenContainer.style.left = 0f;
            _fullScreenContainer.style.right = 0f;
            _fullScreenContainer.style.top = 0f;
            _fullScreenContainer.style.bottom = 0f;
            _rootContainer.Add(_fullScreenContainer);
        }

        public void Update()
        {
            if (Listener is null || !Listener.IsEnabled)
            {
                return;
            }

            foreach (var state in _windows.Values)
            {
                state.WindowHandler.Update();
            }
        }

        public void Show(Window window, Window? owner = null)
        {
            if (_windows.ContainsKey(window))
            {
                if (window.IsModal)
                {
                    throw new InvalidOperationException($"The Window '{window.TitleLabel.text}' already shown as modal. But trying to show modeless.");
                }

                // If equivalent show call, bring the window to front.
                window.Root.BringToFront();
                return;
            }

            if (owner == window)
            {
                throw new InvalidOperationException("Window cannot be an owner of itself.");
            }

            if (window.Service is not null && window.Service != this)
            {
                throw new InvalidOperationException("Window is already shown in another service.");
            }

            if (owner is not null && !_windows.ContainsKey(owner))
            {
                throw new InvalidOperationException("Owner window is not shown.");
            }

            WindowHandler windowHandler = new(this, window);
            windowHandler.Attach();
            TitleBarHandler titleBarHandler = new(this, window);
            titleBarHandler.Attach();

            if (owner is not null)
            {
                owner.OwnedWindowsInternal.Add(window);
                window.Owner = owner;
            }

            VisualElementEx.DisableNonEssentialFocuses(window.Root, includeSelf: true);

            // Start with hidden, and show after a short delay to avoid showing the window in an unexpected location due to the layout not being updated yet.
            window.Root.style.visibility = Visibility.Hidden;

            ModelessContainer.Add(window.Root);

            WindowState state = new(windowHandler, titleBarHandler, null);
            _windows.Add(window, state);
            window.Service = this;

            window.IsModal = false;
            window.IsShown = true;
            window.IsPlaced = false;

            window.OnShown(new WindowShownEventArgs(window));
        }

        public void ShowDialog(Window window, Window? owner = null)
        {
            if (_windows.TryGetValue(window, out WindowState prevState))
            {
                if (!window.IsModal)
                {
                    throw new InvalidOperationException($"The Window '{window.TitleLabel.text}' already shown as modeless. But trying to show modal.");
                }

                // If equivalent show call, bring the window to front.
                prevState.Blocker?.BringToFront();
                window.Root.BringToFront();
                return;
            }

            if (owner == window)
            {
                throw new InvalidOperationException("Window cannot be an owner of itself.");
            }

            if (window.Service is not null && window.Service != this)
            {
                throw new InvalidOperationException("Window is already shown in another service.");
            }

            if (owner is not null && !_windows.ContainsKey(owner))
            {
                throw new InvalidOperationException("Owner window is not shown.");
            }

            WindowHandler windowHandler = new(this, window);
            windowHandler.Attach();
            TitleBarHandler titleBarHandler = new(this, window);
            titleBarHandler.Attach();

            if (owner is not null)
            {
                owner.OwnedWindowsInternal.Add(window);
                window.Owner = owner;
            }
            
            VisualElementEx.DisableNonEssentialFocuses(window.Root, includeSelf: true);

            // Start with hidden.
            window.Root.style.visibility = Visibility.Hidden;

            VisualElement blocker = CreateBlocker();
            if (owner is not null)
            {
                owner.Root.Add(blocker);
                owner.Root.Add(window.Root);
            }
            else
            {
                VisualElement container = FullScreenContainer;
                container.Add(blocker);
                container.Add(window.Root);
            }

            WindowState state = new(windowHandler, titleBarHandler, blocker);
            _windows.Add(window, state);
            window.Service = this;

            window.IsModal = true;
            window.IsShown = true;
            window.IsPlaced = false;

            window.OnShown(new WindowShownEventArgs(window));
        }

        public void Close(Window window)
        {
            Close_Impl(window, WindowCloseReason.UserClosing, closeChildren: true, allowCancel: true);
        }

        bool Close_Impl(Window window, WindowCloseReason reason, bool closeChildren, bool allowCancel)
        {
            if (!_windows.TryGetValue(window, out WindowState state))
            {
                return true;
            }

            {
                WindowClosingEventArgs e = new(window, reason);
                window.OnClosing(ref e);
                if (allowCancel && e.Cancel)
                {
                    return false;
                }
            }

            if (closeChildren)
            {
                List<Window> ownedWindows = window.OwnedWindowsInternal;
                for (int i = ownedWindows.Count - 1; i >= 0; i--)
                {
                    Window ownedWindow = ownedWindows[i];
                    if (!Close_Impl(ownedWindow, WindowCloseReason.OwnerClosing, closeChildren, allowCancel))
                    {
                        return false;
                    }
                }
            }

            state.WindowHandler.Detach();
            state.TitleBarHandler.Detach();

            state.Blocker?.RemoveFromHierarchy();

            if (window.Owner is not null)
            {
                window.Owner.OwnedWindowsInternal.Remove(window);
                window.Owner = null;
            }

            window.Root.RemoveFromHierarchy();

            _windows.Remove(window);
            window.Service = null;

            window.IsModal = false;
            window.IsShown = false;
            window.IsPlaced = false;

            window.OnClosed(new WindowClosedEventArgs(window, reason));

            return true;
        }

        void CloseAllForClear(WindowCloseReason reason)
        {
            foreach (var window in _windows.Keys)
            {
                Close_Impl(window, reason, closeChildren: false, allowCancel: false);
            }
            _windows.Clear();
        }

        public void ClearListener(IWindowServiceListener listener)
        {
            if (Listener != listener)
            {
                Debug.LogWarning("Listener of WindowService is clearing in another context.");
            }
            Listener = null;

            CloseAllForClear(WindowCloseReason.ServiceClosing);
        }
        public void SetListener(IWindowServiceListener listener)
        {
            if (Listener is not null)
            {
                Debug.LogWarning("Listener of WindowService is set without clearing.");
            }
            Listener = listener;

            CloseAllForClear(WindowCloseReason.ServiceClosing);
        }

        internal void OnBringToFrontPerformed(Window window)
        {
            if (!_windows.ContainsKey(window))
            {
                return;
            }
            if (window.IsModal)
            {
                if (window.Owner is not null)
                {
                    OnBringToFrontPerformed(window.Owner);
                }
                return;
            }
            window.Root.BringToFront();
        }

        bool TryPlaceWindow(float windowWidth, float windowHeight, ref Vector2 location)
        {
            float panelWith = RootContainer.resolvedStyle.width;
            float panelHeight = RootContainer.resolvedStyle.height;

            if (float.IsNaN(windowWidth) || float.IsNaN(windowHeight) ||
                float.IsNaN(panelWith) || float.IsNaN(panelHeight))
            {
                // Layout has not been updated yet. So cannot place window location.
                // This will not occur in normal cases.
                return false;
            }

            if (location.x < 0f)
            {
                location.x = 0f;
            }
            else if (location.x > panelWith - windowWidth)
            {
                location.x = panelWith - windowWidth;
            }

            if (location.y < 0f)
            {
                location.y = 0f;
            }
            else if (location.y > panelHeight - windowHeight)
            {
                location.y = panelHeight - windowHeight;
            }

            return true;
        }

        bool PlaceByStartupLocation(Window window)
        {
            if (window.WindowStartupLocation is WindowStartupLocation.Manual)
            {
                return true;
            }

            float selfWidth = window.Root.resolvedStyle.width;
            float selfHeight = window.Root.resolvedStyle.height;
            if (float.IsNaN(selfWidth) || float.IsNaN(selfHeight))
            {
                // Layout has not been updated yet.
                return false;
            }

            float parentX, parentY;
            float parentWidth, parentHeight;
            switch (window.WindowStartupLocation)
            {
                case WindowStartupLocation.CenterScreen:
                    parentX = 0f;
                    parentY = 0f;
                    parentWidth = RootContainer.resolvedStyle.width;
                    parentHeight = RootContainer.resolvedStyle.height;
                    break;

                case WindowStartupLocation.CenterOwner:
                    if (window.Owner is null)
                    {
                        goto case WindowStartupLocation.CenterScreen;
                    }

                    parentX = window.Owner.Root.resolvedStyle.left;
                    parentY = window.Owner.Root.resolvedStyle.top;
                    parentWidth = window.Owner.Root.resolvedStyle.width;
                    parentHeight = window.Owner.Root.resolvedStyle.height;
                    break;

                default: throw new NotImplementedException();
            }

            if (float.IsNaN(parentX) || float.IsNaN(parentY) ||
                float.IsNaN(parentWidth) || float.IsNaN(parentHeight))
            {
                // Layout has not been updated yet.
                return false;
            }

            float x = parentX + (parentWidth - selfWidth) * 0.5f;
            float y = parentY + (parentHeight - selfHeight) * 0.5f;
            window.Root.style.left = x;
            window.Root.style.top = y;

            return true;
        }

        class WindowHandler
        {
            public WindowService Service { get; }
            public Window Window { get; }

            int _visibleCounter = 0;
            bool _isVisible = false;

            public WindowHandler(WindowService service, Window window)
            {
                Service = service;
                Window = window;
            }

            public void Update()
            {
                UpdateVisibility();
            }

            void UpdateVisibility()
            {
                if (_isVisible)
                {
                    return;
                }

                if (_visibleCounter < VisibleDelayFrameCount)
                {
                    _visibleCounter++;
                    return;
                }

                if (Service.PlaceByStartupLocation(Window))
                {
                    // Actually show the window after the location is placed.
                    Window.Root.style.visibility = Visibility.Visible;

                    Window.IsPlaced = true;

                    Window.OnPlaced(new WindowPlacedEventArgs(Window));

                    _isVisible = true;
                }
            }

            public void Attach()
            {
                Window.Root.RegisterCallback<PointerDownEvent>(WindowPointerDown, TrickleDown.TrickleDown);
            }
            public void Detach()
            {
                Window.Root.UnregisterCallback<PointerDownEvent>(WindowPointerDown, TrickleDown.TrickleDown);
            }

            void WindowPointerDown(PointerDownEvent e)
            {
                if (!Window.IsPlaced)
                {
                    return;
                }
                Service.OnBringToFrontPerformed(Window);
            }
        }

        class TitleBarHandler
        {
            public WindowService Service { get; }
            public Window Window { get; }

            readonly VisualElement _titleBar;

            bool _isDragging = false;
            Vector2 _startedPointerLocation, _startedWindowLocation;

            public TitleBarHandler(WindowService service, Window window)
            {
                Service = service;
                Window = window;

                _titleBar = window.TitleBar;
            }

            public void Attach()
            {
                _titleBar.RegisterCallback<PointerDownEvent>(OnPointerDown);
                _titleBar.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                _titleBar.RegisterCallback<PointerUpEvent>(OnPointerUp);
                _titleBar.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
                _titleBar.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            }
            public void Detach()
            {
                _titleBar.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                _titleBar.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                _titleBar.UnregisterCallback<PointerUpEvent>(OnPointerUp);
                _titleBar.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
                _titleBar.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            }

            void OnPointerDown(PointerDownEvent e)
            {
                if (e.target != _titleBar)
                {
                    return;
                }
                if (e.button != 0)
                {
                    return;
                }
                if (!Window.IsPlaced)
                {
                    return;
                }

                if (!_titleBar.HasPointerCapture(e.pointerId))
                {
                    _titleBar.CapturePointer(e.pointerId);
                }

                float x = Window.Root.resolvedStyle.left;
                float y = Window.Root.resolvedStyle.top;
                if (float.IsNaN(x) || float.IsNaN(y))
                {
                    // Layout has not been updated yet. Cannot get proper window location. So cannot start dragging.
                    // This will not occur in normal cases.
                    return;
                }

                _isDragging = true;
                _startedPointerLocation = e.position;
                _startedWindowLocation = new Vector2(x, y);
            }
            void OnPointerMove(PointerMoveEvent e)
            {
                if (!_titleBar.HasPointerCapture(e.pointerId))
                {
                    return;
                }
                if (!_isDragging)
                {
                    return;
                }

                float width = _titleBar.resolvedStyle.width;
                float height = _titleBar.resolvedStyle.height;

                Vector2 pointerDelta = (Vector2)e.position - _startedPointerLocation;

                Vector2 location = _startedWindowLocation + pointerDelta;

                if (!Service.TryPlaceWindow(width, height, ref location))
                {
                    return;
                }

                Window.Root.style.left = location.x;
                Window.Root.style.top = location.y;
            }
            void OnPointerUp(PointerUpEvent e)
            {
                if (!_titleBar.HasPointerCapture(e.pointerId))
                {
                    return;
                }

                if (e.button != 0)
                {
                    return;
                }

                _titleBar.ReleasePointer(e.pointerId);

                _isDragging = false;
            }
            void OnPointerCancel(PointerCancelEvent e)
            {
                _titleBar.ReleasePointer(e.pointerId);

                _isDragging = false;
            }
            void OnPointerCaptureOut(PointerCaptureOutEvent e)
            {
                _isDragging = false;
            }
        }

        static VisualElement CreateBlocker()
        {
            VisualElement blocker = new()
            {
                pickingMode = PickingMode.Position,
                focusable = false,
            };
            blocker.AddToClassList(BlockerClassName);

            blocker.style.position = Position.Absolute;
            blocker.style.left = 0f;
            blocker.style.right = 0f;
            blocker.style.top = 0f;
            blocker.style.bottom = 0f;

            blocker.RegisterCallback<PointerDownEvent>(Block);
            blocker.RegisterCallback<PointerUpEvent>(Block);
            blocker.RegisterCallback<WheelEvent>(Block);
            blocker.RegisterCallback<PointerCancelEvent>(Block);
            blocker.RegisterCallback<PointerCaptureOutEvent>(Block);

            return blocker;
        }

        static void Block(PointerDownEvent e)
        {
            e.StopPropagation();
        }
        static void Block(PointerUpEvent e)
        {
            e.StopPropagation();
        }
        static void Block(WheelEvent e)
        {
            e.StopPropagation();
        }
        static void Block(PointerCancelEvent e)
        {
            e.StopPropagation();
        }
        static void Block(PointerCaptureOutEvent e)
        {
            e.StopPropagation();
        }

        readonly struct WindowState
        {
            public readonly WindowHandler WindowHandler;
            public readonly TitleBarHandler TitleBarHandler;
            public readonly VisualElement? Blocker;

            public WindowState(WindowHandler handler, TitleBarHandler titleBarHandler, VisualElement? blocker)
            {
                WindowHandler = handler;
                TitleBarHandler = titleBarHandler;
                Blocker = blocker;
            }
        }

        const int VisibleDelayFrameCount = 3;

        public const string BlockerClassName = "infrastructure-window-blocker";
    }

    public interface IWindowServiceListener
    {
        bool IsEnabled { get; }
    }
}
