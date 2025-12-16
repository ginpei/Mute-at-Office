using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Mute_at_Office.Pages.Dashboard;
using Mute_at_Office.Pages.History;
using Mute_at_Office.Pages.Settings;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mute_at_Office
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private bool _isExitRequested;

        public MainWindow()
        {
            InitializeComponent();

            this.Closed += OnWindowClosed;

            AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1200, Height = 1200 });

            ContentFrame.Navigate(typeof(DashboardPage));
            MainNavigationView.SelectedItem = MainNavigationView.MenuItems[0];
        }

        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            if (!_isExitRequested)
            {
                args.Handled = true;
                this.Hide();
            }
        }

        [RelayCommand]
        public void CloseWindow()
        {
            _isExitRequested = true;
            Application.Current.Exit();
        }

        [RelayCommand]
        public void ShowWindow()
        {
            this.Show();
            this.Activate();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem selectedItem)
            {
                return;
            }

            var tag = selectedItem.Tag?.ToString();

            switch (tag)
            {
                case "Dashboard":
                    ContentFrame.Navigate(typeof(DashboardPage));
                    break;
                case "History":
                    ContentFrame.Navigate(typeof(HistoryPage));
                    break;
                case "Settings":
                    ContentFrame.Navigate(typeof(SettingsPage));
                    break;
            }
        }
    }
}
