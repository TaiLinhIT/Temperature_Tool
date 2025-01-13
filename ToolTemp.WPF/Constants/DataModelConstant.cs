using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolTemp.WPF.Models;

namespace ToolTemp.WPF.Constants
{
    public static class DataModelConstant
    {
        public static List<int> BaudrateConst = new List<int>() { 2400, 4800, 9600, 19200, 115200 };
        public static Dictionary<string, string> FactoryConst = new Dictionary<string, string>
        {
            { "Lien Dinh","VA1"  },
            { "Lien Thuan 1","VB1" },
            { "Lien Thuan 2","VB2" },
            { "Campuchia","CA1" },
            { "Bangladesh","MA1" }
        };

        public static List<int> AddressConst = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        
        
    }
}
