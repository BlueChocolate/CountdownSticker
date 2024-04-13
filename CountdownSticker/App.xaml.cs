using CommunityToolkit.Mvvm.Messaging;
using CountdownSticker.Models;
using CountdownSticker.Services;
using CountdownSticker.Views;
using Microsoft.Extensions.DependencyInjection;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.Windows;

namespace CountdownSticker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddSingleton<ISettingService, SettingService>();
            serviceCollection.AddSingleton<ICountdownService, CountdownService>();
            serviceCollection.AddSingleton<IWindowService, WindowsService>();
            Services = serviceCollection.BuildServiceProvider();

            var windowService = Services.GetRequiredService<IWindowService>();
            windowService.Initialization();
            windowService.ShowMainWindow();

            var settingService = Services.GetRequiredService<ISettingService>();
            settingService.SetSetting("CountdownStickersFilePath", "C:\\Users\\RadioNoise\\OneDrive\\便笺\\CountdownStickers.json");
            settingService.SetSetting("Theme", "Dark");

            GlobalSettings.ChangeTheme("Dark");
            Debug.WriteLine("App 初始化完成");
        }
    }
}
