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
        public int TotalMachine { get; set; }
        public Dictionary<string, string> Requests { get; set; } // Danh sách các yêu cầu
        public int TimeSendRequest { get; set; }
        public List<DeviceConf> devices { get; set; } // Danh sách các mã điều khiển

    }

}
