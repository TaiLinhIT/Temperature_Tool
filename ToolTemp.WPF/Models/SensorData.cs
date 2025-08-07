using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToolTemp.WPF.Models
{
    [Table("SensorData")]
    public class SensorData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int logid { get; set; }
        public int devid { get; set; }
        public int codeid { get; set; }
        public double value { get; set; }
        public DateTime day { get; set; }
    }
}
