using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public readonly Audio.AudioStore audioStore = new();
        public readonly Wifi.WifiStore wifiStore = new();
        public readonly UserConfig.UserConfigFile userConfigFile = UserConfig.UserConfigFile.Instance;
        public readonly ObservableCollection<LookoutHistoryRecord> History = new();
        public readonly int maxHistorySize = 100;

        private LookoutAgent()
        {
            wifiStore.PropertyChanged += WifiStore_PropertyChanged;
            audioStore.PropertyChanged += AudioStore_PropertyChanged;
            userConfigFile.PropertyChanged += UserConfigFile_PropertyChanged;
        }

        private void WifiStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Wifi.WifiStore s || e.PropertyName != "Ssid")
            {
                return;
            }
            AddHistory(LookoutEventType.WiFi, wifiStore.IsConnected ? $"Connected to SSID: {wifiStore.Ssid}" : "Disconnected");

            updateByStatus();
        }

        private void AudioStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Audio.AudioStore s || e.PropertyName != "Name")
            {
                return;
            }
            AddHistory(LookoutEventType.Audio, $"Switched device: {audioStore.Name} ({(audioStore.IsMuted ? "Muted" : "Unmuted")})");

            updateByStatus();
        }

        private void UserConfigFile_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            updateByStatus();
        }

        private void updateByStatus()
        {
            // do nothing if not target
            if (audioStore.Name != userConfigFile.Current.SpeakerName) {
                return;
            }

            if (wifiStore.Ssid == userConfigFile.Current.Ssid)
            {
                audioStore.SetMute(false);
                AddHistory(LookoutEventType.MuteAtOffice, "Unmuted");
            }
            else
            {
                audioStore.SetMute(true);
                AddHistory(LookoutEventType.MuteAtOffice, "Muted");
            }
        }

        private void AddHistory(LookoutEventType eventType, string message)
        {
            while (History.Count >= maxHistorySize)
            {
                History.RemoveAt(History.Count - 1);
            }
            
            History.Insert(0, new LookoutHistoryRecord(eventType, message));
            System.Diagnostics.Debug.WriteLine($"[LookoutAgent.History] [{eventType}] {message}");
        }
    }
}
