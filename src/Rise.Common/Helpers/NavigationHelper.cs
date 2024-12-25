using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.Common.Helpers
{
    /// <summary>
    /// NavigationHelper aids in navigation between pages.  It provides commands used to 
    /// navigate back and forward as well as registers for standard mouse and keyboard 
    /// shortcuts used to go back and forward in Windows and the hardware back button in
    /// Windows Phone.  In addition it integrates SuspensionManger to handle process lifetime
    /// management and state management when navigating between pages.
    /// </summary>
    /// <example>
    /// To make use of NavigationHelper, follow these two steps or
    /// start with a BasicPage or any other Page item template other than BlankPage.
    /// 
    /// 1) Create an instance of the NavigationHelper somewhere such as in the 
    ///     constructor for the page and register a callback for the LoadState and 
    ///     SaveState events.
    /// <code>
    ///     public MyPage()
    ///     {
    ///         this.InitializeComponent();
    ///         var navigationHelper = new NavigationHelper(this);
    ///         this.navigationHelper.LoadState += navigationHelper_LoadState;
    ///         this.navigationHelper.SaveState += navigationHelper_SaveState;
    ///     }
    ///     
    ///     private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
    ///     { }
    ///     private async void navigationHelper_SaveState(object sender, LoadStateEventArgs e)
    ///     { }
    /// </code>
    /// 
    /// 2) Register the page to call into the NavigationHelper whenever the page participates 
    ///     in navigation by overriding the <see cref="Views.UI.Xaml.Controls.Page.OnNavigatedTo"/> 
    ///     and <see cref="Views.UI.Xaml.Controls.Page.OnNavigatedFrom"/> events.
    /// <code>
    ///     protected override void OnNavigatedTo(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedTo(e);
    ///     }
    ///     
    ///     protected override void OnNavigatedFrom(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedFrom(e);
    ///     }
    /// </code>
    /// </example>
    [WebHostHidden]
    public partial class NavigationHelper : DependencyObject
    {
        private Page Page { get; set; }
        private Frame Frame => Page.Frame;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationHelper"/> class.
        /// </summary>
        /// <param name="page">A reference to the current page used for navigation.  
        /// This reference allows for frame manipulation.</param>
        public NavigationHelper(Page page)
        {
            Page = page;

            // When this page is part of the visual tree make two changes:
            // 1) Map application view state to visual state for the page
            // 2) Handle hardware navigation requests
            Page.Loaded += (sender, e) =>
            {
#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#else
                // Listen to the window directly so focus isn't required
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed +=
                    CoreWindow_PointerPressed;
#endif
            };

            // Undo the same changes when the page is no longer visible
            Page.Unloaded += (sender, e) =>
            {
#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#else
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    CoreWindow_PointerPressed;
#endif
            };
        }

        #region Navigation support

        /// <summary>
        /// Virtual method used by the <see cref="GoBackCommand"/> property
        /// to determine if the <see cref="Frame"/> can go back.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Frame"/> has at least one entry 
        /// in the back navigation history.
        /// </returns>
        public virtual bool CanGoBack()
        {
            return Frame != null && Frame.CanGoBack;
        }
        /// <summary>
        /// Virtual method used by the <see cref="GoForwardCommand"/> property
        /// to determine if the <see cref="Frame"/> can go forward.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Frame"/> has at least one entry 
        /// in the forward navigation history.
        /// </returns>
        public virtual bool CanGoForward()
        {
            return Frame != null && Frame.CanGoForward;
        }

        /// <summary>
        /// Virtual method used by the <see cref="GoBackCommand"/> property
        /// to invoke the <see cref="Views.UI.Xaml.Controls.Frame.GoBack"/> method.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanGoBack))]
        public virtual void GoBack()
        {
            if (Frame != null && Frame.CanGoBack) Frame.GoBack();
        }

        /// <summary>
        /// Virtual method used by the <see cref="GoForwardCommand"/> property
        /// to invoke the <see cref="Views.UI.Xaml.Controls.Frame.GoForward"/> method.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanGoForward))]
        public virtual void GoForward()
        {
            if (Frame != null && Frame.CanGoForward) Frame.GoForward();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Invoked when the hardware back button is pressed. For Windows Phone only.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (GoBackCommand.CanExecute(null))
            {
                e.Handled = true;
                GoBackCommand.Execute(null);
            }
        }
#else
        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations, when
        /// this page is active and occupies the entire window.  Used to detect keyboard navigation
        /// between pages even when the page itself doesn't have focus.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Only investigate further when Left, Right, or the dedicated Previous or Next keys
            // are pressed
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // When the previous key or Alt+Left are pressed navigate back
                    e.Handled = true;
                    GoBackCommand.Execute(null);
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    e.Handled = true;
                    GoForwardCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        /// page is active and occupies the entire window.  Used to detect browser-style next and
        /// previous mouse button clicks to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) GoBackCommand.Execute(null);
                if (forwardPressed) GoForwardCommand.Execute(null);
            }
        }
