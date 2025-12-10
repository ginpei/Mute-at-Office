using Microsoft.UI.Xaml.Controls;
using Mute_at_Office.Libs.UserConfig;
using Microsoft.UI.Xaml;
using System;

namespace Mute_at_Office.Pages.Dashboard
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();

            var cfg = UserConfigFile.Instance.Current;
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
    }
}
