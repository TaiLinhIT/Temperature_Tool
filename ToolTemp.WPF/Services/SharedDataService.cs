using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolTemp.WPF.Services
{
    public class SharedDataService
    {
        private string _sharedData;
        public string SharedData
        {
            get => _sharedData;
            set => _sharedData = value;
        }
    }
}
