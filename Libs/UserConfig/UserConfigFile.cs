using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mute_at_Office.Libs.UserConfig;

class UserConfigFile : INotifyPropertyChanged, IDisposable
{
    // provide a singleton instance for easy access from UI code
    private static readonly Lazy<UserConfigFile> _instance = new(() => new UserConfigFile());
    public static UserConfigFile Instance => _instance.Value;

    private readonly string _directory;
    public string FilePath { get; } = "";

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;
    public UserConfig Current { get; private set; } = new UserConfig();

    public event PropertyChangedEventHandler? PropertyChanged;

    private UserConfigFile()
    {
        _directory = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        FilePath = Path.Combine(_directory, "config.json");
    }

    public void NotifyPropertyChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
    }

    public async Task LoadAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (File.Exists(FilePath))
            {
                //var json = await File.ReadAllTextAsync(_filePath);

                await using var fs = File.OpenRead(FilePath);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Current = await JsonSerializer.DeserializeAsync<UserConfig>(fs, opts)
                          ?? new UserConfig();
                System.Diagnostics.Debug.WriteLine($"[UserConfigFile] Loaded: {FilePath}");
                NotifyPropertyChanged();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await using var fs = File.Create(FilePath);
            var opts = new JsonSerializerOptions { WriteIndented = true };
            await JsonSerializer.SerializeAsync(fs, Current, opts);
            await fs.FlushAsync();
            System.Diagnostics.Debug.WriteLine($"[UserConfigFile] Saved: {FilePath}");
            NotifyPropertyChanged();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore.Dispose();
            _disposed = true;
        }
    }
}