#endif

        #endregion

        #region Process lifetime management

        private string _pageKey;

        /// <summary>
        /// Register this event on the current page to populate the page
        /// with content passed during navigation as well as any saved
        /// state provided when recreating a page from a prior session.
        /// </summary>
        public event LoadStateEventHandler LoadState;
        /// <summary>
        /// Register this event on the current page to preserve
        /// state associated with the current page in case the
        /// application is suspended or the page is discarded from
        /// the navigaqtion cache.
        /// </summary>
        public event SaveStateEventHandler SaveState;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.  
        /// This method calls <see cref="LoadState"/>, where all page specific
        /// navigation and process lifetime management logic should be placed.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property provides the group to be displayed.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(Frame);
            _pageKey = "Page-" + Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page to the
                // navigation stack
                var nextPageKey = _pageKey;
                int nextPageIndex = Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Pass the navigation parameter to the new page
                LoadState?.Invoke(this, new LoadStateEventArgs(e.Parameter, null));
            }
            else
            {
                // Pass the navigation parameter and preserved page state to the page, using
                // the same strategy for loading suspended state and recreating pages discarded
                // from cache
                LoadState?.Invoke(this, new LoadStateEventArgs(e.Parameter, (Dictionary<string, object>)frameState[_pageKey]));
            }
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// This method calls <see cref="SaveState"/>, where all page specific
        /// navigation and process lifetime management logic should be placed.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property provides the group to be displayed.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(Frame);
            var pageState = new Dictionary<string, object>();
            SaveState?.Invoke(this, new SaveStateEventArgs(pageState));
            frameState[_pageKey] = pageState;
        }

        #endregion
    }

    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.LoadState"/>event
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.SaveState"/>event
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// Class used to hold the event data required when a page attempts to load state.
    /// </summary>
    public class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// The parameter value passed to <see cref="Frame.Navigate(Type, object)"/> 
        /// when this page was initially requested.
        /// </summary>
        public object NavigationParameter { get; private set; }
        /// <summary>
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </summary>
        public Dictionary<string, object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadStateEventArgs"/> class.
        /// </summary>
        /// <param name="navigationParameter">
        /// The parameter value passed to <see cref="Frame.Navigate(Type, object)"/> 
        /// when this page was initially requested.
        /// </param>
        /// <param name="pageState">
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </param>
        public LoadStateEventArgs(object navigationParameter, Dictionary<string, object> pageState)
            : base()
        {
            NavigationParameter = navigationParameter;
            PageState = pageState;
        }
    }
    /// <summary>
    /// Class used to hold the event data required when a page attempts to save state.
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// An empty dictionary to be populated with serializable state.
        /// </summary>
        public Dictionary<string, object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        public SaveStateEventArgs(Dictionary<string, object> pageState)
            : base()
        {
            PageState = pageState;
        }
    }
}
