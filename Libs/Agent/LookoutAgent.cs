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
        private static readonly Lazy<LookoutAgent> _instance = new(() => new LookoutAgent());

        public static LookoutAgent Instance => _instance.Value;

        public Audio.AudioStore AudioStore { get; } = new();
        public Wifi.WifiStore WifiStore { get; } = new();
        public UserConfig.UserConfigFile UserConfigFile { get; } = UserConfig.UserConfigFile.Instance;
        public ObservableCollection<LookoutHistoryRecord> History { get; } = new();
        public int MaxHistorySize { get; } = 100;

        private LookoutAgent()
        {
            WifiStore.PropertyChanged += WifiStore_PropertyChanged;
            AudioStore.PropertyChanged += AudioStore_PropertyChanged;
            UserConfigFile.PropertyChanged += UserConfigFile_PropertyChanged;
        }

        private void WifiStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Wifi.WifiStore s || e.PropertyName != "Ssid")
            {
                return;
            }
            AddHistory(LookoutEventType.WiFi, WifiStore.IsConnected ? $"Connected to SSID: {WifiStore.Ssid}" : "Disconnected");

            UpdateByStatus();
        }

        private void AudioStore_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Audio.AudioStore s || e.PropertyName != "Name")
            {
                return;
            }
            AddHistory(LookoutEventType.Audio, $"Switched device: {AudioStore.Name} ({(AudioStore.IsMuted ? "Muted" : "Unmuted")})");

            UpdateByStatus();
        }

        private void UserConfigFile_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateByStatus();
        }

        private void UpdateByStatus()
        {
            // do nothing if not target
            if (AudioStore.Name != UserConfigFile.Current.SpeakerName) {
                return;
            }

            if (WifiStore.Ssid == UserConfigFile.Current.Ssid)
            {
                AudioStore.SetMute(false);
                AddHistory(LookoutEventType.MuteAtOffice, "Unmuted");
            }
            else
            {
                AudioStore.SetMute(true);
                AddHistory(LookoutEventType.MuteAtOffice, "Muted");
            }
        }

        private void AddHistory(LookoutEventType eventType, string message)
        {
            while (History.Count >= MaxHistorySize)
            {
                History.RemoveAt(History.Count - 1);
            }
            
            History.Insert(0, new LookoutHistoryRecord(eventType, message));
            System.Diagnostics.Debug.WriteLine($"[LookoutAgent.History] [{eventType}] {message}");
        }
    }
}
