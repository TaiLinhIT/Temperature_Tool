using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Constants;
using ToolTemp.WPF.Core;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Services;
using ToolTemp.WPF.Utils;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using Style = ToolTemp.WPF.Models.Style;



namespace ToolTemp.WPF.MVVM.ViewModel
{
    public class SettingViewModel : BaseViewModel, INotifyPropertyChanged , IDataErrorInfo
    {
        private SerialPort _serialPort;
        private string _selectedPort;
        private readonly ToolService _toolService;
        public event Action OnMachineLoadDefault;
        private readonly MyDbContext _context;
        private MySerialPortService _myserialPort;

        public ObservableCollection<int> LstBaudrate { get; set; }
        public ObservableCollection<string> LstChooseAssembling { get; set; }

        #region Khai báo và lấy ra danh sách các post

        private List<string> _lstPost;
        public List<string> ListPost
        {
            get => _lstPost;
            set { 
         
                this._lstPost = value;
                OnPropertyChanged(nameof(_lstPost));
            }
        }

        public string port;
        public string Port
        {
            get => port;
            set
            {
                this.port = value;
                OnPropertyChanged(nameof(_lstPost));
            }
        }
        public void GetPorts()
        {
            string[] ArryPort = SerialPort.GetPortNames();
            ListPost = ArryPort.ToList<string>();
        }
        #endregion

        #region Lấy ra danh sách các tốc độ truyền

        private List<int> _lstBaute;
        public List<int> lstBaute
        {
            get => _lstBaute;
            set
            {
                this._lstBaute = value;
                OnPropertyChanged(nameof(lstBaute));
            }
        }
        public int baudrate;
        public int Baudrate
        {
            get => baudrate;
            set
            {
                this.baudrate = value;
                OnPropertyChanged(nameof(Baudrate));
            }
        }
        #endregion

        #region GetPortName
        public ObservableCollection<string> LstPort { get; set; } = new ObservableCollection<string>();


        public void GetPortName()
        {
            string[] lstPort = SerialPort.GetPortNames();
            foreach (var item in lstPort)
            {
                LstPort.Add(item);

            }
        }
        #endregion





        #region Entity


        private ToolTemp.WPF.Models.Machine _selectedMachine;
        public ToolTemp.WPF.Models.Machine SelectedMachine
        {
            get => _selectedMachine;
            set
            {
                if (_selectedMachine != value)
                {
                    _selectedMachine = value;
                    OnPropertyChanged(nameof(SelectedMachine));
                }
            }
        }
        private List<KeyValue> _lstAssebling;
        public List<KeyValue> LstAssemblings
        {
            get { return _lstAssebling; }
            set
            {
                _lstAssebling = value;
                OnPropertyChanged(nameof(LstAssemblings));
            }
        }
        private KeyValue _selectedAssembling;
        public KeyValue SelectedAssembling
        {
            get { return _selectedAssembling; }
            set
            {
                _selectedAssembling = value;
                OnPropertyChanged(nameof(SelectedAssembling));
            }
        }

        private DeviceConfig _deviceConfig;

        public DeviceConfig DeviceConfig
        {
            get { return _deviceConfig; }
            set
            {
                _deviceConfig = value;
                OnPropertyChanged(nameof(DeviceConfig));
            }
        }
        // Thuộc tính lưu trữ Baudrate được chọn
        private int _selectedBaudrate;
        public int SelectedBaudrate
        {
            get => _selectedBaudrate;
            set
            {
                if (_selectedBaudrate != value)
                {
                    _selectedBaudrate = value;
                    OnPropertyChanged(nameof(SelectedBaudrate));
                }
            }
        }





        //Thuoc tinh luu tru port
        private string _selectPort;

        public string SelectedPort
        {
            get => _selectPort;
            set
            {
                if (_selectPort != value)
                {
                    _selectPort = value;
                    OnPropertyChanged(nameof(SelectedPort));
                }
            }
        }


        //Thuoc tinh luu tru LstAssembling
        private string _selectedChooseAssembling;

        public string SelectedChooseAssembling
        {
            get => _selectedChooseAssembling;
            set
            {
                if (_selectedChooseAssembling != value)
                {

                    _selectedChooseAssembling = value;
                    OnPropertyChanged(nameof(SelectedChooseAssembling));
                }
            }
        }


