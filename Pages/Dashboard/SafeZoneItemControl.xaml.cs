using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mute_at_Office.Libs.UserConfig;

namespace Mute_at_Office.Pages.Dashboard;

public sealed partial class SafeZoneItemControl : UserControl
{
    public ZoneCondition ZoneCondition
    {
        get => (ZoneCondition)GetValue(ZoneConditionProperty);
        set => SetValue(ZoneConditionProperty, value);
    }

    public static readonly DependencyProperty ZoneConditionProperty =
        DependencyProperty.Register(
            nameof(ZoneCondition),
            typeof(ZoneCondition),
            typeof(SafeZoneItemControl),
            new PropertyMetadata(null));

    public SafeZoneItemControl()
    {
        this.InitializeComponent();
    }
}
