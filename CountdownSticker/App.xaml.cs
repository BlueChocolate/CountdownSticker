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

            // 只允许运行一个实例
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            if (processes.Length > 1)
            {
                Console.WriteLine("只能运行一个实例");
                Environment.Exit(1);
            }

            // 注册服务
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddSingleton<ISettingService, SettingService>();
            serviceCollection.AddSingleton<ICountdownService, CountdownService>();
            serviceCollection.AddSingleton<IWindowService, WindowsService>();
            Services = serviceCollection.BuildServiceProvider();

            var windowService = Services.GetRequiredService<IWindowService>();
            var countdownService = Services.GetRequiredService<ICountdownService>();
            windowService.SetCountdowns(countdownService.GetCountdowns());
            //windowService.ShowMainWindow();

            GlobalSettings.ChangeTheme("Dark");
        }
    }
}