        // Thuộc tính lưu trữ NameMachine (TextBox)
        private string _nameMachine;
        public string NameMachine
        {
            get => _nameMachine;
            set
            {
                if (_nameMachine != value)
                {
                    _nameMachine = value;
                    OnPropertyChanged(nameof(NameMachine));
                }
            }
        }



        //Thuoc tinh luu tru Address
        private string _addressMachine = string.Empty;
        public string AddressMachine
        {
            get => _addressMachine;
            set
            {
                if (_addressMachine != value)
                {
                    if (int.TryParse(value, out int parsedValue) && parsedValue >= 1 && parsedValue <= 50)
                    {
                        _addressMachine = value;
                        ErrorMessage = string.Empty; // Xóa lỗi nếu giá trị hợp lệ
                    }
                    else if (string.IsNullOrWhiteSpace(value))
                    {
                        _addressMachine = value; // Cho phép chuỗi rỗng
                        ErrorMessage = "Vui lòng nhập số từ 1 đến 50.";
                    }
                    else
                    {
                        ErrorMessage = "AddressMachine chỉ cho phép nhập số từ 1 đến 50.";
                    }
                    OnPropertyChanged(nameof(AddressMachine));
                }
            }
        }


        public string _nameStyle;
        public string NameStyle
        {
            get => _nameStyle;
            set
            {
                this._nameStyle = value;
                OnPropertyChanged(nameof(NameStyle));
                IsEnabledBtnAddStyle = true;
            }
        }

        
        public int _demin;
        public int DeMin
        {
            get => _demin;
            set
            {
                this._demin = value;
                OnPropertyChanged(nameof(DeMin));
            }
        }

        public int _demax;
        public int DeMax
        {
            get => _demax;
            set
            {
                this._demax = value;
                OnPropertyChanged(nameof(DeMax));
            }
        }

        public int _giaymin;
        public int GiayMin
        {
            get => _giaymin;
            set
            {
                this._giaymin = value;
                OnPropertyChanged(nameof(GiayMin));
            }
        }
        public int _giaymax;
        public int GiayMax
        {
            get => _giaymax;
            set
            {
                this._giaymax = value;
                OnPropertyChanged(nameof(GiayMax));
            }
        }
        public List<int> _lstAddress;
        public List<int> ListAddress
        {
            get => _lstAddress;
            set
            {
                this._lstAddress = value;
                OnPropertyChanged(nameof(ListAddress));
            }
        }
        public int address;
        public int Address
        {
            get => address;
            set
            {
                this.address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = null;
                switch (columnName)
                {
                    case nameof(Port):
                        if (string.IsNullOrEmpty(Port))
                            error = "Port is required.";
                        else if (Port == "COM1")
                            error = "Port is default! Choose anthore port.";
                        break;
                    case nameof(Baudrate):
                        if (string.IsNullOrEmpty(Baudrate.ToString()))
                            error = "Baudrate is required.";
                        else if (Baudrate != 115200)
                            error = "Baudrate is not correct.";
                        break;
                    
                }
                return error;
            }
        }

        public string Error => null;
        #endregion




        public RelayCommand SaveConfigCommand { get; }
        string selectedCompany;
        #region Command
        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand AddStyleCommand { get; set; }
        public ICommand DeleteStyleCommand { get; set; }

        public ICommand AddMachineCommand { get; set; }
        public ICommand EditMachineCommand { get; set; }
        public ICommand DeleteMachineCommand { get; set; }

        #endregion
        private MySerialPortService _mySerialPort;
        public ToolViewModel _toolViewModel;
        public AppSettings _appSetting;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<Button,Button> NewButtonCreated;

        private string _textBoxContent;
        public string TextBoxContent
        {
            get => _textBoxContent;
            set
            {
                _textBoxContent = value;
                OnPropertyChanged(nameof(TextBoxContent));
            }
        }
        public ICommand CreatedButtonCommand { get; }

        #region Ẩn hiển settings
        public event Action OnConnect;



        #endregion

