using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.MVVM.View;
using ToolTemp.WPF.MVVM.ViewModel;
using ToolTemp.WPF.Services;

namespace ToolTemp.WPF
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Khởi tạo Startup
            var startup = new Startup();
            _serviceProvider = startup.ServiceProvider;

            // Khởi tạo MainWindow với DI
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
