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
            AddHistory(LookoutEventType.Audio, $"Switched device: {AudioStore.Name}");

            UpdateByStatus();
        }

        private void UserConfigFile_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateByStatus();
        }

        private void UpdateByStatus()
        {
            var speakerName = AudioStore.Name;
            var ssid = WifiStore.Ssid;
            var allConditions = UserConfigFile.Current.SafeZoneConditions;
            var isMuted = AudioStore.IsMuted;

            if (allConditions.Count() == 0)
            {
                // no need to log
                return;
            }

            var speakerConditions = allConditions.Where(condition => condition.SpeakerName == speakerName);
            if (speakerConditions.Count() == 0)
            {
                if (isMuted)
                {
                    AudioStore.SetMute(false);
                    AddHistory(LookoutEventType.MuteAtOffice, "Unmuted (non-target speaker)");
                }
                else
                {
                    AddHistory(LookoutEventType.MuteAtOffice, "Keep unmuted (non-target speaker)");
                }

                return;
            }

            if (!WifiStore.IsConnected)
            {
                if (isMuted)
                {
                    AddHistory(LookoutEventType.MuteAtOffice, "Keep muted (no WiFi)");
                }
                else
                {
                    AudioStore.SetMute(true);
                    AddHistory(LookoutEventType.MuteAtOffice, "Muted (no WiFi)");
                }

                return;
            }

            var isSafe = speakerConditions.Any(condition => condition.Ssid == ssid);
            if (isSafe)
            {
                if (isMuted)
                {
                    AudioStore.SetMute(false);
                    AddHistory(LookoutEventType.MuteAtOffice, "Unmuted (matched safe zone)");
                }
                else
                {
                    AddHistory(LookoutEventType.MuteAtOffice, "Keep unmuted (matched safe zone)");
                }
            }
            else
            {
                if (isMuted)
                {
                    AddHistory(LookoutEventType.MuteAtOffice, "Keep muted (no safe zone match)");
                }
                else
                {
                    AudioStore.SetMute(true);
                    AddHistory(LookoutEventType.MuteAtOffice, "Muted (no safe zone match)");
                }
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
