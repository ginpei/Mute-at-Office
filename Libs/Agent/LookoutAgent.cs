using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Radios;

namespace Mute_at_Office.Libs.Agent
{
    class LookoutAgent
    {
        private static readonly Lazy<LookoutAgent> instance = new(() => new LookoutAgent());

        public static LookoutAgent Instance => instance.Value;

        private readonly string ssid = "";

        public readonly Audio.AudioStore audioStore = new();
        public readonly Wifi.WifiStore wifiStore = new();

        private LookoutAgent()
        {
            wifiStore.PropertyChanged += WifiStore_PropertyChanged;
        }

        private void WifiStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Wifi.WifiStore s || e.PropertyName != "Ssid")
            {
                return;
            }

            if (s.Ssid == ssid)
            {
                Debug.WriteLine("#wifi OK!");
                audioStore.SetMute(false);
            }
            else
            {
                Debug.WriteLine($"#wifi Uh ({s.Ssid})");
                audioStore.SetMute(true);
            }
        }
    }
}
