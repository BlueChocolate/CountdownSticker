using Accessibility;
using CommunityToolkit.Mvvm.Messaging;
using CountdownSticker.Messages;
using CountdownSticker.ViewModels;
using CountdownSticker.Views;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownSticker.Recipients
{
    public class OpenWindowRecipient : IRecipient<OpenWindowMessage>, IRecipient<StickersChangedMessage>
    {
        private WindowX _mainWindow;
        private List<WindowX> _stickerWindows;

        public OpenWindowRecipient()
        {
            _mainWindow = new MainWindow();
            _mainWindow.DataContext = new MainViewModel();

            _stickerWindows = new List<WindowX>();
        }

        public void Receive(OpenWindowMessage message)
        {
            if (message.Value == typeof(MainWindow))
            {
                _mainWindow.Show();
            }
            else if (message.Value == typeof(StickerWindow))
            {
                var stickerWindow = new StickerWindow();
                //stickerWindow.DataContext = new StickerViewModel();
                stickerWindow.Closed += (sender, e) => { _stickerWindows.Remove(stickerWindow); };
                _stickerWindows.Add(stickerWindow);
                stickerWindow.Show();
            }
        }

        public void Receive(StickersChangedMessage message)
        {

        }
    }
}