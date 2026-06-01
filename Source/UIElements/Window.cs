// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Jih.Unity.Infrastructure.UIElements
{
    public class Window
    {
        public WindowService? Service { get; internal set; }

        public Window? Owner { get; internal set; }

        internal List<Window> OwnedWindowsInternal { get; } = new List<Window>();
        public IReadOnlyList<Window> OwnedWindows => OwnedWindowsInternal;

        public bool IsModal { get; internal set; }
        public bool IsShown { get; internal set; }
        public bool IsPlaced { get; internal set; }

        public WindowStartupLocation WindowStartupLocation { get; internal set; }

        public VisualElement Root { get; }

        public VisualElement TitleBar { get; }
        public VisualElement ContentRoot { get; }

        public Label TitleLabel { get; }
        public Button MinimizeButton { get; }
        public Button CloseButton { get; }

        bool _isMinimized;
        public bool IsMinimized
        {
            get => _isMinimized;
            set
            {
                if (_isMinimized == value)
                {
                    return;
                }
                _isMinimized = value;
                OnIsMinimizedChanged();
            }
        }

        public Window()
        {
            Root = new VisualElement()
            {
                name = RootElementName,
            };

            TitleBar = new VisualElement()
            {
                name = TitleBarElementName,
            };
            Root.Add(TitleBar);

            TitleLabel = new Label()
            {
                name = TitleLabelElementName,
            };
            TitleBar.Add(TitleLabel);

            MinimizeButton = new Button()
            {
                name = MinimizeButtonElementName,
            };
            TitleBar.Add(MinimizeButton);

            CloseButton = new Button()
            {
                name = CloseButtonElementName,
            };
            TitleBar.Add(CloseButton);

            ContentRoot = new VisualElement()
            {
                name = ContentRootElementName,
            };
            Root.Add(ContentRoot);

            InitializeComponents();
        }

        public Window(VisualTreeAsset asset) : this(asset.CloneTree())
        {
        }
        public Window(VisualElement root)
        {
            Root = root.QT(RootElementName);
            Root.RemoveFromHierarchy();

            TitleBar = Root.QT(TitleBarElementName);
            TitleLabel = Root.QT<Label>(TitleLabelElementName);
            MinimizeButton = Root.QT<Button>(MinimizeButtonElementName);
            CloseButton = Root.QT<Button>(CloseButtonElementName);
            ContentRoot = Root.QT(ContentRootElementName);

            InitializeComponents();
        }

        void InitializeComponents()
        {
            Root.AddToClassList(RootClassName);

            TitleBar.pickingMode = PickingMode.Position;
            TitleBar.AddToClassList(TitleBarClassName);

            TitleLabel.pickingMode = PickingMode.Ignore;
            TitleLabel.AddToClassList(TitleLabelClassName);

            MinimizeButton.AddToClassList(MinimizeButtonClassName);
            MinimizeButton.clicked += OnMinimizeButtonClicked;

            CloseButton.AddToClassList(CloseButtonClassName);
            CloseButton.clicked += OnCloseButtonClicked;

            ContentRoot.AddToClassList(ContentRootClassName);

            UpdateMinimizedState();
        }

        void UpdateMinimizedState()
        {
            MinimizeButton.EnableInClassList(MinimizeButtonMinimizedClassName, IsMinimized);

            ContentRoot.style.display = IsMinimized ? DisplayStyle.None : DisplayStyle.Flex;
        }

        protected virtual void OnMinimizeButtonClicked()
        {
            if (!IsPlaced)
            {
                return;
            }
            IsMinimized = !IsMinimized;
        }

        protected virtual void OnCloseButtonClicked()
        {
            if (!IsPlaced)
            {
                return;
            }
            Service?.Close(this);
        }

        protected virtual void OnIsMinimizedChanged()
        {
            UpdateMinimizedState();
        }

        /// <summary>
        /// Called when the Window is registered to the hierarchy but not visible to user yet.
        /// </summary>
        /// <remarks>
        /// The UI Toolkit needs some frames to calculate the layout. Therefore, the Window is not visible immediately.<br/>
        /// However, the elements are actually interactable.<br/>
        /// Use <see cref="IsPlaced"/> to check the state.
        /// </remarks>
        protected internal virtual void OnShown(WindowShownEventArgs e)
        {
        }
        /// <summary>
        /// Called when layout of the Window had been calculated and the Window is placed. The Window is now visible to user at this moment.
        /// </summary>
        /// <remarks>
        /// Use <see cref="IsPlaced"/> to check the state.
        /// </remarks>
        protected internal virtual void OnPlaced(WindowPlacedEventArgs e)
        {
        }

        protected internal virtual void OnClosing(ref WindowClosingEventArgs e)
        {
        }
        protected internal virtual void OnClosed(WindowClosedEventArgs e)
        {
        }

        public const string RootElementName = "infrastructure-window";
        public const string TitleBarElementName = "infrastructure-window__title-bar";
        public const string TitleLabelElementName = "infrastructure-window__title-label";
        public const string MinimizeButtonElementName = "infrastructure-window__minimize-button";
        public const string CloseButtonElementName = "infrastructure-window__close-button";
        public const string ContentRootElementName = "infrastructure-window__content-root";

        public const string RootClassName = "infrastructure-window";
        public const string TitleBarClassName = "infrastructure-window__title-bar";
        public const string TitleLabelClassName = "infrastructure-window__title-label";
        public const string MinimizeButtonClassName = "infrastructure-window__minimize-button";
        public const string MinimizeButtonMinimizedClassName = "infrastructure-window__minimize-button--minimized";
        public const string CloseButtonClassName = "infrastructure-window__close-button";
        public const string ContentRootClassName = "infrastructure-window__content-root";
    }

    public enum WindowStartupLocation
    {
        Manual,
        CenterScreen,
        CenterOwner,
    }

    public readonly struct WindowShownEventArgs
    {
        public readonly Window Window;

        public WindowShownEventArgs(Window window)
        {
            Window = window;
        }
    }

    public readonly struct WindowPlacedEventArgs
    {
        public readonly Window Window;

        public WindowPlacedEventArgs(Window window)
        {
            Window = window;
        }
    }

    public struct WindowClosingEventArgs
    {
        public readonly Window Window;
        public readonly WindowCloseReason CloseReason;
        public bool Cancel;

        public WindowClosingEventArgs(Window window, WindowCloseReason closeReason)
        {
            Window = window;
            CloseReason = closeReason;
            Cancel = false;
        }
    }

    public readonly struct WindowClosedEventArgs
    {
        public readonly Window Window;
        public readonly WindowCloseReason CloseReason;

        public WindowClosedEventArgs(Window window, WindowCloseReason closeReason)
        {
            Window = window;
            CloseReason = closeReason;
        }
    }

    public enum WindowCloseReason
    {
        None,
        UserClosing,
        OwnerClosing,
        ServiceClosing,
    }
}
