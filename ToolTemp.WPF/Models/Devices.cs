using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToolTemp.WPF.Models
{
    [Table("devices")]
    public class Device
    {
        [Key]
        public string DevId { get; set; }
        public string Name { get; set; }
        public int ActiveId { get; set; }
        public int TypeId { get; set; }
    }

}
