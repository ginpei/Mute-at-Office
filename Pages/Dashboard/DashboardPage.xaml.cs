using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Mute_at_Office.Libs.Agent;
using Mute_at_Office.Libs.UserConfig;
using Mute_at_Office.Pages.ZoneConditionEdit;
using System;
using System.Collections.ObjectModel;

namespace Mute_at_Office.Pages.Dashboard
{
    public sealed partial class DashboardPage : Page
    {
        public ObservableCollection<ZoneCondition> SafeZoneConditions = [];

        public DashboardPage()
        {
            InitializeComponent();

            if (LookoutAgent.Instance != null)
            {
                RenderSsid(LookoutAgent.Instance.WifiStore.Ssid);
                LookoutAgent.Instance.WifiStore.PropertyChanged += WifiStore_PropertyChanged;

                RenderSpeaker(LookoutAgent.Instance.AudioStore.Name);
                LookoutAgent.Instance.AudioStore.PropertyChanged += AudioStore_PropertyChanged;
            }

            UserConfigFile.Instance.PropertyChanged += UserConfig_PropertyChanged;

            var cfg = UserConfigFile.Instance.Current;
            System.Diagnostics.Debug.WriteLine($"dashboard init \"{cfg.Ssid}\" ({string.IsNullOrEmpty(cfg.Ssid)})");
            if (!string.IsNullOrEmpty(cfg.Ssid))
            {
                if (this.FindName("SsidRun") is Microsoft.UI.Xaml.Documents.Run ssidRun)
                {
                    ssidRun.Text = cfg.Ssid;
                }
            }

            if (!string.IsNullOrEmpty(cfg.SpeakerName))
            {
                if (this.FindName("SpeakerRun") is Microsoft.UI.Xaml.Documents.Run speakerRun)
                {
                    speakerRun.Text = cfg.SpeakerName;
                }
            }

            SafeZoneConditions = new ObservableCollection<ZoneCondition>(cfg.safeZoneConditions);
        }

        private void WifiStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Libs.Wifi.WifiStore s)
            {
                return;
            }

            RenderSsid(s.Ssid);
        }

        private void AudioStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Libs.Audio.AudioStore s)
            {
                return;
            }

            RenderSpeaker(s.Name);
        }

        private void UserConfig_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not UserConfigFile configFile)
            {
                return;
            }

            var cfg = configFile.Current;

            SafeZoneConditions = new ObservableCollection<ZoneCondition>(cfg.safeZoneConditions);

            if (!string.IsNullOrEmpty(cfg.Ssid))
            {
                if (this.FindName("SsidRun") is Microsoft.UI.Xaml.Documents.Run ssidRun)
                {
                    ssidRun.Text = cfg.Ssid;
                }
            }

            if (!string.IsNullOrEmpty(cfg.SpeakerName))
            {
                if (this.FindName("SpeakerRun") is Microsoft.UI.Xaml.Documents.Run speakerRun)
                {
                    speakerRun.Text = cfg.SpeakerName;
                }
            }
        }

        private void SafeZoneItem_ItemClicked(object sender, ZoneCondition zoneCondition)
        {
            Frame.Navigate(typeof(ZoneConditionEditPage), new ZoneConditionEditParameters(ZoneConditionEditType.Update, zoneCondition));
        }

        private void AddConditionButton_Clicked(object sender, RoutedEventArgs args)
        {
            ZoneCondition condition = new("", LookoutAgent.Instance.AudioStore.Name, LookoutAgent.Instance.WifiStore.Ssid);
            Frame.Navigate(typeof(ZoneConditionEditPage), new ZoneConditionEditParameters(ZoneConditionEditType.New, condition));
        }

        private void RenderSsid(string ssid)
        {
            if (this.FindName("SsidText") is not TextBlock ssidText)
            {
                return;
            }

            ssidText.Text = string.IsNullOrEmpty(ssid) ? "(No WiFi)" : ssid;
        }

        private void RenderSpeaker(string name)
        {
            if (this.FindName("SpeakerText") is not TextBlock speakerText)
            {
                return;
            }

            speakerText.Text = string.IsNullOrEmpty(name) ? "(No speaker)" : name;
        }
    }

    public partial class EmptyCountToVisibilityVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not int count)
            {
                return Visibility.Collapsed;
            }

            var inverse = parameter?.ToString() == "inverse";
            var isEmpty = count == 0;
            var isVisible = inverse ? !isEmpty : isEmpty;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
