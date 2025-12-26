using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Mute_at_Office.Libs.UserConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mute_at_Office.Pages.ZoneConditionEdit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ZoneConditionEditPage : Page
    {
        public ZoneCondition ZoneCondition { get; set; } = new ZoneCondition("", "", "");
        public string SaveButtonText { get; set; } = "";

        public ZoneConditionEditPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not ZoneConditionEditParameters parameters)
            {
                return;
            }

            ZoneCondition = parameters.ZoneCondition;

            SaveButtonText = parameters.Type == ZoneConditionEditType.New ? "Add" : "Update";
            Bindings.Update();
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs args)
        {
            // TODO
            System.Diagnostics.Debug.WriteLine($"click: {ZoneCondition.SpeakerName}, {ZoneCondition.Ssid}");
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs args)
        {
            // TODO
            System.Diagnostics.Debug.WriteLine("cancel");
        }
    }

    public enum ZoneConditionEditType { New, Update };

    public class ZoneConditionEditParameters
    {
        public ZoneCondition ZoneCondition { get; set; }
        public ZoneConditionEditType Type { get; set; }

        public ZoneConditionEditParameters(ZoneConditionEditType type, ZoneCondition zoneCondition)
        {
            ZoneCondition = zoneCondition;
            Type = type;
        }
    }
}
