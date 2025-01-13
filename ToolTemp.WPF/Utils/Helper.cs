using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolTemp.WPF.Models;

namespace ToolTemp.WPF.Utils
{
    public static class Helper
    {
        /// <summary>
        /// Send command to machine
        /// </summary>
        /// <param name="_serialPort"></param>
        /// <param name="str"></param>
        public static void SerialPortWrite(SerialPort _serialPort, string str)
        {
            byte[] data = new byte[1]; 
            string strs = str;
            
            if (strs.Length % 2 == 1)
            {
                strs = strs.Insert(strs.Length - 1, "0");
            }
            
            for (int i = 0; i < strs.Length / 2; i++)
            {
               
                data[0] = Convert.ToByte(strs.Substring(i * 2, 2), 16);
              
                _serialPort.Write(data, 0, 1);
            }
            //_serialPort.WriteLine(str);
        }

        /// <summary>
        /// Generate CRC for Command
        /// </summary>
        /// <param name="decimalString"></param>
        /// <returns></returns>
        public static string CalculateCRC(string decimalString)
        {
            var result = string.Empty;
            try
            {
                var hexString = DecimalStringToHexString(decimalString);
                byte[] data = HexStringToByteArray(hexString);
                ushort crc = 0xFFFF;

                for (int pos = 0; pos < data.Length; pos++)
                {
                    crc ^= (ushort)data[pos];          // XOR byte vào CRC

                    for (int i = 8; i != 0; i--)       // Duyệt qua 8 bits của byte
                    {
                        if ((crc & 0x0001) != 0)       // Nếu bit thấp nhất là 1
                        {
                            crc >>= 1;                 // Dịch phải 1 bit
                            crc ^= 0xA001;             // XOR với đa thức chuẩn
                        }
                        else                           // Nếu bit thấp nhất là 0
                        {
                            crc >>= 1;                 // Chỉ dịch phải 1 bit
                        }
                    }
                }

                byte[] crcBytes = new byte[2];
                crcBytes[0] = (byte)(crc & 0xFF);      // Lưu byte thấp
                crcBytes[1] = (byte)((crc >> 8) & 0xFF); // Lưu byte cao

                var b1 = BitConverter.ToString(crcBytes).Replace("-", " ");
                result = $"{hexString} {b1}";
                return result;
            }
            catch (Exception ex)
            {
                Tool.Log("ERRO: Can not generate command");
                return result;
            }
        }

        public static string DecimalStringToHexString(string decimalString)
        {
            // Tách các giá trị thập phân bằng dấu cách và chuyển đổi từng giá trị
            string[] values = decimalString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder hexBuilder = new StringBuilder();

            foreach (var value in values)
            {
                // Chuyển giá trị thập phân sang hex và thêm vào chuỗi hexBuilder
                int decimalValue = int.Parse(value);
                string hexValue = decimalValue.ToString("X2"); // X2 để chuyển đổi sang hex với 2 chữ số
                hexBuilder.Append(hexValue).Append(" ");
            }

            // Xóa dấu cách cuối cùng nếu có
            if (hexBuilder.Length > 0)
            {
                hexBuilder.Length--; // Xóa ký tự cuối cùng (dấu cách)
            }

            return hexBuilder.ToString();
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            hex = hex.Replace(" ", "");
            byte[] data = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                data[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return data;
        }

        
    }
}
