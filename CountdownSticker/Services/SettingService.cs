using CountdownSticker.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CountdownSticker.Services
{
    public interface ISettingService
    {
        public T? GetSetting<T>(string key);
        public void SetSetting(string key, object value);
        public Task<T> GetSettingAsync<T>(string key);
        public Task SetSettingAsync(string key, object value);
    }

    public class SettingService : ISettingService
    {
        private readonly IFileService _fileService;
        private readonly string _settingsFilePath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly object _lock;
        private Dictionary<string, JsonElement> _settings;

        public SettingService()
        {
            _fileService = App.Services.GetRequiredService<IFileService>();
            _settingsFilePath = "settings.json";
            _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            _lock = new object();

            var settingsJson = _fileService.ReadFileText(_settingsFilePath) ?? "{}";
            var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(settingsJson);
            _settings = settings ?? [];

            // 如果 Key 不存在，添加默认值
            var defaultSettings = new Dictionary<string, object?>
            {
                { "CountdownStickersFilePath", "C:\\Users\\RadioNoise\\OneDrive\\便笺\\CountdownStickers.json" },
                { "Theme", "Dark" }
            };
            foreach (var defaultSetting in defaultSettings)
            {
                if (!_settings.ContainsKey(defaultSetting.Key))
                {
                    string jsonText = JsonSerializer.Serialize(defaultSetting.Value);
                    var jsonElement = JsonDocument.Parse(jsonText).RootElement;
                    _settings[defaultSetting.Key] = jsonElement;
                }
            }
            SaveSettingsToFile();
        }

        public T? GetSetting<T>(string key)
        {
            if (_settings.GetValueOrDefault(key) is JsonElement jsonElement)
            {
                var value = JsonSerializer.Deserialize<T>(jsonElement);
                return value;
            }
            return default(T);
        }

        public void SetSetting(string key, object value)
        {
            string json = JsonSerializer.Serialize(value);
            _settings[key] = JsonDocument.Parse(json).RootElement;
            SaveSettingsToFile();
        }

        private void SaveSettingsToFile()
        {
            var settingText = JsonSerializer.Serialize(_settings, _jsonSerializerOptions);
            _fileService.WriteTextToFile(_settingsFilePath, settingText);
        }

        Task<T> ISettingService.GetSettingAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        Task ISettingService.SetSettingAsync(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}
