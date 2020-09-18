using System;
using System.Collections.Generic;
using ShellDuo.ViewModels;
using ShellDuo.Views;
using Xamarin.Forms;

namespace ShellDuo
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
