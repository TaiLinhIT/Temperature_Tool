using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolTemp.WPF.Models
{
    [Table("dv_style")]
    public class Style
    {
        public int Id { get; set; }
        public string? NameStyle { get; set; }
        [Column("SoleMax")]
        public decimal DeMax { get; set; }
        [Column("SoleMin")]
        public decimal DeMin { get; set; }
        [Column("ShoesMax")]
        public decimal GiayMax { get; set; }
        [Column("ShoesMin")]
        public decimal GiayMin { get; set; }
        public string? Devid { get; set; } // Cột này nếu có trong DB

        [Column("Standard_temp")]
        public string? StandardTemp { get; set; }
        [Column("Compensate_Vaild")]
        public decimal? CompensateVaild { get; set; }
    }


}
