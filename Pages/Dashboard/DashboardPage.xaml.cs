using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Mute_at_Office.Libs.Agent;
using Mute_at_Office.Libs.UserConfig;
using Mute_at_Office.Pages.ZoneConditionEdit;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Mute_at_Office.Pages.Dashboard;

public sealed partial class DashboardPage : Page, INotifyPropertyChanged
{
    public ObservableCollection<ZoneCondition> SafeZoneConditions = [];

    private string _speakerName = "...";
    public string SpeakerName
    {
        get => string.IsNullOrEmpty(_speakerName) ? "(No speaker)" : _speakerName;
        set
        {
            if (_speakerName != value)
            {
                _speakerName = value;
                OnPropertyChanged();
            }
        }
    }

    private string _ssid = "...";
    public string Ssid
    {
        get => string.IsNullOrEmpty(_ssid) ? "(No WiFi)" : _ssid;
        set
        {
            if (_ssid != value)
            {
                _ssid = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DashboardPage()
    {
        InitializeComponent();

        if (LookoutAgent.Instance != null)
        {
            Ssid = LookoutAgent.Instance.WifiStore.Ssid;
            LookoutAgent.Instance.WifiStore.PropertyChanged += WifiStore_PropertyChanged;

            SpeakerName = LookoutAgent.Instance.AudioStore.Name;
            LookoutAgent.Instance.AudioStore.PropertyChanged += AudioStore_PropertyChanged;
        }

        UserConfigFile.Instance.PropertyChanged += UserConfig_PropertyChanged;

        SafeZoneConditions = new ObservableCollection<ZoneCondition>(UserConfigFile.Instance.Current.SafeZoneConditions);
    }

    private void WifiStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not Libs.Wifi.WifiStore s)
        {
            return;
        }

        Ssid = s.Ssid;
    }

    private void AudioStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not Libs.Audio.AudioStore s)
        {
            return;
        }

        SpeakerName = s.Name;
    }

    private void UserConfig_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not UserConfigFile configFile)
        {
            return;
        }

        var cfg = configFile.Current;

        SafeZoneConditions = new ObservableCollection<ZoneCondition>(cfg.SafeZoneConditions);
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
