using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolTemp.WPF.Models
{
    [Table("dv_Factory_Configs")]
    public class Factory
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Line { get; set; }
        public int Address { get;set; }
    }
}
