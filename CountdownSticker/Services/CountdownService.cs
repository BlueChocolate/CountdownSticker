using CountdownSticker.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace CountdownSticker.Services
{
    public interface ICountdownService
    {
        public ICollection<Countdown> GetCountdowns();
        public void AddCountdown(Countdown countdown);
        public void UpdateCountdown(Countdown countdown);
        public void UpdateCountdown(Guid id, string? title = null, string? note = null, DateTime? endTime = null, bool? isVisible = null);
        public void RemoveCountdown(Countdown countdown);
        public void RemoveCountdown(Guid id);
    }

    public class CountdownService : ICountdownService
    {
        private readonly ISettingService _settingService;
        private readonly IFileService _fileService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private ICollection<Countdown> _countdowns;

        public CountdownService()
        {
            _settingService = App.Services.GetRequiredService<ISettingService>();
            _fileService = App.Services.GetRequiredService<IFileService>();
            _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            _countdowns = ReadCountdownsFromFile();
        }

        public ICollection<Countdown> GetCountdowns()
        {
            return _countdowns;
        }

        public void AddCountdown(Countdown countdown)
        {
            _countdowns.Add(countdown);
            WriteCountdownsToFile();
        }

        public void UpdateCountdown(Countdown countdown)
        {
            var oldCountdown = _countdowns.FirstOrDefault(c => c.Id == countdown.Id);
            if (oldCountdown != null)
            {
                _countdowns.Remove(oldCountdown);
                _countdowns.Add(countdown);
                WriteCountdownsToFile();
            }
        }

        public void UpdateCountdown(Guid id, string? title = null, string? note = null, DateTime? endTime = null, bool? isVisible = null)
        {
            // 实现映射，为null 的无需更新
            var countdown = _countdowns.FirstOrDefault(c => c.Id == id);
            if (countdown != null)
            {
                countdown.Title = title ?? countdown.Title;
                countdown.Note = note ?? countdown.Note;
                countdown.EndTime = endTime ?? countdown.EndTime;
                countdown.IsVisible = isVisible ?? countdown.IsVisible;
                WriteCountdownsToFile();
            }
        }

        public void RemoveCountdown(Countdown countdown)
        {
            if (_countdowns.Remove(countdown))
            {
                WriteCountdownsToFile();
            }

        }

        public void RemoveCountdown(Guid id)
        {
            var countdown = _countdowns.FirstOrDefault(c => c.Id == id);
            if (countdown != null)
            {
                _countdowns.Remove(countdown);
                WriteCountdownsToFile();
            }
        }


        private ICollection<Countdown> ReadCountdownsFromFile()
        {
            var countdownsFilePath = _settingService.GetSetting<string>("CountdownStickersFilePath") ?? "CountdownStickers.json";
            var countdownsJosn = _fileService.ReadFileText(countdownsFilePath) ?? "[]";
            var countdowns = JsonSerializer.Deserialize<ICollection<Countdown>>(countdownsJosn) ?? [];
            return countdowns;
        }

        private void WriteCountdownsToFile()
        {
            var countdownsFilePath = _settingService.GetSetting<string>("CountdownStickersFilePath") ?? "CountdownStickers.json";
            var countdownsJosn = JsonSerializer.Serialize(_countdowns, _jsonSerializerOptions);
            _fileService.WriteTextToFile(countdownsFilePath, countdownsJosn);
        }
    }
}
