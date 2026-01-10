using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sms;
using Windows.Foundation.Collections;

namespace Mute_at_Office.Libs.Audio;

internal class AudioStore : ObservableObject, IDisposable, IMMNotificationClient
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private uint _volume;
    public uint Volume
    {
        get => _volume;
        set => SetProperty(ref _volume, value);
    }

    private bool _isMuted = false;
    public bool IsMuted
    {
        get => _isMuted;
        set => SetProperty(ref _isMuted, value);
    }

    private MMDeviceEnumerator deviceEnumerator;
    private MMDevice? device;

    private readonly DispatcherQueue dispatcher = DispatcherQueue.GetForCurrentThread();

    public AudioStore()
    {
        deviceEnumerator = new MMDeviceEnumerator();
        deviceEnumerator.RegisterEndpointNotificationCallback(this);

        device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        device.AudioEndpointVolume.OnVolumeNotification += Device_OnVolumeNotification;

        UpdateValuesByDevice();
    }

    private void UpdateValuesByDevice()
    {
        if (device == null)
        {
            return;
        }

        var name = device.FriendlyName;
        var volume = (uint)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        var isMuted = device.AudioEndpointVolume.Mute;

        dispatcher.TryEnqueue(() =>
        {
            Name = name;
            Volume = volume;
            IsMuted = isMuted;
            Debug.WriteLine($"AudioStore: UpdateValuesByDevice {Name} {Volume} {(IsMuted ? "(muted)" : "")}");
        });
    }

    private void Device_OnVolumeNotification(AudioVolumeNotificationData data)
    {
        UpdateValuesByDevice();
    }

    // IMMNotificationClient implementation for device change notifications
    public void OnDeviceStateChanged(string pwstrDeviceId, DeviceState dwNewState) { }

    public void OnDeviceAdded(string pwstrDeviceId) { }

    public void OnDeviceRemoved(string pwstrDeviceId) { }

    public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string pwstrDefaultDeviceId)
    {
        Task.Run(() =>
        {
            try
            {
                MMDevice? oldDevice = null;
                MMDevice? newDevice = null;

                lock (this)
                {
                    oldDevice = device;

                    if (oldDevice != null)
                    {
                        oldDevice.AudioEndpointVolume.OnVolumeNotification -= Device_OnVolumeNotification;
                    }

                    newDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    Debug.WriteLine($"OnDefaultDeviceChanged {oldDevice?.FriendlyName} -> {newDevice?.FriendlyName}");
                    device = newDevice;

                    if (newDevice != null)
                    {
                        newDevice.AudioEndpointVolume.OnVolumeNotification += Device_OnVolumeNotification;
                    }
                }

                if (newDevice != null)
                {
                    UpdateValuesByDevice();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnDefaultDeviceChanged Exception: {ex}");
            }
        });
    }

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

    public void Dispose()
    {
        deviceEnumerator?.UnregisterEndpointNotificationCallback(this);
        deviceEnumerator?.Dispose();
    }

    public void SetVolume(uint newVolume)
    {
        if (device?.AudioEndpointVolume == null)
        {
            return;
        }

        try
        {
            // Clamp volume between 0 and 100
            newVolume = Math.Max(0, Math.Min(newVolume, 100));

            // Convert from percentage to scalar (0.0 to 1.0)
            float volumeScalar = newVolume / 100.0f;

            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeScalar;
            Volume = newVolume;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SetVolume Exception: {ex}");
        }
    }

    public void SetMute(bool mute)
    {
        if (device?.AudioEndpointVolume == null)
        {
            return;
        }

        Task.Run(() =>
        {
            try
            {
                lock (this)
                {
                    if (device?.AudioEndpointVolume != null)
                    {
                        device.AudioEndpointVolume.Mute = mute;
                        
                        dispatcher.TryEnqueue(() =>
                        {
                            IsMuted = mute;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SetMute Exception: {ex}");
            }
        });
    }

    public void ToggleMute()
    {
        SetMute(!IsMuted);
    }
}
