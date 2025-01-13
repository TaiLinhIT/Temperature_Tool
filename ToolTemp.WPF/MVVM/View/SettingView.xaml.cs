using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToolTemp.WPF.MVVM.ViewModel;
using ToolTemp.WPF.Services;


namespace ToolTemp.WPF.MVVM.View
{
    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : UserControl, INotifyPropertyChanged
    {
        
        public SettingView()
        {
            
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Kiểm tra nếu ký tự nhập vào có phải là số không
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            // Kiểm tra nếu tất cả ký tự trong chuỗi đều là số
            return int.TryParse(text, out _);
        }

        private void txb_address_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}

