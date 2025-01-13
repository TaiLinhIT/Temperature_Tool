using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolTemp.WPF.Models
{
    [Table("devices")]
    public class Device
    {
        public string DevId { get; set; }
        public string Name { get; set; }
        public int ActiveId { get; set; }
        public int TypeId { get; set; }
    }

}
