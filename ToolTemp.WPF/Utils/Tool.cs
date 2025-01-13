using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ToolTemp.WPF.Utils
{
    public class Tool
    {
        // đây là phương thức tĩnh
        public static int index = 0;
        public static string path()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }
        public static string pathd()
        {
            return path() + @"Resources\";
        }
        public static string Bytetostring(byte[] lg)
        {
            string str = string.Empty;
            foreach (byte b in lg)
            {
                str += b.ToString("X2") + " ";
            }
            return str;
        }
        static readonly object obj = new object();
        public static void Log(string lg)
        {
            lock (obj)
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string logDirectory = Path.Combine(baseDirectory, "Logs");

                // Tạo thư mục Logs nếu chưa tồn tại
                if (!Directory.Exists(logDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi tạo thư mục Logs: {ex.Message}");
                        return; // Thoát khỏi phương thức nếu không thể tạo thư mục
                    }
                }

                string logFileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
                string logFilePath = Path.Combine(logDirectory, logFileName);

                try
                {
                    using (StreamWriter w = File.AppendText(logFilePath))
                    {
                        w.WriteLine($"{DateTime.Now:HH:mm:ss} - {lg}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi ghi log: {ex.Message}");
                }
            }
        }
        //public static void Log(string lg)
        //{
        //    lock (obj)
        //    {
        //        string filepath = pathd() + "log.txt";

        //        if (!File.Exists(filepath))
        //        {
        //            // Nếu không tìm thấy tệp tin, tạo một tệp tin mới
        //            using (StreamWriter createFile = File.CreateText(filepath))
        //            {
        //                createFile.Close();
        //            }
        //        }
        //        using (StreamWriter w = File.AppendText(filepath))
        //        {
        //            w.Write(DateTime.Now);
        //            w.Write(":");
        //            w.WriteLine(lg);
        //        }
        //    }
        //}
        public static string[] gCrc16Table;
        public static void Init()
        {

            Crc16();
        }
        public static Int16 byte_int16(string value)
        {
            value = value.Replace(" ", "");//字符替换
            string a1 = value.Substring(0, 2);
            string a2 = value.Substring(2, 2);
            value = a2 + a1;

            //Console.WriteLine("byte_int16(a1)=" + a1);
            //Console.WriteLine("byte_int16(a2)=" + a2);

            UInt16 x = Convert.ToUInt16(value, 16);

            Int16 fy = BitConverter.ToInt16(BitConverter.GetBytes(x), 0);
            //Console.WriteLine("byte_int16=" + fy);
            return fy;
        }
        public static void Crc16()
        {
            string filepath = pathd() + "crc.json";
            string json = string.Empty;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8")))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            gCrc16Table = JsonSerializer.Deserialize<string[]>(json);
        }

        //////////////
        public static int CALCRC16(int crc, int[] buf, int length)
        {
            int pos = 0;
            while (length != 0)
            {
                length -= 1;
                crc = Crc16Byte(crc, buf[pos]);
                pos += 1;
            }
            return crc;
        }
        public static int Crc16Byte(int crc, int data)
        {
            ushort u16 = Convert.ToUInt16(gCrc16Table[((crc >> 8 ^ data) & 0xff)], 16);
            return (crc << 8 ^ u16);
        }
        public static byte[] cal_crc(int msg, int[] data, int length)
        {

            int size = 1;
            int crc = 0;
            int index = 0;
            int[] tempBuf = new int[251];
            size *= 8;
            length = size;
            tempBuf[index] = 0x9e;
            tempBuf[index + 1] = (byte)(size >> 8);
            tempBuf[index + 2] = (byte)(size & 0xff);
            tempBuf[index + 3] = msg;
            for (int i = 4; i < 4 + (9 - size); i++)
            {
                tempBuf[i] = data[i - 4];
            }
            crc = CALCRC16(crc, tempBuf, 4 + (9 - size));
            tempBuf[4 + 9 - size] = (byte)(crc >> 8);
            tempBuf[5 + 9 - size] = (byte)(crc & 0xff);
            tempBuf[6 + 9 - size] = 0x9a;
            string str = "";
            byte[] tempBuft = new byte[8];
            int j = 0;
            foreach (int elements in tempBuf)
            {
                str += elements.ToString("X2") + " ";
                tempBuft[j] = (byte)elements;
                j++;
                if (j == 8)
                    break;
            }
            Debug.WriteLine(str);
            return tempBuft;
        }
        public static byte[] cal_crc(int msg, int[] data)
        {
            int size = data.Length;
            int length = 7 + size;
            int crc = 0;
            int index = 0;
            int[] tempBuf = new int[251];
            tempBuf[index] = 0x9e;
            tempBuf[index + 1] = (length >> 8) & 0xff;
            tempBuf[index + 2] = length & 0xff;
            tempBuf[index + 3] = msg;
            for (int i = 4; i < 4 + size; i++)
            {
                tempBuf[i] = data[i - 4];
            }

            crc = CALCRC16(crc, tempBuf, size + 4);
            tempBuf[size + 4] = (byte)((crc >> 8) & 0xff);
            tempBuf[size + 5] = (byte)(crc & 0xff);
            tempBuf[size + 6] = 0x9a;

            int j = 0;
            string str = "";
            byte[] tempBuft = new byte[length];
            foreach (int elements in tempBuf)
            {
                str += elements.ToString("X2") + " ";
                tempBuft[j] = (byte)elements;
                j++;
                if (j == length)
                    break;
            }
            Debug.WriteLine(str);
            return tempBuft;
        }
        public static Tuple<bool, Int32> jiaoyan(int[] ushorts)
        {
            int[] ushortst;
            int ml = ushorts[3];

            int cj = 0;
            //搜索设备返回更新设备数据 0x20 搜索设备  0x30停止 ,返回0x21。
            if (ml == 0x21)
            {
                cj = 28;
            }
            //同步成功返回数据体1
            if (ml == 0x23)
            {
                cj = 28;
            }
            if (ml == 0x24)
            {
                cj = 28;
            }
            //扫描返回数据体3
            if (ml == 0x41)
            {
                cj = 10;
            }
            //扫描停止返回数据体3
            if (ml == 0x43)
            {
                cj = 10;
            }
            //随时处理数据体2
            if (ml == 0x26)
            {
                cj = 12;
            }
            if (cj == 0)
                return new Tuple<bool, Int32>(false, ml);
            ushortst = ushorts.Take(cj).ToArray();
            int c = ushortst.Length;
            int ov1 = ushorts[c];
            int ov2 = ushorts[c + 1];
            int cv = CALCRC16(0, ushortst, cj);
            int cv1 = (cv >> 8) & 0xff;
            int cv2 = cv & 0xff;

            return new Tuple<bool, Int32>(cv1 == ov1 && cv2 == ov2, ml);
        }
        public static int[] convert_to_hex(int number)
        {
            int[] array = new int[3] { 0, 0, 0 };
            array[2] = number % 100;
            number = number / 100;
            array[1] = number % 100;
            number = number / 100;
            array[0] = number;
            array[0] = ((array[0] / 10) * 16 + (array[0] % 10));
            array[1] = ((array[1] / 10) * 16 + (array[1] % 10));
            array[2] = ((array[2] / 10) * 16 + (array[2] % 10));
            for (int i = 0; i < 3; i++)
            {
                Console.Write(string.Format("{0:X2},", array[i]) + " ");
            }
            Console.WriteLine(" ");
            return array;
        }
        public static int Asc(string s, int i)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            if (i + 1 <= s.Length)
                return Asc(s[i].ToString());
            else
                return 0;
        }
        public static int Asc(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length == 1)
            {
                ASCIIEncoding a = new ASCIIEncoding();
                int b = (int)a.GetBytes(s)[0];
                return b;
            }
            else
            {
                return 0;
            }
        }
        public static string Cha(int a)
        {
            if (a >= 0 && a <= 255)
            {
                char c = (char)a;
                return c.ToString();
            }
            else
            {
                return "";
            }
        }
        public static string convert_to_dec(params string[] p)
        {
            string str = "";
            byte[] RxdBuf = p.Select((fruit, intx) => new { intx, str = byte.Parse(fruit) }).Select(p => p.str).ToArray();

            ASCIIEncoding ASCIITochar = new ASCIIEncoding();
            char[] ascii = ASCIITochar.GetChars(RxdBuf);
            for (int i = 0; i < 8; i++)
            {
                str += ascii[i];
            }
            return str;
        }
        public static int convert_to_dec(string a1, string a2, string a3)
        {
            int[] ints = new int[3];
            ints[0] = Convert.ToUInt16(a1);
            ints[1] = Convert.ToUInt16(a2);
            ints[2] = Convert.ToUInt16(a3);
            return convert_to_dec(ints);
        }
        public static int convert_to_dec(int[] array)
        {
            int[] f = new int[3] { 0, 0, 0 };
            f[0] = array[0];
            f[1] = array[1];
            f[2] = array[2];
            int a = f[2] & 0xf0;
            a >>= 4;
            a = a * 10 + (f[2] & 0x0f);
            int a32 = a;
            a = f[1] & 0xf0;
            a >>= 4;
            a = a * 10 + (f[1] & 0x0f);
            int b32 = a;
            a32 = a32 + b32 * 100;
            a = f[0] & 0xf0;
            a >>= 4;
            a = a * 10 + (f[0] & 0x0f);
            b32 = a;
            a32 = a32 + b32 * 10000;
            Console.WriteLine(a32);
            return a32;
        }
        //public static bool iss = true;
        //public static ObservableCollection<IDfile> dfiles = new ObservableCollection<IDfile>();
        //public static readonly object balanceLock = new object();

        //public static void sxh()
        //{
        //    Task.Run(async () =>
        //    {
        //        while (iss)
        //        {
        //            lock (balanceLock)
        //            {
        //                foreach (var f in dfiles)
        //                {
        //                    if (f.Rf_value_left > 0)
        //                    {
        //                        f.Rf_value_left -= 1;
        //                    }
        //                }
        //            }
        //            await Task.Delay(50);
        //        }
        //    });
        //}



        //-------------CRC校验相关的-----------------

        #region CRC校验相关的

        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="start">起点</param>
        /// <param name="len">长度</param>
        /// <returns>byte[]</returns>
        public static byte[] CRC_jy(byte[] buffer, UInt16 start, UInt16 len)
        {
            byte[] buffer2 = buffer.Take(len).ToArray();//数组复制

            byte[] buffer1 = SoftCRC16Handle.CRC16(buffer2);
            //Console.WriteLine("串口数据CRC_16:" + Byte_string(buffer1));
            return buffer1;
        }
        /// <summary>
        /// 比较当前byte数组与另一数组是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target">需要比较的数组。</param>
        /// <returns></returns>
        public static bool EqualsBytes(byte[] obj, byte[] target)
        {
            if (obj.Length != target.Length)
                return false;
            for (int i = 0; i < obj.Length; i++)
            {
                if (obj[i] != target[i])
                    return false;
            }
            return true;
        }
        //public static readonly object lockFlag = new object();
        /// <summary>
        /// 判断返回的CRC校验对不对
        /// </summary>
        /// <param name="A">数据值</param>
        /// <returns>bool(True/False)</returns>
        public static bool CRC_PD(byte[] A)
        {

            byte[] B = CRC_jy(A, 0, (ushort)(A.Length - 2));

            if (EqualsBytes(A, B))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public byte[] call_Crc16(byte[] buffer, int start = 0, int len = 0)
        {
            if (buffer == null || buffer.Length == 0) return null;
            if (start < 0) return null;
            if (len == 0) len = buffer.Length - start;
            int length = start + len;
            if (length > buffer.Length) return null;
            ushort crc = 0;// Initial value
            for (int i = start; i < length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) > 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);// 0xA001 = reverse 0x8005
                    else
                        crc = (ushort)(crc >> 1);
                }
            }
            byte[] ret = BitConverter.GetBytes(crc);
            Array.Reverse(ret);
            return ret;
        }
        #endregion
    }
}
