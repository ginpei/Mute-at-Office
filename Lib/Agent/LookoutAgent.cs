using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Radios;

namespace Mute_at_Office.Lib.Agent
{
    class LookoutAgent
    {
        private readonly string ssid = "";

        private readonly Audio.AudioStore audioStore = new();
        private readonly Wifi.WifiStore wifiStore = new();

        public LookoutAgent()
        {
            wifiStore.PropertyChanged += WifiStore_PropertyChanged;
        }

        private void WifiStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Wifi.WifiStore s)
            {
                if (e.PropertyName == "Ssid")
                {
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
    }
}
