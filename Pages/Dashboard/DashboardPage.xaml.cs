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
    public ObservableCollection<ZoneCondition> SafeZoneConditions
    {
        get
        {
            var config = LookoutAgent.Instance.UserConfigFile.Current;
            return new ObservableCollection<ZoneCondition>(config.SafeZoneConditions);
        }
    }

    public string SpeakerName
    {
        get
        {
            var name = LookoutAgent.Instance.AudioStore.Name;
            return string.IsNullOrEmpty(name) ? "(No speaker)" : name;
        }
    }


    public string Ssid
    {
        get
        {
            var ssid = LookoutAgent.Instance.WifiStore.Ssid;
            return string.IsNullOrEmpty(ssid) ? "(No WiFi)" : ssid;
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

        LookoutAgent.Instance.WifiStore.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Ssid));
        LookoutAgent.Instance.AudioStore.PropertyChanged +=  (s, e) => OnPropertyChanged(nameof(SpeakerName));
        UserConfigFile.Instance.PropertyChanged += (s, e) => OnPropertyChanged(nameof(SafeZoneConditions));
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
