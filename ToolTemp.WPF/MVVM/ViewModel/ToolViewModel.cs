using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Threading;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Core;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Services;
using ToolTemp.WPF.Utils;

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
        private readonly SemaphoreSlim _serialLock = new(1, 1);// SemaphoreSlim để đồng bộ hóa truy cập vào cổng COM

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


        public void SetFactory(string factory, int address)
        {
            FactoryCode = factory;
            AddressCurrent = address;

            //ReloadData(FactoryCode, AddressCurrent); // Trigger the data reload immediately with the new address
            StartTimer(); // Start the timer only after setting the address
        }
        //constructor
        public ToolViewModel(AppSettings appSettings, ToolService toolService, MyDbContext myDbContext)
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

            //ReloadData(FactoryCode, AddressCurrent);

        }


        private async void ReloadData(string factory, int address)
        {
            if (!string.IsNullOrEmpty(Port))
            {
                List<BusDataWithDevice> data = await _toolService.GetListDataWithDevicesAsync(Port, factory, address, Baudrate, CurrentLanguage);

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
            //ReloadData(FactoryCode, AddressCurrent);
            _dispatcherTimer.Start();
        }

        public void StopTimer()
        {
            _dispatcherTimer.Stop();
        }


        public async void Start()
        {
            _mySerialPort = new MySerialPortService();
            _mySerialPort.Port = Port;
            _mySerialPort.Baudrate = Baudrate;
            _mySerialPort.Sdre += SerialPort_DataReceived;
            _mySerialPort.Conn();
            Tool.Log($"Serial port started on {Port} with baudrate {Baudrate} status {_mySerialPort.IsOpen}");
            await SendRequestsToAllAddressesAsync(); // Gửi yêu cầu đến tất cả các địa chỉ máy móc

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
        public async Task SendRequestsToAllAddressesAsync()
        {
            for (int address = 1; address <= _appSettings.TotalMachine; address++)
            {
                //SetFactory("VB2", address);
                int capturedAddress = address; // tránh closure issue
                _ = Task.Run(() => LoopRequestsForMachineAsync(capturedAddress));
            }
        }
        private async Task LoopRequestsForMachineAsync(int address)
        {
            while (true)
            {
                //Tool.Log($"Máy {address}: Bắt đầu gửi dữ liệu");

                foreach (var request in _appSettings.Requests)
                {
                    string requestName = $"{request.Key}_Address_{address}";
                    await SendRequestAsync(requestName, request.Value, address);
                    await Task.Delay(1000);
                }

                await Task.Delay(TimeSpan.FromMinutes(_appSettings.TimeSendRequest)); // Hoặc dùng _appSetting.TimeReloadData
            }
        }
        private Dictionary<string, CancellationTokenSource> responseTimeouts = new Dictionary<string, CancellationTokenSource>();
        private static readonly object lockObject = new object();


        #region Gửi request
        private async Task SendRequestAsync(string requestName, string requestHex, int address)
        {
            try
            {
                await _serialLock.WaitAsync(); //Chỉ 1 máy được gửi tại 1 thời điểm

                // B1: Thêm vào activeRequests
                string requestKey = $"{address}_{requestName}";
                if (!activeRequests.ContainsKey(requestKey))
                {
                    activeRequests[requestKey] = requestName;

                    //Thiết lập timeout nếu cần
                    var cts = new CancellationTokenSource();
                    responseTimeouts[address.ToString()] = cts;
                    _ = StartResponseTimeoutAsync(address.ToString(), cts.Token);
                }

                // B2: Xử lý dữ liệu hex
                byte[] requestBytes = _toolService.ConvertHexStringToByteArray(requestHex);
                string addressHex = _toolService.ConvertToHex(address).PadLeft(2, '0');
                string requestString = addressHex + " " + BitConverter.ToString(requestBytes).Replace("-", " ");
                string CRCString = Helper.CalculateCRC(requestString);
                requestString += " " + CRCString;

                // B3: Gửi
                _mySerialPort.Write(requestString);
                //Tool.Log($"Máy {address} gửi {requestName}: {requestString}");

                await Task.Delay(1000); // Chờ thiết bị phản hồi
            }
            catch (Exception ex)
            {
                Tool.Log($"Lỗi gửi request {requestName}: {ex.Message}");
            }
            finally
            {
                _serialLock.Release(); //Giải phóng cho máy khác gửi
            }
        }
        private async Task StartResponseTimeoutAsync(string addressKey, CancellationToken cancellationToken)
        {
            try
            {
                int timeoutSeconds = _appSettings.TimeSendRequest; // đảm bảo bạn đã config nó trong appsettings.json

                await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cancellationToken);

                // Nếu không bị hủy, nghĩa là timeout xảy ra
                if (activeRequests.Keys.Any(k => k.StartsWith($"{addressKey}_")))
                {
                    //Tool.Log($"Timeout: Không nhận được phản hồi từ máy có địa chỉ {addressKey} sau {timeoutSeconds} giây.");
                    activeRequests = activeRequests
                        .Where(kvp => !kvp.Key.StartsWith($"{addressKey}_"))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
            }
            catch (TaskCanceledException)
            {
                // Bị huỷ đúng cách do có phản hồi đến
                Tool.Log($"Máy {addressKey} đã phản hồi đúng hạn.");
            }
            catch (Exception ex)
            {
                Tool.Log($"Lỗi khi xử lý timeout cho địa chỉ {addressKey}: {ex.Message}");
            }
        }

        #endregion

        //private Dictionary<string, string> activeRequests = new Dictionary<string, string>();// đối tượng dùng làm khóa
        private Dictionary<string, string> activeRequests = new Dictionary<string, string>(); // key = "address_requestName"

        // Biến lưu trạng thái các request đã nhận
        private readonly Dictionary<string, double> receivedData = new Dictionary<string, double>();

        private Dictionary<int, Dictionary<string, double>> receivedDataByAddress = new Dictionary<int, Dictionary<string, double>>();
        private HashSet<string> processedRequests = new HashSet<string>();
        #region Nhận dữ liệu
        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                int bytesCount = sp.BytesToRead;
                byte[] bufferb = new byte[bytesCount];
                sp.Read(bufferb, 0, bytesCount);

                // Tìm IdMachine từ Address (byte đầu tiên là địa chỉ thiết bị)
                int address = bufferb[0];


                // Lặp qua các activeRequests để tìm đúng request
                var matchedRequest = activeRequests.FirstOrDefault(kvp => kvp.Key.StartsWith($"{address}_"));

                if (!string.IsNullOrEmpty(matchedRequest.Key))
                {
                    string requestName = matchedRequest.Value;
                    string requestKey = matchedRequest.Key;

                    // Tránh xử lý trùng trong cùng một lần nhận
                    if (processedRequests.Contains(requestKey))
                    {
                        Tool.Log($"Data for {requestName} at address {address} already processed. Skipping...");
                        return;
                    }

                    // Đánh dấu là đã xử lý
                    processedRequests.Add(requestKey);

                    // Hủy timeout nếu có
                    if (responseTimeouts.ContainsKey(address.ToString()))
                    {
                        responseTimeouts[address.ToString()].Cancel();
                        responseTimeouts.Remove(address.ToString());
                    }

                    activeRequests.Remove(requestKey);

                    // Gọi hàm xử lý
                    ParseAndStoreReceivedData(bufferb, requestName, address);

                    // XÓA KEY để lần sau vẫn xử lý được
                    processedRequests.Remove(requestKey);
                }
                else
                {
                    Tool.Log($"Received data from address {address} does not match any active request.");
                }
            }
            catch (Exception ex)
            {
                Tool.Log($"DataReceived Error: {ex.Message}");
                if (ex.StackTrace != null)
                    Tool.Log(ex.StackTrace.ToString());
            }
        }


        #endregion
        #region Dịch dữ liệu
        private void ParseAndStoreReceivedData(byte[] data, string requestName, int address)
        {
            try
            {
                if (data.Length >= 9)
                {
                    int dataByteCount = data[2];
                    if (dataByteCount != 4 || data.Length < 5 + dataByteCount)
                    {
                        Tool.Log($"Invalid data for {requestName} at address {address}: insufficient length.");
                        return;
                    }

                    // Giải mã giá trị float
                    byte[] bytes = new byte[] { data[4], data[3], data[6], data[5] };


                    double actualValue = BitConverter.ToSingle(bytes, 0);
                    if (actualValue >= 999)
                    {
                        return; // Bỏ qua nếu dữ liệu không hợp lệ
                    }



                    actualValue = Math.Round(actualValue, 2);

                    lock (lockObject)
                    {
                        if (!receivedDataByAddress.ContainsKey(address))
                            receivedDataByAddress[address] = new Dictionary<string, double>();

                        receivedDataByAddress[address][requestName] = actualValue;

                        if (receivedDataByAddress[address].Count == _appSettings.Requests.Count)
                        {

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await SaveAllData(address);

                                    lock (lockObject)
                                    {
                                        receivedDataByAddress[address].Clear();
                                        processedRequests.RemoveWhere(k => k.StartsWith($"{address}_"));
                                    }

                                    Tool.Log($"Lưu thành công dữ liệu cho địa chỉ {address}");
                                }
                                catch (Exception ex)
                                {
                                    Tool.Log($"Lỗi khi lưu dữ liệu cho địa chỉ {address}: {ex.Message}");
                                }
                            });
                        }
                    }
                }
                else
                {
                    Tool.Log($"Incomplete data for {requestName} at address {address}.");
                }
            }
            catch (Exception ex)
            {
                Tool.Log($"Lỗi khi phân tích dữ liệu {requestName} tại địa chỉ {address}: {ex.Message}");
                Tool.Log($"Dữ liệu gốc: {BitConverter.ToString(data)}");
            }
        }

        #endregion

        #region Save database
        private async Task SaveAllData(int address)
        {
            try
            {
                Dictionary<string, double> dataForAddress;

                lock (lockObject)
                {
                    if (!receivedDataByAddress.TryGetValue(address, out dataForAddress))
                    {
                        Tool.Log($"Không tìm thấy dữ liệu cho địa chỉ {address}.");
                        return;
                    }
                }

                var device = _appSettings.devices.FirstOrDefault(m => m.address == address);
                if (device == null)
                {
                    Tool.Log($"Không tìm thấy IdMachine với địa chỉ {address}");
                    return;
                }

                int idMachine = device.devid;
                var now = DateTime.Now;

                // Lấy prefix từ idMachine: 1->A, 2->B, 3->C, 4->D
                string prefix = GetPrefixFromIdMachine(idMachine);

                // Danh sách suffix cố định
                List<string> suffixes = new List<string>
                {
                    "01-前烘箱溫度",
                    "02-前烘箱溫度",
                    "03-加硫機溫度",
                    "04-中段烘箱1-上溫度",
                    "05-中段烘箱1-下溫度",
                    "06-中段烘箱2-上溫度",
                    "07-中段烘箱2-下溫度",
                    "08-中段烘箱3-上溫度"
                };

                // Tạo dictionary giá trị cần lưu
                var valuesToSave = suffixes.ToDictionary(
                    suffix => $"{prefix}{suffix}",  // Key để lưu/so sánh với control code
                    suffix => GetValueWithAddressSuffix(dataForAddress, suffix, address) // Tra key không có prefix
                );


                // Lấy danh sách control code theo devid
                using (var newContext = new MyDbContext())
                {
                    var controlCodes = await newContext.controlcodes
                        .Where(c => c.devid == idMachine)
                        .ToListAsync();

                    int savedCount = 0;

                    foreach (var item in valuesToSave)
                    {
                        if (!item.Value.HasValue) continue;

                        var code = controlCodes.FirstOrDefault(c => c.name == item.Key);
                        if (code != null)
                        {
                            var sensorData = new SensorData
                            {
                                devid = idMachine,
                                codeid = code.codeid,
                                value = item.Value.Value,
                                day = now
                            };

                            bool isSaved = await _toolService.InsertToSensorDataAsync(sensorData);
                            if (isSaved)
                            {
                                savedCount++;
                            }
                        }
                    }

                    if (savedCount == 0)
                    {
                        Tool.Log($"⚠ Không có bản ghi nào được lưu vào bảng SensorData cho địa chỉ {address}.");
                    }
                    else
                    {
                        Tool.Log($"→ Đã lưu {savedCount} bản ghi vào bảng SensorData cho địa chỉ {address}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Tool.Log($"Lỗi khi lưu dữ liệu cho địa chỉ {address}: {ex.Message}");
            }
        }

        private double? GetValueWithAddressSuffix(Dictionary<string, double> data, string suffixOnly, int address)
        {
            string fullKey = $"{suffixOnly}_Address_{address}";
            return data.TryGetValue(fullKey, out var value) ? value : null;
        }

        #endregion
        private string GetPrefixFromIdMachine(int idMachine)
        {
            // Chuyển 1 => A, 2 => B, 3 => C, 4 => D
            return idMachine switch
            {
                1 => "A",
                2 => "B",
                3 => "C",
                4 => "D",
                _ => throw new ArgumentException("Invalid idMachine")
            };
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