        #region Language
        private string _portMachineCommandText;
        public string PortMachineCommandText
        {
            get => _portMachineCommandText;
            set
            {
                _portMachineCommandText = value;
                OnPropertyChanged(nameof(PortMachineCommandText));
            }
        }
        private string _nameMachineCommandText;
        public string NameMachineCommandText
        {
            get => _nameMachineCommandText;
            set
            {
                _nameMachineCommandText = value;
                OnPropertyChanged(nameof(NameMachineCommandText));
            }
        }
        private string _baudrateMachineCommandText;
        public string BaudrateMachineCommandText
        {
            get => _baudrateMachineCommandText;
            set
            {
                _baudrateMachineCommandText = value;
                OnPropertyChanged(nameof(BaudrateMachineCommandText));
            }
        }

        private string _addMachineCommandText;
        public string AddMachineCommandText
        {
            get => _addMachineCommandText;
            set
            {
                _addMachineCommandText = value;
                OnPropertyChanged(nameof(AddMachineCommandText));
            }
        }
        private string _editMachineCommandText;
        public string EditMachineCommandText
        {
            get => _editMachineCommandText;
            set
            {
                _editMachineCommandText = value;
                OnPropertyChanged(nameof(EditMachineCommandText));
            }
        }
        private string _deleteMachineCommandText;
        public string DeleteMachineCommandText
        {
            get => _deleteMachineCommandText;
            set
            {
                _deleteMachineCommandText = value;
                OnPropertyChanged(nameof(DeleteMachineCommandText));
            }
        }

        private string _addressMachineCommandText;
        public string AddressMachineCommandText
        {
            get => _addressMachineCommandText;
            set
            {
                _addressMachineCommandText = value;
                OnPropertyChanged(nameof(AddressMachineCommandText));
            }
        }
        private string _connectCommandText;
        public string ConnectCommandText
        {
            get => _connectCommandText;
            set
            {
                _connectCommandText = value;
                OnPropertyChanged(nameof(ConnectCommandText));
            }
        }
        private string _deleteStyleCommandText;
        public string DeleteStyleCommandText
        {
            get => _deleteStyleCommandText;
            set
            {
                _deleteStyleCommandText = value;
                OnPropertyChanged(nameof(DeleteStyleCommandText));
            }
        }
        private string _chooseStyleCommandText;
        public string ChooseStyleCommandText
        {
            get => _chooseStyleCommandText;
            set
            {
                _chooseStyleCommandText = value;
                OnPropertyChanged(nameof(ChooseStyleCommandText));
            }
        }
        private string _addStyleCommandText;
        public string AddStyleCommandText
        {
            get => _addStyleCommandText;
            set
            {
                _addStyleCommandText = value;
                OnPropertyChanged(nameof(AddStyleCommandText));
            }
        }
        private string _nameCommandText;
        public string NameCommandText
        {
            get => _nameCommandText;
            set
            {
                _nameCommandText = value;
                OnPropertyChanged(nameof(NameCommandText));
            }
        }

        private string _maxCommandText;
        public string MaxCommandText
        {
            get => _maxCommandText;
            set
            {
                _maxCommandText = value;
                OnPropertyChanged(nameof(MaxCommandText));
            }
        }

        private string _minCommandText;
        public string MinCommandText
        {
            get => _minCommandText;
            set
            {
                _minCommandText = value;
                OnPropertyChanged(nameof(MinCommandText));
            }
        }

        private string _soleCommandText;
        public string SoleCommandText
        {
            get => _soleCommandText;
            set
            {
                _soleCommandText = value;
                OnPropertyChanged(nameof(SoleCommandText));
            }
        }

        private string _shoesCommandText;
        public string ShoesCommandText
        {
            get => _shoesCommandText;
            set
            {
                _shoesCommandText = value;
                OnPropertyChanged(nameof(ShoesCommandText));
            }
        }
        #endregion

