using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Mute_at_Office.Libs.Agent;
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
        public ZoneConditionEditType EditType { get; set; } = ZoneConditionEditType.New;
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

            ZoneCondition = new ZoneCondition(parameters.ZoneCondition.ID, parameters.ZoneCondition.SpeakerName, parameters.ZoneCondition.Ssid);
            EditType = parameters.Type;
            SaveButtonText = EditType == ZoneConditionEditType.New ? "Add" : "Update";

            Bindings.Update();
        }

        private async void SaveButton_Clicked(object sender, RoutedEventArgs args)
        {
            var conditions = LookoutAgent.Instance.UserConfigFile.Current.SafeZoneConditions;
            if (EditType == ZoneConditionEditType.New)
            {
                conditions.Add(new ZoneCondition(Guid.NewGuid().ToString(), ZoneCondition.SpeakerName, ZoneCondition.Ssid));
            }
            else
            {
                var existingCondition = conditions.FirstOrDefault(c => c.ID == ZoneCondition.ID);
                if (existingCondition != null)
                {
                    existingCondition.SpeakerName = ZoneCondition.SpeakerName;
                    existingCondition.Ssid = ZoneCondition.Ssid;
                }
            }

            await LookoutAgent.Instance.UserConfigFile.SaveAsync();

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs args)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void DeleteButton_Clicked(object sender, RoutedEventArgs args)
        {
            var dialog = new ContentDialog
            {
                //Title = "Delete zone condition",
                Content = "Are you sure?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            var conditions = LookoutAgent.Instance.UserConfigFile.Current.SafeZoneConditions;
            var existingCondition = conditions.FirstOrDefault(c => c.ID == ZoneCondition.ID);
            if (existingCondition != null)
            {
                conditions.Remove(existingCondition);
                await LookoutAgent.Instance.UserConfigFile.SaveAsync();
            }

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
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
