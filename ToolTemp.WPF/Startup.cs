using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.MVVM.View;
using ToolTemp.WPF.MVVM.ViewModel;
using ToolTemp.WPF.Services;

namespace ToolTemp.WPF
{
    public class Startup
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public Startup()
        {
            // Đọc cấu hình từ appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Bind AppSettings with configuration from appsettings.json
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            services.AddSingleton(appSettings);
            // Register MyDbContext as scoped
            services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(appSettings.ConnectString));

            // Register view models and other services
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<SettingViewModel>();
            services.AddSingleton<ToolViewModel>();
            services.AddSingleton<MySerialPortService>();


            // Register services
            services.AddSingleton<ToolService>();
        }

    }
}
