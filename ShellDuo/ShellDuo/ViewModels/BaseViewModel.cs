using ShellDuo.Models;
using ShellDuo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;

namespace ShellDuo.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        private string title = string.Empty;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        #region INotifyPropertyChanged

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

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
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopToRootAsync();
                }
            }
            else if (DeviceIsSpanned || DeviceIsBigScreen)
            {
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.TopBottom;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.LeftRight;
                WasSpanned = true;
            }
            else
            {
                PanePriority = TwoPaneViewPriority.Pane1;
                TallModeConfiguration = TwoPaneViewTallModeConfiguration.SinglePane;
                WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane;

                if (WasSpanned && itemSelected)
                {
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
