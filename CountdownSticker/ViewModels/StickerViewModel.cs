using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CountdownSticker.Services;
using CountdownSticker.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CountdownSticker.ViewModels
{
    public partial class StickerViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;
        private readonly ICountdownService _countdownService;

        [ObservableProperty]
        private Guid _id;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string? _note;

        [ObservableProperty]
        private DateTime _endTime;

        [ObservableProperty]
        private TimeSpan _remaining;

        [ObservableProperty]
        private bool _isActive;

        [ObservableProperty]
        private bool _isVisible;

        [RelayCommand]
        void HideCountdown()
        {
            IsVisible = false;
            _windowService.UpdateCountdown(this);
            _countdownService.UpdateCountdown(id: Id, isVisible: false);
        }


        [RelayCommand]
        public void ShowMainWindow()
        {
            _windowService.ShowMainWindow();
        }

        public StickerViewModel(Guid id, string title, string? note, DateTime endTime, TimeSpan remaining, bool isActive, bool isVisible)
        {
            _windowService = App.Services.GetRequiredService<IWindowService>();
            _countdownService = App.Services.GetRequiredService<ICountdownService>();

            Id = id;
            Title = title;
            Note = note;
            EndTime = endTime;
            Remaining = remaining;
            IsActive = isActive;
            IsVisible = isVisible;
        }
    }
}
