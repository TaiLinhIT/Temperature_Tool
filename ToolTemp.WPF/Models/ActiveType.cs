using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToolTemp.WPF.Models
{
    [Table("ActiveType")]
    public class ActiveType
    {
        [Key]
        public int activeid { get; set; }
        public string name { get; set; }
    }
}
