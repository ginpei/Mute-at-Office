using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Mute_at_Office.Libs.Agent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mute_at_Office.Pages.History
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryPage : Page
    {
        public HistoryPage()
        {
            InitializeComponent();

            this.Loaded += HistoryPage_Loaded;
        }

        private void HistoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsControl = this.FindName("HistoryItemsControl") as ItemsControl;
            if (itemsControl != null)
            {
                itemsControl.ItemsSource = LookoutAgent.Instance.History;
            }
        }
    }
}
