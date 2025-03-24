using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Codes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CodeId { get; set; }
        public string Code { get; set; }
        public string CodeName { get; set; }
        public int? SizeId { get; set; }
        public Size? Size { get; set; }
    }
}
