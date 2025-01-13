using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolTemp.WPF.Core;

namespace ToolTemp.WPF.MVVM.ViewModel
{
    public class MenuItemViewModel : BaseViewModel
    {
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }
}
