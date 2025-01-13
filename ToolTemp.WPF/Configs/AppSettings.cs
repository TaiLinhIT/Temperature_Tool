using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolTemp.WPF.Models;

namespace ToolTemp.WPF.Configs
{
    public class AppSettings
    {
        public string ConnectString { get; set; }
        public List<ChannelMachine> ConfigCommand { get; set; } = new List<ChannelMachine>(); // Initialize the list
        public int TimeReloadData { get; set; }
        public int TimeBusTemp { get; set; }
        public string CurrentArea { get; set; }
        public string Port { get; set; }
        public int Baudrate { get; set; }
        public string Channel { get; set; }
    }

}
