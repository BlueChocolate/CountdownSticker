using CommunityToolkit.Mvvm.Messaging;
using CountdownSticker.Messages;
using CountdownSticker.Messenger.Messages;
using CountdownSticker.Messenger.Recipients;
using CountdownSticker.Views;
using Panuon.WPF.UI;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace CountdownSticker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static RecipientManager RecipientManager { get; private set; } = new RecipientManager();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RecipientManager
                .Register<FileRecipient, StickersChangedMessage>()
                .Register<OpenWindowRecipient, OpenWindowMessage>();

            WeakReferenceMessenger.Default.Send(new OpenWindowMessage(typeof(MainWindow)));
            GlobalSettings.ChangeTheme("Dark");

            Debug.WriteLine("App 初始化完成");
        }
    }
}
