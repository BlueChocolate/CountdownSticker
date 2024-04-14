using CountdownSticker.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CountdownSticker.Services
{
    public interface ISettingService
    {
        public event EventHandler<SettingChangedEventArgs> SettingChanged;
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

        public event EventHandler<SettingChangedEventArgs> SettingChanged;

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
                { "Theme", "Dark" },
                { "Language", "zh-CN" },
                { "Topmost", true },
                { "AutoStart", false },
                { "HorizontalSpacing" ,30 },
                { "VerticalSpacing", 30 },
                { "HideMainWindowAtStartup", true}
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
            SaveToFile();
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
            var oldValue = GetSetting<object>(key);
            string json = JsonSerializer.Serialize(value);
            _settings[key] = JsonDocument.Parse(json).RootElement;
            SaveToFile();
            OnSettingChanged(new SettingChangedEventArgs(key, oldValue, value));
        }

        private void SaveToFile()
        {
            // 保存设置
            var settingText = JsonSerializer.Serialize(_settings, _jsonSerializerOptions);
            _fileService.WriteTextToFile(_settingsFilePath, settingText);
        }

        protected virtual void OnSettingChanged(SettingChangedEventArgs e)
        {
            SettingChanged?.Invoke(this, e);
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

    public class SettingChangedEventArgs : EventArgs
    {
        public string SettingName { get; }
        public object? OldValue { get; }
        public object NewValue { get; }

        public SettingChangedEventArgs(string settingName, object? oldValue, object newValue)
        {
            SettingName = settingName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}