        #region Constructor
        public SettingViewModel(AppSettings appSettings, ToolViewModel toolViewModel, ToolService toolService, MyDbContext myDbContext)
        {
            _toolService = toolService;
            _context = myDbContext;
            _appSetting = appSettings;
            message.Port = _appSetting.Port;
            message.Baudrate = _appSetting.Baudrate;
            _toolViewModel = toolViewModel;

            _toolViewModel.Port = _appSetting.Port;
            _toolViewModel.Baudrate = _appSetting.Baudrate;

            _myserialPort = new MySerialPortService();

            IsEnabledBtnConnect = true;
            IsEnabledBtnAddStyle = true;
            IsEnabledBtnDelete = false;

            IsEnabledBtnConnect = true;
            IsEnabledBtnAddMachine = true;
            IsEnableBtnEditMachine = false;

            // list baudrate

            LstBaudrate = new ObservableCollection<int>()
            {
                2400, 4800, 9600, 19200, 115200
            };

            // list choose Assembling

            LstChooseAssembling = new ObservableCollection<string>()
            {
                "Nong","Lanh"
            };

            // Lấy danh sách lstAssembling từ cơ sở dữ liệu
            List<string> lstAssembling = _context.dvFactoryAssemblings
                .Where(x => x.Factory == _appSetting.CurrentArea)
                .Select(x => x.Assembling)
                .ToList();

            // Khởi tạo danh sách lstAssemblings
            LstAssemblings = new List<KeyValue>();

            // Thêm từng mục vào danh sách lstAssemblings
            foreach (var item in lstAssembling)
            {
                LstAssemblings.Add(new KeyValue
                {
                    key = item,
                    value = "Thành Hình " + item
                });
            }

            

            //Buttons
            ConnectCommand = new RelayCommand(ExecuteConnectCommand, CanConnect);
            DisconnectCommand = new RelayCommand(ExecuteDisConnectCommand, CanDisconnect);
            AddStyleCommand = new RelayCommand(AddButton, CanAddStyle);
            DeleteStyleCommand = new RelayCommand(ExecuteDeleteCommand, CanDeleteCommand);


            AddMachineCommand = new RelayCommand(ExecuteAddMachineCommand, CanAddMachine);
            EditMachineCommand = new RelayCommand(ExecuteEditMachineCommand, CanEditMachine);
            DeleteMachineCommand = new RelayCommand(ExecuteDeleteMachineCommand, CanDeleteMachine);


            ButtonList = new ObservableCollection<Button>();

            
           
            

            //OpenViewMainCommand = new RelayCommand(OpenViewMain);
            GetPorts();

            Messenger.Default.Register<DeviceConfig>(this, "DeviceConfigToChild", HandleDeviceConfigToChildMessage);

            LoadButtonsAsync();

            

            GetPortName();

            IsEnabledBtnConnect = false;
            //MessageBox.Show("Connection successful!");
            _toolViewModel.DeMax = 75;
            _toolViewModel.DeMin = 70;
            _toolViewModel.GiayMax = 65;
            _toolViewModel.GiayMin = 60;
            _toolViewModel.idStyle = 2;
            _toolViewModel.NameStyle = "Decker";
            _toolViewModel.Start();
            for (int i = 1; i <= 6; i++)
            {
                _toolViewModel.SetFactory("VB2", i);
                _toolViewModel.GetTempFromMachine(i, i); // Truyền thêm IdMachine

            }

        }

        #endregion

        private bool canExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private void Createbutton(object obj)
        {
            // Kiểm tra nội dung hợp lệ
            if (!string.IsNullOrWhiteSpace(TextBoxContent))
            {
                // Tạo Button mới
                var newButton = new Button
                {
                    Content = TextBoxContent,
                    Width = 90,
                    Height = 30,
                    Margin = new Thickness(5),
                    Background = new SolidColorBrush(Colors.LightGreen)
                };

                
            }
        }


        private void HandleDeviceConfigToChildMessage(DeviceConfig message)
        {
            Port = message.Port;
            Baudrate = message.Baudrate;
            DeMax = Convert.ToInt32(message.DeMax) ;
            DeMin = Convert.ToInt32(message.DeMin);
            NameStyle = message.NameStyle;
        }

        #region Style

        private string _codetext;
        public string CodeText
        {
            get => _codetext;
            set
            {
                _codetext = value;
                OnPropertyChanged(nameof(CodeText));
                UpdateStyleButtonTexts();
            }
        }
        private void UpdateStyleButtonTexts()
        {
            foreach (var button in ButtonList)
            {
                if (button.DataContext is Style model)
                {
                    if (button.Content is TextBlock textBlock)
                    {
                        textBlock.Text = CodeText + " " + model.NameStyle;  // Cập nhật nội dung button
                    }
                }
            }
        }

        // Danh sách chứa các button đã tạo
        public ObservableCollection<Button> _buttonList = new ObservableCollection<Button>();
        public ObservableCollection<Button> ButtonList
        {
            get { return _buttonList; }
            set
            {
                _buttonList = value;
                OnPropertyChanged(nameof(ButtonList));

            }
        }

        

