using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Forms;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Utils;

namespace ToolTemp.WPF.Services
{

    public class ToolService : IToolService
    {
        private readonly MyDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public ToolService(MyDbContext myDbContext, IServiceScopeFactory serviceScope)
        {
            _context = myDbContext;
            _scopeFactory = serviceScope;
        }

        public async Task<bool> InsertToStyle(Style model)
        {
            try
            {
                await _context.Style.AddAsync(model);
                await _context.SaveChangesAsync();


                return true; // Trả về 1 nếu thêm dữ liệu thành công
            }
            catch (Exception ex)
            {
                Tool.Log(ex.Message);
                return false;
            }
        }
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<bool> InsertToSensorDataAsync(SensorData data)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                await _semaphore.WaitAsync();

                try
                {
                    await dbContext.sensorDatas.AddAsync(data);
                    var result = await dbContext.SaveChangesAsync();

                    Tool.Log($"→ SaveChangesAsync: {result} record(s) affected for codeid {data.codeid}");
                    return result > 0;
                }
                catch (Exception ex)
                {
                    Tool.Log($"Lỗi khi lưu SensorData (codeid {data.codeid}): {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Tool.Log($"→ Chi tiết lỗi bên trong: {ex.InnerException.Message}");
                    }
                    return false;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public async Task<int> InsertData(BusDataTemp model)
        {
            try
            {
                await _context.BusDataTemp.AddAsync(model);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                Tool.Log(ex.Message);
                return 0;
            }
        }




        public async Task<List<BusDataWithDevice>> GetListDataWithDevicesAsync(string port, string factory, int address, int baudRate, string language)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                // Ensure the connection is open
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetListDataWithDevices"; // Stored procedure name
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddRange(new[]
                    {
                new SqlParameter("@Language", System.Data.DbType.String) { Value = language },
                new SqlParameter("@Port", System.Data.DbType.String) { Value = port },
                new SqlParameter("@Factory", System.Data.DbType.String) { Value = factory },
                new SqlParameter("@Address", System.Data.DbType.Int32) { Value = address },
                new SqlParameter("@Baudrate", System.Data.DbType.Int32) { Value = baudRate }
            });

                    var result = new List<BusDataWithDevice>();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var busDataTempWithDevice = new BusDataWithDevice
                            {
                                Id = !reader.IsDBNull(reader.GetOrdinal("Id")) ? reader.GetInt32(reader.GetOrdinal("Id")) : 0,
                                IdMachine = !reader.IsDBNull(reader.GetOrdinal("IdMachine")) ? reader.GetInt32(reader.GetOrdinal("IdMachine")) : 0,
                                IdStyle = !reader.IsDBNull(reader.GetOrdinal("IdStyle")) ? reader.GetInt32(reader.GetOrdinal("IdStyle")) : 0,
                                Channel = !reader.IsDBNull(reader.GetOrdinal("Channel")) ? reader.GetString(reader.GetOrdinal("Channel")) : null,
                                Factory = !reader.IsDBNull(reader.GetOrdinal("Factory")) ? reader.GetString(reader.GetOrdinal("Factory")) : null,
                                Line = !reader.IsDBNull(reader.GetOrdinal("Line")) ? reader.GetString(reader.GetOrdinal("Line")) : null,
                                AddressMachine = !reader.IsDBNull(reader.GetOrdinal("AddressMachine")) ? reader.GetInt32(reader.GetOrdinal("AddressMachine")) : 0,
                                LineCode = !reader.IsDBNull(reader.GetOrdinal("LineCode")) ? reader.GetString(reader.GetOrdinal("LineCode")) : null,
                                Port = !reader.IsDBNull(reader.GetOrdinal("Port")) ? reader.GetString(reader.GetOrdinal("Port")) : null,
                                Temp = !reader.IsDBNull(reader.GetOrdinal("Temp")) ? reader.GetDouble(reader.GetOrdinal("Temp")) : 0,
                                Baudrate = !reader.IsDBNull(reader.GetOrdinal("Baudrate")) ? reader.GetInt32(reader.GetOrdinal("Baudrate")) : 0,
                                Max = !reader.IsDBNull(reader.GetOrdinal("Max")) ? reader.GetInt32(reader.GetOrdinal("Max")) : 0,
                                Min = !reader.IsDBNull(reader.GetOrdinal("Min")) ? reader.GetInt32(reader.GetOrdinal("Min")) : 0,
                                UploadDate = !reader.IsDBNull(reader.GetOrdinal("UploadDate")) ? reader.GetDateTime(reader.GetOrdinal("UploadDate")) : DateTime.MinValue,
                                IsWarning = !reader.IsDBNull(reader.GetOrdinal("IsWarning")) ? reader.GetBoolean(reader.GetOrdinal("IsWarning")) : false,
                                Sensor_Typeid = !reader.IsDBNull(reader.GetOrdinal("Sensor_Typeid")) ? reader.GetInt32(reader.GetOrdinal("Sensor_Typeid")) : 0,
                                Sensor_kind = !reader.IsDBNull(reader.GetOrdinal("Sensor_kind")) ? reader.GetString(reader.GetOrdinal("Sensor_kind")) : null,
                                Sensor_ant = !reader.IsDBNull(reader.GetOrdinal("Sensor_ant")) ? reader.GetString(reader.GetOrdinal("Sensor_ant")) : null,
                                DeviceName = !reader.IsDBNull(reader.GetOrdinal("DeviceName")) ? reader.GetString(reader.GetOrdinal("DeviceName")) : null
                            };


                            result.Add(busDataTempWithDevice);
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Tool.Log($"Error in GetListDataWithDevicesAsync: {ex.Message}");
                return new List<BusDataWithDevice>(); // Return an empty list instead of null
            }
        }


