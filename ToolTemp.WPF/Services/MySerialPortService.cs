using System.Diagnostics;
using System.IO.Ports;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Utils;

namespace ToolTemp.WPF.Services
{
    public class MySerialPortService
    {
        public SerialPort _serialPort;
        public event SerialDataReceivedEventHandler Sdre;
        public string Port;
        public int Baudrate;
        bool _continue;
        Thread readThread;
        private AppSettings _appsetting;
        private readonly Dictionary<string, int> _requestIdMapping = new Dictionary<string, int>();

        public MySerialPortService()
        {

            _appsetting = new AppSettings();

        }
        public void Conn()
        {
            _serialPort = new SerialPort();
            _serialPort.DataReceived += Sdre;
            _serialPort.ErrorReceived += _serialPort_ErrorReceived;
            _serialPort.PinChanged += _serialPort_PinChanged;

            _serialPort.PortName = Port;
            _serialPort.BaudRate = Baudrate;
            _serialPort.DataBits = 8;//数据长度：
            _serialPort.StopBits = StopBits.One;//停止位
            _serialPort.Handshake = Handshake.None;
            _serialPort.Parity = Parity.None;//校验方式
            _serialPort.ReadTimeout = 200; //设置超时读取时间
            _serialPort.WriteTimeout = 100;
            _serialPort.RtsEnable = true;
            try
            {
                _serialPort.Open();

            }
            catch (Exception e)
            {
                Tool.Log(string.Format("端口{0}打开失败:{1}", Port, e));
            }
        }

        private void _serialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Tool.Log(e.ToString());
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Tool.Log(e.ToString());
        }

        public void Stop()
        {
            _continue = false; _serialPort.Close(); _serialPort.Dispose();
        }

        public void Read()
        {
            try
            {
                _serialPort.Open();
                _continue = true;
            }
            catch (Exception e)
            {
                _continue = false;
                Debug.WriteLine(e);
            }

            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Debug.WriteLine(message);
                }
                catch (Exception e)
                {
                    _continue = false;
                    Debug.WriteLine(e);
                }
                Thread.Sleep(100);
            }
            readThread.Join();
            _serialPort.Close();
        }
        ~MySerialPortService()
        {
            _serialPort.Close();
        }

        public void Write(string hexData)
        {
            try
            {
                // Gửi requestName trước
                //byte[] requestNameBytes = Encoding.ASCII.GetBytes(requestName);
                //_serialPort.Write(requestNameBytes, 0, requestNameBytes.Length);

                // Tách dữ liệu hex và gửi từng byte
                byte[] data = new byte[1];
                string strs = hexData.Replace(" ", "").Replace("\r", "").Replace("\n", "");

                if (strs.Length % 2 == 1)
                {
                    strs = strs.Insert(strs.Length - 1, "0");
                }

                foreach (char c in strs)
                {
                    if (!Uri.IsHexDigit(c))
                    {
                        throw new FormatException($"Ký tự không hợp lệ trong chuỗi hex: {c}");
                    }
                }

                for (int i = 0; i < strs.Length / 2; i++)
                {
                    data[0] = Convert.ToByte(strs.Substring(i * 2, 2), 16);
                    _serialPort.Write(data, 0, 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                throw;
            }
        }
        public bool IsOpen()
        {
            return _serialPort.IsOpen;
        }

        /// <summary>
        /// Customize hard code
        /// </summary>
        /// <param name="bufferb"></param>
        /// <returns></returns>
        public string MapByteResponeToChannel(byte bufferb, AppSettings appSettings)
        {
            // Ensure appSettings is not null
            if (appSettings == null || appSettings.ConfigCommand == null)
            {
                throw new ArgumentNullException(nameof(appSettings), "AppSettings or ConfigCommand is null");
            }

            var channelName = string.Empty;

            // Mapping bufferb to index (4 -> 0, 6 -> 1, etc.)
            int index = (bufferb - 4) / 2;

            // Check if the index is valid and within range of the ConfigCommand list
            if (index >= 0 && index < appSettings.ConfigCommand.Count)
            {
                channelName = appSettings.ConfigCommand[index].Channel;
            }
            else
            {
                // Handle case where the bufferb value doesn't map to a valid index
                channelName = "Unknown Channel";
            }

            return channelName;
        }


    }
}
