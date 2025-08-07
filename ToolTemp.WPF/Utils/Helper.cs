using System.IO.Ports;
using System.Text;

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

        public static byte[] ConvertHexStringToByteArray(string hex)
        {
            // Loại bỏ khoảng trắng và đảm bảo chuỗi được chuyển sang chữ hoa
            hex = hex.Replace(" ", "").ToUpper();

            // Kiểm tra nếu chuỗi có độ dài lẻ
            if (hex.Length % 2 != 0)
            {
                throw new FormatException("Hex string must have an even number of characters.");
            }

            // Khởi tạo mảng byte với độ dài bằng nửa độ dài chuỗi hex
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                // Chuyển mỗi cặp ký tự hex thành một byte
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
        public static string CalculateCRC(string request)
        {
            try
            {
                // Chuyển chuỗi request thành mảng byte
                byte[] requestBytes = ConvertHexStringToByteArray(request);

                // Tính toán CRC
                ushort crc = 0xFFFF; // Giá trị CRC ban đầu

                foreach (byte byteData in requestBytes)
                {
                    crc ^= byteData;
                    for (int i = 0; i < 8; i++)
                    {
                        if ((crc & 0x0001) != 0)
                        {
                            crc >>= 1;
                            crc ^= 0xA001;
                        }
                        else
                        {
                            crc >>= 1;
                        }
                    }
                }

                // Chuyển CRC thành chuỗi theo cặp byte (ví dụ: "47 41")
                string crcResult = crc.ToString("X4").ToUpper();  // Lấy kết quả CRC theo định dạng hex 4 ký tự
                string formattedCRC = string.Join(" ", new[] { crcResult.Substring(2, 2), crcResult.Substring(0, 2) });

                return formattedCRC; // Trả về CRC theo đúng định dạng cặp byte
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating CRC: " + ex.Message);
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
