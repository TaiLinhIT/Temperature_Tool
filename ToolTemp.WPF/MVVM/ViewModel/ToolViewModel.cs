using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Core;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Services;
using ToolTemp.WPF.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ToolTemp.WPF.MVVM.ViewModel
{
    public class ToolViewModel : BaseViewModel
    {
        private SettingViewModel _settingViewModel;
        private readonly MyDbContext _context;
        public string Port = string.Empty;
        public string Factory = string.Empty;
        public string NameStyle = string.Empty;//add new item
        public int DeMax = 0;//add new item
        public int DeMin = 0;//add new item
        public int GiayMax = 0;//add new item
        public int GiayMin = 0;//add new item
        public int idStyle = 0;
        public int Baudrate = 0;
        private DispatcherTimer _dispatcherTimer;
        private readonly IToolService _toolService;
        private readonly AppSettings _appSettings;
        public MySerialPortService _mySerialPort;
        public string CurrentFactory;
        public int CurrentIdMachine;
        private readonly Dictionary<string, int> _requestIdMapping = new Dictionary<string, int>();

        public ObservableCollection<BusDataWithDevice> _temperatures;
        public ObservableCollection<BusDataWithDevice> Temperatures
        {
            get => _temperatures;
            set
            {
                _temperatures = value;
                OnPropertyChanged(nameof(Temperatures));
            }
        }
        public string FactoryCode { get; private set; }
        public int AddressCurrent { get; private set; }

        
        public void SetFactory(string factory,int address)
        {
            FactoryCode = factory;
            AddressCurrent = address;
   
            ReloadData(FactoryCode,AddressCurrent); // Trigger the data reload immediately with the new address
            StartTimer(); // Start the timer only after setting the address
        }
        //constructor
        public ToolViewModel(AppSettings appSettings,ToolService toolService, MyDbContext myDbContext)
        {
            CurrentLanguage = "en";
            _appSettings = appSettings;
            _toolService = toolService;
            _context = myDbContext;


            Temperatures = new ObservableCollection<BusDataWithDevice>
            {                
                
                new BusDataWithDevice() {Channel = _appSettings.Channel},

            };
            _dispatcherTimer = new DispatcherTimer();

            _dispatcherTimer.Interval = TimeSpan.FromSeconds(Convert.ToInt32(_appSettings.TimeReloadData));

            _dispatcherTimer.Tick += DispatcherTimer_Tick;
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            ReloadData(FactoryCode, AddressCurrent);

        }


        private async void ReloadData(string factory, int address)
        {
            if (!string.IsNullOrEmpty(Port))
            {
                List<BusDataWithDevice> data = await  _toolService.GetListDataWithDevicesAsync(Port, factory, address, Baudrate,CurrentLanguage);

                if (data != null)
                {
                    var find = data;

                    // Clear the existing items and add new ones
                    Temperatures.Clear();
                    foreach (var item in find)
                    {
                        Temperatures.Add(item);
                    }
                }
            }
        }

        public void StartTimer()
        {
            ReloadData(FactoryCode, AddressCurrent);
            _dispatcherTimer.Start();
        }

        public void StopTimer()
        {
            _dispatcherTimer.Stop();
        }


        public void Start()
        {
            _mySerialPort = new MySerialPortService();
            _mySerialPort.Port = Port;
            _mySerialPort.Baudrate = Baudrate;
            _mySerialPort.Sdre += SerialPort_DataReceived;
            _mySerialPort.Conn();
        }
        public void Close()
        {
            try
            {
                _mySerialPort.Stop();

            }
            catch (Exception ex)
            {

            }
        }

        #region Read Temperature and save database

        // Cập nhật hàng đợi để lưu cả dữ liệu và IdMachine
        private readonly Queue<(byte[] Data, int IdMachine)> _dataQueue = new Queue<(byte[] Data, int IdMachine)>();
        private readonly object _queueLock = new object();

        // Sự kiện nhận dữ liệu từ SerialPort
        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                int bytesCount = sp.BytesToRead;
                byte[] bufferb = new byte[bytesCount];
                sp.Read(bufferb, 0, bytesCount);


                // Tìm IdMachine từ ánh xạ
                int idMachine = _context.machines.Where(x => x.Address == (int)bufferb[0]).Select(x => x.Id).FirstOrDefault();

                // Đưa dữ liệu và IdMachine vào hàng đợi
                lock (_queueLock)
                {
                    _dataQueue.Enqueue((bufferb, idMachine));
                }

                // Xử lý hàng đợi
                await ProcessDataQueueAsync();
            }
            catch (Exception ex)
            {
                Tool.Log($"DataReceived Error: {ex.Message}");
                if (ex.StackTrace != null)
                    Tool.Log(ex.StackTrace.ToString());
            }
        }


        // Xử lý dữ liệu trong hàng đợi
        private async Task ProcessDataQueueAsync()
        {
            while (_dataQueue.Count > 0)
            {
                (byte[] Data, int IdMachine) item;

                // Lấy dữ liệu và IdMachine từ hàng đợi
                lock (_queueLock)
                {
                    if (_dataQueue.Count == 0) break;
                    item = _dataQueue.Dequeue();
                }

                byte[] data = item.Data;
                int idMachine = item.IdMachine;

                // Kiểm tra CRC
                if (Tool.CRC_PD(data))
                {
                    // Nếu Function Code là 03 (Read Data)
                    if (data[1] == 3)
                    {
                        try
                        {
                            int address = data[0];
                            string factory = FactoryCode;
                            var modBusDTO = new BusDataTemp();

                            // Xử lý dữ liệu trả về
                            byte[] bytes = new byte[] { data[4], data[3], data[6], data[5] };
                            float temp = BitConverter.ToSingle(bytes, 0);
                            if (temp >= 999)
                            {
                                return; // Bỏ qua nếu dữ liệu không hợp lệ
                            }

                            // Thiết lập các thuộc tính của modBusDTO
                            modBusDTO.Temp = (double)temp;
                            var channel = _mySerialPort.MapByteResponeToChannel(data[2], _appSettings);
                            modBusDTO.Channel = channel;
                            modBusDTO.Port = _appSettings.Port;
                            modBusDTO.Factory = factory;
                            modBusDTO.Baudrate = Baudrate;
                            modBusDTO.AddressMachine = address;
                            modBusDTO.IdStyle = idStyle;
                            modBusDTO.Sensor_Typeid = 7;
                            modBusDTO.UploadDate = DateTime.Now;
                            var find = _context.Style.Where(x => x.Id == idStyle).ToList();
                            int lastChannel = int.Parse(channel[channel.Length - 1].ToString());
                            // Kiểm tra số thứ tự và so sánh nhiệt độ
                            switch (lastChannel % 2) // % 2 kiểm tra chẵn lẻ
                            {
                                case 1: // Số lẻ
                                    modBusDTO.Max = DeMax;
                                    modBusDTO.Min = DeMin;
                                    modBusDTO.IsWarning = (decimal)temp < find.First().DeMin || (decimal)temp > find.First().DeMax;
                                    break;

                                case 0: // Số chẵn
                                    modBusDTO.Max = GiayMax;
                                    modBusDTO.Min = GiayMin;
                                    modBusDTO.IsWarning = (decimal)temp < find.First().GiayMin || (decimal)temp > find.First().GiayMax;
                                    break;

                                default:
                                    modBusDTO.IsWarning = false; // Trường hợp mặc định
                                    break;
                            }

                            // Lưu IdMachine và Line
                            modBusDTO.IdMachine = idMachine;
                            modBusDTO.LineCode = _context.machines.Where(x => x.Id == idMachine).Select(x => x.LineCode).FirstOrDefault();
                            modBusDTO.Line = _context.machines
                                .Where(x => x.Id == idMachine)
                                .Select(x => x.Line)
                                .FirstOrDefault();

                            // Lưu dữ liệu vào cơ sở dữ liệu
                            await Task.Run(async () =>
                            {
                                try
                                {
                                    await _toolService.InsertData(modBusDTO);
                                }
                                catch (Exception insertEx)
                                {
                                    Tool.Log($"Error saving data: {insertEx.Message}");
                                    if (insertEx.StackTrace != null)
                                        Tool.Log(insertEx.StackTrace.ToString());
                                }
                            });
                        }
                        catch (Exception processEx)
                        {
                            Tool.Log($"Error processing data: {processEx.Message}");
                            if (processEx.StackTrace != null)
                                Tool.Log(processEx.StackTrace.ToString());
                        }
                    }
                }
                else
                {
                    Tool.Log($"Invalid data: {BitConverter.ToString(data)}");
                }
            }


        }

        #endregion

        public async Task GetTempFromMachine(int address, int idMachine)
        {
            while (true)
            {
                var listChannel = _appSettings.ConfigCommand;
                foreach (var item in listChannel)
                {
                    ModbusSendFunction("0" + address + item.AddressWrite, idMachine);
                    await Task.Delay(TimeSpan.FromSeconds(Convert.ToInt32(_appSettings.TimeBusTemp)));
                }
            }
        }


        private void ModbusSendFunction(string decimalString, int idMachine)
        {
            try
            {
                var hexWithCRC = Helper.CalculateCRC(decimalString);
                var message = hexWithCRC.Replace(" ", ""); // Chuỗi đầy đủ với CRC

                
                // Gửi message qua SerialPort
                _mySerialPort.Write(message);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Errors: {ex.Message}");
                return;
            }
        }


        #region Language
        private string _currentLanguage;
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                OnPropertyChanged(nameof(CurrentLanguage));
            }
        }
        #endregion
    }
}