        // Phương thức để thêm button mới vào danh sách
        private async void AddButton(object parameter)
        {
            Style style = new Style();
            style.NameStyle = NameStyle;
            style.DeMax = DeMax;
            style.DeMin = DeMin;
            style.GiayMax = GiayMax;
            style.GiayMin = GiayMin;
            // Save data asynchronously
            await Task.Run(async () =>
            {
                try
                {
                    await _toolService.InsertToStyle(style);
                }
                catch (Exception insertEx)
                {
                    Tool.Log($"Error saving data: {insertEx.Message}");
                    if (insertEx.StackTrace != null)
                        Tool.Log(insertEx.StackTrace.ToString());
                }
            });


            Button btn_Style = new Button
            {
                Content = new TextBlock
                {
                    Text = "Mã " + _toolService.GetListStyle().OrderByDescending(x => x.Id).Select(x => x.NameStyle)
                             .FirstOrDefault().ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 90,
                    Height = 30,

                },
                Margin = new Thickness(5),
                DataContext = style,


                Background = new SolidColorBrush(Colors.LightGreen)
            };

            btn_Style.Click += btn_Style_Click;
            // Thêm vào ButtonList
            ButtonList.Add(btn_Style);



            // Reset các trường nhập liệu
            NameStyle = string.Empty;

        }
        public DeviceConfig message = new DeviceConfig();

        private void btn_Style_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cast sender to Button
                Button clickedButton = (Button)sender;

                

                // Retrieve the Style object from DataContext
                if (clickedButton.DataContext is Style style)
                {
                    // Use the Style object to populate DeviceConfig
                    message.NameStyle = style.NameStyle;
                    message.DeMax = style.DeMax;
                    message.DeMin = style.DeMin;
                    message.GiayMax = style.GiayMax;
                    message.GiayMin = style.GiayMin;
                    
                    Messenger.Default.Send(message, "DeviceConfigMessage");
                    _toolViewModel.DeMax = Convert.ToInt32(style.DeMax);
                    _toolViewModel.DeMin = Convert.ToInt32(style.DeMin);
                    _toolViewModel.GiayMax = Convert.ToInt32(style.GiayMax);
                    _toolViewModel.GiayMin = Convert.ToInt32(style.GiayMin);
                    _toolViewModel.idStyle = Convert.ToInt32(style.Id);
                    _toolViewModel.NameStyle = style.NameStyle;

                    NameStyle = style.NameStyle;
                    DeMax = Convert.ToInt32(style.DeMax);
                    DeMin = Convert.ToInt32(style.DeMin);
                    GiayMax = Convert.ToInt32(style.GiayMax);
                    GiayMin = Convert.ToInt32(style.GiayMin);
                    IsEnabledBtnAddStyle = false;
                    IsEnabledBtnDelete = true;
                    
                    //_toolViewModel.Start();
                }
                else
                {
                    // Handle the case where DataContext is not a Style object
                    MessageBox.Show("The DataContext of the button is not of type Style.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error! " + ex.Message);
            }
        }



