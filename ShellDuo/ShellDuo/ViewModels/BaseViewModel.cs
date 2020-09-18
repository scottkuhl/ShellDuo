using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using ShellDuo.Models;
using ShellDuo.Services;
using Xamarin.Forms.DualScreen;

namespace ShellDuo.ViewModels
{
    public class BaseViewModel : BaseModel
    {
        public INavigation Navigation { get; set; }
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        #region Screens

        protected bool DeviceIsSpanned => DualScreenInfo.Current.SpanMode != TwoPaneViewMode.SinglePane;
        protected bool DeviceIsBigScreen => (Device.Idiom == TargetIdiom.Tablet) || (Device.Idiom == TargetIdiom.Desktop);

        protected bool WasSpanned = false;
        protected bool IsDetail = false;

        private TwoPaneViewTallModeConfiguration _tallModeConfiguration;

        public TwoPaneViewTallModeConfiguration TallModeConfiguration
        {
            get => _tallModeConfiguration;
            set => SetProperty(ref _tallModeConfiguration, value);
        }

        private TwoPaneViewWideModeConfiguration _wideModeConfiguration;

        public TwoPaneViewWideModeConfiguration WideModeConfiguration
        {
            get => _wideModeConfiguration;
            set => SetProperty(ref _wideModeConfiguration, value);
        }

        private TwoPaneViewPriority _panePriority;

        public TwoPaneViewPriority PanePriority
        {
            get => _panePriority;
            set => SetProperty(ref _panePriority, value);
        }

        protected virtual void UpdateLayouts()
        {
            UpdateLayouts(false, null);
        }

        protected async void UpdateLayouts(bool itemSelected, string route)
        {
            if (IsDetail && DeviceIsSpanned)
            {   // the detail view should never be showing when spanned
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopToRootAsync();
                }
            }
            else if (DeviceIsSpanned || DeviceIsBigScreen)
            {   // two screens: side by side
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.TopBottom;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.LeftRight;
                WasSpanned = true;
            }
            else
            {   // single-screen: only list is shown
                PanePriority = TwoPaneViewPriority.Pane1;
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.SinglePane;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane;
                // wasSpanned check is needed, or this will open on first-run or rotation
                // stack count is needed, or we might push multiple on rotation
                if (WasSpanned && itemSelected)
                {   // open the detail page
                    await Shell.Current.GoToAsync(route);
                }
                WasSpanned = false;
            }
        }

        protected void DualScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateLayouts();
        }

        public void OnAppearing(bool isDetail = false)
        {
            IsBusy = true;
            IsDetail = isDetail;
            DualScreenInfo.Current.PropertyChanged += DualScreen_PropertyChanged;
            UpdateLayouts();
        }

        public void OnDisappearing()
        {
            DualScreenInfo.Current.PropertyChanged -= DualScreen_PropertyChanged;
        }

        #endregion Screens
    }
}
