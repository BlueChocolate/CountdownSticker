using CountdownSticker.Models;
using CountdownSticker.ViewModels;
using CountdownSticker.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace CountdownSticker.Services
{
    public interface IWindowService
    {
        public void Initialization();
        public void ShowMainWindow();
        public void HideMainWindow();
        public void AddCountdown(Countdown countdown);
        public void UpdateCountdown(Countdown countdown);
        public void UpdateCountdown(StickerViewModel stickerViewModel);
        public void RemoveCountdown(Guid id);
    }

    public class WindowsService : IWindowService
    {
        private readonly ICountdownService _countdownService;
        private readonly int leftPosition;
        private MainWindowX? _mainWindow;
        private Dictionary<Guid, StickerWindowX> _stickerWindows;
        private readonly Timer _timer;

        public WindowsService()
        {
            _countdownService = App.Services.GetRequiredService<ICountdownService>();
            leftPosition = (int)SystemParameters.PrimaryScreenWidth - 30 - 300;
            _stickerWindows = [];
            _timer = new Timer(TimerElapsed, null, 0, 1000);
        }

        public void Initialization()
        {
            if (_mainWindow == null)
            {
                _mainWindow = new MainWindowX();
                _mainWindow.DataContext = new MainViewModel();
                _mainWindow.Closing += (sender, e) =>
                {
                    e.Cancel = true;
                    ((MainWindowX)sender!).Visibility = Visibility.Hidden;
                };
            }
        }

        public void ShowMainWindow()
        {
            if (_mainWindow == null)
            {
                Initialization();
            }
            _mainWindow!.Show();
        }

        public void HideMainWindow()
        {
            if (_mainWindow == null)
            {
                Initialization();
            }
            _mainWindow!.Hide();

        }

        public void AddCountdown(Countdown countdown)
        {
            // Id 一定不会重复
            _stickerWindows[countdown.Id] = new StickerWindowX()
            {
                DataContext = new StickerViewModel(countdown.Id, countdown.Title, countdown.Note, countdown.EndTime, countdown.Remaining, countdown.IsActive, countdown.IsVisible),
                WindowStartupLocation = WindowStartupLocation.Manual
            };
            FixWindowPosition();
            if (countdown.IsVisible)
            {
                _stickerWindows[countdown.Id].Show();
            }
        }

        public void UpdateCountdown(Countdown countdown)
        {
            if (_stickerWindows.TryGetValue(countdown.Id, out StickerWindowX? value))
            {
                value.DataContext = new StickerViewModel(countdown.Id, countdown.Title, countdown.Note, countdown.EndTime, countdown.Remaining, countdown.IsActive, countdown.IsVisible);
                FixWindowPosition();
                if (countdown.IsVisible)
                {
                    value.Show();
                }
                else
                {
                    value.Hide();
                }
            }
        }

        public void UpdateCountdown(StickerViewModel stickerViewModel)
        {
            if (_stickerWindows.TryGetValue(stickerViewModel.Id, out StickerWindowX? value))
            {
                //value.DataContext = stickerViewModel; // 好像没必要
                FixWindowPosition();
                if (!value.IsVisible && stickerViewModel.IsVisible)
                {
                    value.Show();
                }
                else
                {
                    value.Hide();
                }
            }
        }

        public void RemoveCountdown(Guid id)
        {
            if (_stickerWindows.TryGetValue(id, out StickerWindowX? value))
            {
                value.Close();
                _stickerWindows.Remove(id);
                FixWindowPosition();
            }
        }

        public void FixWindowPosition()
        {
            int index = 0;
            _stickerWindows = _stickerWindows.OrderBy(x => ((StickerViewModel)x.Value.DataContext).EndTime).ToDictionary(x => x.Key, x => x.Value).ToDictionary();
            foreach (var stickerWindow in _stickerWindows.Values)
            {
                if (((StickerViewModel)stickerWindow.DataContext).IsVisible)
                {
                    stickerWindow.Top = 30 + index++ * 165;
                    stickerWindow.Left = leftPosition;
                }
            }
        }

        private void TimerElapsed(object? state)
        {
            foreach (var stickerWindow in _stickerWindows.Values)
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var dataContext = (StickerViewModel)stickerWindow.DataContext;
                        if (dataContext.IsVisible)
                        {
                            dataContext.Remaining = dataContext.EndTime - DateTime.Now;
                        }
                    });
                }
            }
        }
    }
}