        private async Task LoadButtonsAsync()
        {
            try
            {
                // Retrieve the styles from the database
                var styles = _toolService.GetAllStyles();

                // Clear existing buttons if necessary
                ButtonList.Clear();

                foreach (var style in styles)
                {
                    Button btn_Style = new Button
                    {
                        Content = new TextBlock
                        {
                            Text = "Mã " + style.NameStyle,
                            VerticalAlignment = VerticalAlignment.Center,
                            Width = 90,
                            Height = 30
                        },
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(Colors.LightGreen),
                        DataContext = style // Set DataContext to the Style object
                    };

                    btn_Style.Click += btn_Style_Click;
                    ButtonList.Add(btn_Style); // Add the button to the list
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading buttons: " + ex.Message);
            }
        }



        // Điều kiện để cho phép thêm button (ComboBox không rỗng)
        private bool CanAddStyle(object parameter)
        {
            try
            {
                return !string.IsNullOrEmpty(NameStyle) && !string.IsNullOrWhiteSpace(DeMax.ToString()) && !string.IsNullOrWhiteSpace(DeMin.ToString()) && IsEnabledBtnAddStyle;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            // Chỉ cho phép thêm khi các trường Name, Max, Min không rỗng
        }



        public async void ExecuteConnectCommand(object parameter)
        {
            

            try
            {
                //message.Port = this.Port;
                //message.Baudrate = this.Baudrate;

                

                Messenger.Default.Send(message, "DeviceConfigMessage");
                // set port for _toolViewModel
                
                
                _toolViewModel.Start();
                IsEnabledBtnConnect = false;
                //MessageBox.Show("Connection successful!");
                


            }
            catch (Exception ex)
            {
                IsEnabledBtnConnect = true;
                MessageBox.Show("Connection erro!" + ex.Message);
            }
        }
        private bool CanConnect(object parameter)
        {
            return IsEnabledBtnConnect;
        }


        private async void ExecuteDisConnectCommand(object parameter)
        {
            try
            {
                
                _toolViewModel.Close();
                
                IsEnabledBtnConnect = true;
                MessageBox.Show("Disconnected !");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting!" + ex.Message);
                IsEnabledBtnConnect = false;
            }
        }

        private bool CanDisconnect(object parameter)
        {
            return !IsEnabledBtnConnect;
        }


        private void ExecuteDeleteCommand (object parameter)
        {
            try
            {
                _toolService.DeleteStyleByName(NameStyle);
                LoadButtonsAsync();
                NameStyle = string.Empty;
                DeMax = 0;
                DeMin = 0;
                MessageBox.Show("Delete succesesfully!");
            }
            catch (Exception ex)
            {

                Tool.Log($"Errors delete: {ex.Message}");
                
            }
        }
        private bool CanDeleteCommand(object parameter)
        {
            return IsEnabledBtnDelete;
        }
        private bool isEnabledBtnConnect;



        public async void ExecuteAddMachineCommand(object parameter)
        {

            try
            {

                if (_context.machines.Any(x => x.Name == NameMachine))
                {
                    MessageBox.Show("Machine is allready!");
                    return;
                }
                ToolTemp.WPF.Models.Machine machines = new ToolTemp.WPF.Models.Machine();
                machines.Name = NameMachine;
                machines.Port = SelectedPort;
                machines.Baudrate = SelectedBaudrate;
                machines.Address = int.Parse(AddressMachine);
                machines.Line = SelectedAssembling.key;
                machines.LineCode = SelectedChooseAssembling == "Nong" ? "H" : "C";

                await _toolService.InsertToMachine(machines);
                IsEnabledBtnAddMachine = false;

                // Tạo nút Machine
                Button btn_Machine = new Button
                {
                    Content = NameMachine,
                    Background = SelectedChooseAssembling == "Nong" ? Brushes.White : Brushes.Blue
                };

                // Tạo nút Assembling
                Button btn_Assembling = new Button
                {
                    Content = SelectedAssembling.value
                };

                // Gửi Button qua sự kiện
                NewButtonCreated?.Invoke(btn_Machine, btn_Assembling);
            }
            catch (Exception ex)
            {

                IsEnabledBtnAddMachine = true;
                MessageBox.Show("Add machine errors: " + ex.Message);
            }

        }
        private bool CanAddMachine(object parameter)
        {
            try
            {
                return !string.IsNullOrEmpty(AddressMachine.ToString()) &&
               !string.IsNullOrEmpty(NameMachine) &&
               !string.IsNullOrEmpty(SelectedPort) &&
               !string.IsNullOrEmpty(SelectedBaudrate.ToString()) &&
               !string.IsNullOrEmpty(SelectedAssembling?.value) &&
               !string.IsNullOrEmpty(SelectedChooseAssembling);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        //Edit
        public async void ExecuteEditMachineCommand(object parameter)
        {
            try
            {
                if (!IsEnableBtnEditMachine)
                {
                    MessageBox.Show("Button is disabled. Cannot edit machine.");
                    return;
                }

                if (SelectedMachine == null)
                {
                    MessageBox.Show("No machine selected.");
                    return;
                }

                //Tìm máy trong cơ sở dữ liệu
               var find = await _context.machines.FirstOrDefaultAsync(x => x.Id == SelectedMachine.Id);

                if (find == null)
                {
                    MessageBox.Show("Machine not found.");
                    return;
                }

                //Cập nhật thuộc tính của máy
                find.Address = int.Parse(AddressMachine);
                find.Port = SelectedPort;
                find.Baudrate = SelectedBaudrate;
                find.Name = NameMachine;
                find.Line = SelectedAssembling.key;
                find.LineCode = SelectedChooseAssembling == "Nong" ? "H" : "C";

                //Lưu thay đổi vào cơ sở dữ liệu
                await _toolService.EditToMachine(find);
                OnMachineLoadDefault?.Invoke();
                MessageBox.Show("Edit successfully!");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private bool CanEditMachine(object parameter)
        {
            try
            {
                return !string.IsNullOrEmpty(AddressMachine.ToString()) &&
               !string.IsNullOrEmpty(NameMachine) &&
               !string.IsNullOrEmpty(SelectedPort) &&
               !string.IsNullOrEmpty(SelectedBaudrate.ToString()) &&
               !string.IsNullOrEmpty(SelectedAssembling?.value) &&
               !string.IsNullOrEmpty(SelectedChooseAssembling);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        //Delete
        public async void ExecuteDeleteMachineCommand(object parameter)
        {
            try
            {
                if (!IsEnableBtnEditMachine)
                {
                    MessageBox.Show("Button is disabled. Cannot edit machine.");
                    return;
                }

                if (SelectedMachine == null)
                {
                    MessageBox.Show("No machine selected.");
                    return;
                }

                //Tìm máy trong cơ sở dữ liệu
               var find = await _context.machines.FirstOrDefaultAsync(x => x.Id == SelectedMachine.Id);

                if (find == null)
                {
                    MessageBox.Show("Machine not found.");
                    return;
                }



                //Lưu thay đổi vào cơ sở dữ liệu
                await _toolService.DeleteToMachine(find);
                //Giả sử đã xóa thành công
                OnMachineLoadDefault?.Invoke();
                MessageBox.Show("Delete successfully!");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private bool CanDeleteMachine(object parameter)
        {
            try
            {
                return !string.IsNullOrEmpty(AddressMachine.ToString()) &&
                int.Parse(AddressMachine) >= 1 &&
                int.Parse(AddressMachine) <= 50 &&
               !string.IsNullOrEmpty(NameMachine) &&
               !string.IsNullOrEmpty(SelectedPort) &&
               !string.IsNullOrEmpty(SelectedBaudrate.ToString()) &&
               !string.IsNullOrEmpty(SelectedAssembling?.value) &&
               !string.IsNullOrEmpty(SelectedChooseAssembling);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }
        public bool IsEnabledBtnConnect
        {
            get { return isEnabledBtnConnect; }
            set
            {
                isEnabledBtnConnect = value;
                OnPropertyChanged(nameof(IsEnabledBtnConnect));
            }
        }

        private bool isEnabledBtnAddMachine;

        public bool IsEnabledBtnAddMachine
        {
            get { return isEnabledBtnAddMachine; }
            set
            {
                isEnabledBtnAddMachine = value;
                OnPropertyChanged(nameof(IsEnabledBtnAddMachine));
            }
        }
        private bool isEnabledBtnEditMachine;
        public bool IsEnableBtnEditMachine
        {
            get { return isEnabledBtnEditMachine; }
            set
            {
                isEnabledBtnEditMachine = value;
                OnPropertyChanged(nameof(IsEnableBtnEditMachine));
            }
        }
        private bool isEnabledBtnDeleteMachine;
        public bool IsEnabledBtnDeleteMachine
        {
            get { return isEnabledBtnDeleteMachine; }
            set
            {
                isEnabledBtnDeleteMachine = value;
                OnPropertyChanged(nameof(IsEnabledBtnDeleteMachine));
            }
        }

        private bool isEnabledBtnAddStyle;
        public bool IsEnabledBtnAddStyle
        {
            get { return isEnabledBtnAddStyle; }
            set
            {
                isEnabledBtnAddStyle = value;
                OnPropertyChanged(nameof(IsEnabledBtnAddStyle));
            }
        }

        private bool isEnabledBtnDelete;

        public bool IsEnabledBtnDelete
        {
            get { return isEnabledBtnDelete; }
            set
            {
                isEnabledBtnDelete = value;
                OnPropertyChanged(nameof(IsEnabledBtnDelete));
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChangeds;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
