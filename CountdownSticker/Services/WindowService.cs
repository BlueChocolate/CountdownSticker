﻿using CountdownSticker.Models;
using CountdownSticker.ViewModels;
using CountdownSticker.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace CountdownSticker.Services
{
    public interface IWindowService
    {
        public void ShowMainWindow();
        public void HideMainWindow();
        public void SetCountdowns(ICollection<Countdown> countdowns);
        public void AddCountdown(Countdown countdown);
        public void UpdateCountdown(Countdown countdown);
        public void UpdateCountdown(StickerViewModel stickerViewModel);
        public void RemoveCountdown(Guid id);
    }

    public class WindowsService : IWindowService
    {
        private readonly ICountdownService _countdownService;
        private MainWindowX _mainWindow;
        private Dictionary<Guid, StickerWindowX> _stickerWindows;
        private bool _isLocked;
        private readonly object _lock;
        private readonly bool isMainWindowInitialized;


        public WindowsService()
        {
            _countdownService = App.Services.GetRequiredService<ICountdownService>();
            _stickerWindows = [];
            _isLocked = false;
            _lock = new object();
            _mainWindow = new MainWindowX();
            _mainWindow.Closing += (sender, e) =>
            {
                e.Cancel = true;
                ((MainWindowX)sender!).Visibility = Visibility.Hidden;
            };
            isMainWindowInitialized = false;
            _ = TimerElapsed();
        }

        public void ShowMainWindow()
        {
            if (!isMainWindowInitialized)
            {
                _mainWindow.DataContext = new MainViewModel();
            }
            _mainWindow!.Show();
        }

        public void HideMainWindow()
        {
            _mainWindow!.Hide();
        }

        public void SetCountdowns(ICollection<Countdown> countdowns)
        {
            foreach (var countdown in countdowns)
            {
                AddCountdown(countdown);
            }
        }

        public void AddCountdown(Countdown countdown)
        {
            // Id 一定不会重复
            var stickerWindows = _stickerWindows[countdown.Id] = new StickerWindowX()
            {
                DataContext = new StickerViewModel(countdown.Id, countdown.Title, countdown.Note, countdown.EndTime, countdown.Remaining, countdown.IsActive, countdown.IsVisible),
                WindowStartupLocation = WindowStartupLocation.Manual
            };
            stickerWindows.SizeChanged += (sender, e) =>
            {
                AlignAndShow();
            };
            AlignAndShow();
        }

        public void UpdateCountdown(Countdown countdown)
        {
            if (_stickerWindows.TryGetValue(countdown.Id, out StickerWindowX? value))
            {
                value.DataContext = new StickerViewModel(countdown.Id, countdown.Title, countdown.Note, countdown.EndTime, countdown.Remaining, countdown.IsActive, countdown.IsVisible);
                AlignAndShow();
            }
        }

        public void UpdateCountdown(StickerViewModel stickerViewModel)
        {
            if (_stickerWindows.TryGetValue(stickerViewModel.Id, out StickerWindowX? value))
            {
                //value.DataContext = stickerViewModel; // 好像没必要
                AlignAndShow();
            }
        }

        public void RemoveCountdown(Guid id)
        {
            if (_stickerWindows.TryGetValue(id, out StickerWindowX? value))
            {
                value.Close();
                _stickerWindows.Remove(id);
                AlignAndShow();
            }
        }

        public void AlignAndShow()
        {
            var screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            var screenHeight = (int)SystemParameters.PrimaryScreenHeight;
            var horizontalSpacing = 30;
            var verticalSpacing = 30;
            int columns = 1;
            int nextHeight = verticalSpacing;

            Volatile.Write(ref _isLocked, true);
            _stickerWindows = _stickerWindows.OrderBy(x => ((StickerViewModel)x.Value.DataContext).EndTime).ToDictionary(x => x.Key, x => x.Value).ToDictionary();
            foreach (var stickerWindow in _stickerWindows.Values)
            {
                if (((StickerViewModel)stickerWindow.DataContext).IsVisible)
                {
                    if (!stickerWindow.IsVisible)
                    {
                        stickerWindow.Show();
                    }
                    if (nextHeight + (int)stickerWindow.ActualHeight > screenHeight)
                    {
                        columns++;
                        nextHeight = verticalSpacing;
                    }
                    stickerWindow.Left = screenWidth - columns * (horizontalSpacing + (int)stickerWindow.ActualWidth);
                    stickerWindow.Top = nextHeight;
                    nextHeight += verticalSpacing + (int)stickerWindow.ActualHeight;
                }
                else
                {
                    stickerWindow.Hide();
                }
            }
            Volatile.Write(ref _isLocked, false);
        }

        private async Task TimerElapsed()
        {
            while (true)
            {
                if (!Volatile.Read(ref _isLocked))
                {
                    foreach (var stickerWindow in _stickerWindows.Values)
                    {
                        if (!Application.Current.Dispatcher.CheckAccess())
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (((StickerViewModel)stickerWindow.DataContext).IsVisible)
                                {
                                    var dataContext = (StickerViewModel)stickerWindow.DataContext;
                                    if (dataContext.IsVisible)
                                    {
                                        dataContext.Remaining = dataContext.EndTime - DateTime.Now;
                                    }
                                }
                            });
                        }
                    }
                }
                await Task.Delay(1);
            }
        }
    }
}
