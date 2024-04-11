using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CountdownSticker.Messages;
using CountdownSticker.Messenger.Messages;
using CountdownSticker.Models;
using System.Collections.ObjectModel;

namespace CountdownSticker.ViewModels
{
    public partial class MainViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<Countdown> _stickers;

        [ObservableProperty]
        private int _activeCountdownCount;

        partial void OnStickersChanged(ObservableCollection<Countdown> value)
        {
            ActiveCountdownCount = Stickers.Count;
        }



        public MainViewModel()
        {
            //Stickers.CollectionChanged += (sender, e) =>
            //{
            //    ActiveCountdownCount = Stickers.Count;
            //};
            Stickers = [];
        }

        [RelayCommand]
        public void AddSticker()
        {
            var sticker = new Countdown();
            Stickers.Add(sticker);
            WeakReferenceMessenger.Default.Send(new StickersChangedMessage(Stickers));

        }

        [RelayCommand]
        public void RemoveSticker(Countdown sticker)
        {
            Stickers.Remove(sticker);
        }
    }
}
