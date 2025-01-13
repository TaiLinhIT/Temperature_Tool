using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.MVVM.ViewModel;
using ToolTemp.WPF.Services;

namespace ToolTemp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
           

            // Đăng ký sự kiện đóng form
            Closing += MainWindow_Closing;

            
        }

        // Sự kiện đóng cửa sổ
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            // Đóng toàn bộ ứng dụng khi MainWindow bị đóng
            Application.Current.Shutdown();
        }

    }
}
