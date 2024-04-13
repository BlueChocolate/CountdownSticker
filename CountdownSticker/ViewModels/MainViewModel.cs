using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CountdownSticker.Models;
using CountdownSticker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.Json;

namespace CountdownSticker.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;
        private readonly ICountdownService _countdownService;


        [ObservableProperty]
        private ObservableCollection<Countdown> _countdowns;

        [ObservableProperty]
        private int _activeCountdownCount;

        public MainViewModel()
        {
            _windowService = App.Services.GetRequiredService<IWindowService>();
            _countdownService = App.Services.GetRequiredService<ICountdownService>();
            Countdowns = new ObservableCollection<Countdown>(_countdownService.GetCountdowns());
            Countdowns.CollectionChanged += CountdownsCollectionChanged;
        }

        private void CountdownsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ActiveCountdownCount = Countdowns.Count(c => c.IsActive);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    //Debug.WriteLine("NotifyCollectionChangedAction.Add");
                    if (e.NewItems != null)
                    {
                        foreach (var countdown in e.NewItems.OfType<Countdown>())
                        {
                            _countdownService.AddCountdown(countdown);
                            _windowService.AddCountdown(countdown);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    //Debug.WriteLine("NotifyCollectionChangedAction.Remove");
                    if (e.OldItems != null)
                    {
                        foreach (var countdown in e.OldItems.OfType<Countdown>())
                        {
                            _countdownService.RemoveCountdown(countdown);
                            _windowService.RemoveCountdown((countdown).Id);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    //Debug.WriteLine("NotifyCollectionChangedAction.Replace");
                    if (e.OldItems != null && e.NewItems != null)
                    {
                        foreach (var countdown in e.NewItems.OfType<Countdown>())
                        {
                            _countdownService.UpdateCountdown(countdown);
                            _windowService.UpdateCountdown((countdown));
                        }
                    }
                    break;
            }

        }

        [RelayCommand]
        public void AddCountdown()
        {
            Countdowns.Add(new Countdown());
            //Countdowns = new ObservableCollection<Countdown>(Countdowns.OrderBy(c => c.LastModified).ToList());
            //Countdowns.CollectionChanged += CountdownsCollectionChanged;
        }

        [RelayCommand]
        public void RemoveCountdown(Countdown countdown)
        {
            Countdowns.Remove(countdown);
        }

        [RelayCommand]
        public void SaveCountdown(Countdown countdown)
        {
            var oldCountdown = Countdowns.FirstOrDefault(c => c.Id == countdown.Id);
            if (oldCountdown != null)
            {
                countdown.LastModified = DateTime.Now;
                Countdowns.Remove(oldCountdown);
                Countdowns.Add(countdown);
            }

            //for (int i = 0; i < Countdowns.Count; i++)
            //{
            //    if (Countdowns[i].Id == countdown.Id)
            //    {
            //        Countdowns[i] = countdown;
            //        break;
            //    }
            //}
            //Countdowns = new ObservableCollection<Countdown>(Countdowns.OrderBy(c => c.LastModified).ToList());
            //Countdowns.CollectionChanged += CountdownsCollectionChanged;
        }
    }
}