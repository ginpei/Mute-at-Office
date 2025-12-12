using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mute_at_Office.Libs.UserConfig
{
    class UserConfigFile : IDisposable
    {
        // provide a singleton instance for easy access from UI code
        private static readonly Lazy<UserConfigFile> _instance = new(() => new UserConfigFile());
        public static UserConfigFile Instance => _instance.Value;

        private readonly string _directory;
        private readonly string _filePath;

        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private bool _disposed;
        public UserConfig Current { get; private set; } = new UserConfig();

        private UserConfigFile()
        {
            var appName = "MuteAtOffice";

            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _directory = Path.Combine(local, appName);
            _filePath = Path.Combine(_directory, "config.json");
        }

        public async Task LoadAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (File.Exists(_filePath))
                {
                    //var json = await File.ReadAllTextAsync(_filePath);

                    await using var fs = File.OpenRead(_filePath);
                    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    Current = await JsonSerializer.DeserializeAsync<UserConfig>(fs, opts)
                              ?? new UserConfig();
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
                Directory.CreateDirectory(_directory);
                await using var fs = File.Create(_filePath);
                var opts = new JsonSerializerOptions { WriteIndented = true };
                await JsonSerializer.SerializeAsync(fs, Current, opts);
                await fs.FlushAsync();
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
}
