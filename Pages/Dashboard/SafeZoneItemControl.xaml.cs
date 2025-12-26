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

    public event System.EventHandler<ZoneCondition>? ItemClicked;

    public SafeZoneItemControl()
    {
        this.InitializeComponent();
    }

    private void Button_Clicked(object sender, RoutedEventArgs e)
    {
        if (ZoneCondition == null)
        {
            return;
        }

        ItemClicked?.Invoke(this, ZoneCondition);
    }
}