        public List<Style> GetListStyle()
        {
            try
            {
                var result = _context.Style.FromSqlRaw("EXEC GetListStyle").ToList();

                return result;
            }
            catch (Exception ex)
            {
                Tool.Log($"Errors GetListStyle: {ex.Message}");
                return null;
            }
        }
        public bool DeleteStyleByName(string name)
        {
            try
            {
                Style find = _context.Style.Where(x => x.NameStyle == name).FirstOrDefault();
                _context.Style.Remove(find);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                Tool.Log($"Errors Delete style: {ex.Message}");
                return false;
            }
        }

        public List<Factory> GetListAssembling(string factoryCode)
        {
            try
            {
                var factoryCodeParam = new SqlParameter("@FactoryCode", factoryCode);
                var result = _context.FactoryConfigs.FromSqlRaw("EXEC GetListFactory @FactoryCode", factoryCodeParam).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Tool.Log($"Errors GetListFactory: {ex.Message}");
                return null;
            }
        }



        public async Task<string> GetLineByAddressAndFactoryAsync(int address, string factory)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetLineByAddressAndFactory";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm tham số @Address
                    var addressParameter = command.CreateParameter();
                    addressParameter.ParameterName = "@Address";
                    addressParameter.Value = address;
                    addressParameter.DbType = System.Data.DbType.Int32;
                    command.Parameters.Add(addressParameter);

                    // Thêm tham số @Factory
                    var factoryParameter = command.CreateParameter();
                    factoryParameter.ParameterName = "@Factory";
                    factoryParameter.Value = factory;
                    factoryParameter.DbType = System.Data.DbType.String;
                    command.Parameters.Add(factoryParameter);

                    var result = await command.ExecuteScalarAsync();
                    return result as string;
                }
            }
            catch (Exception ex)
            {
                Tool.Log($"Error in GetLineByAddressAndFactoryAsync: {ex.Message}");
                return null;
            }
        }


        public List<Style> GetAllStyles()
        {
            try
            {
                return _context.Style.ToList();

            }
            catch (Exception ex)
            {
                Tool.Log($"Error retrieving data: {ex.Message}");
                if (ex.StackTrace != null)
                    Tool.Log(ex.StackTrace.ToString());
                return new List<Style>();
            }
        }

        public async Task<int> InsertToMachine(Machine machine)
        {
            try
            {
                await _context.machines.AddAsync(machine);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteToMachine(Machine machine)
        {
            try
            {
                _context.machines.Remove(machine);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<int> EditToMachine(Machine machine)
        {
            try
            {
                _context.machines.Update(machine);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }
        public string ConvertToHex(int number)
        {
            return number.ToString("X");
        }
        public byte[] ConvertHexStringToByteArray(string hexString)
        {
            hexString = hexString.Replace(" ", "").ToUpper(); // Chuẩn hóa chuỗi
            if (hexString.Length % 2 != 0 || !System.Text.RegularExpressions.Regex.IsMatch(hexString, "^[0-9A-F]+$"))
            {
                throw new FormatException("Invalid hex string.");
            }

            int numberOfBytes = hexString.Length / 2;
            byte[] bytes = new byte[numberOfBytes];
            for (int i = 0; i < numberOfBytes; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }
}
