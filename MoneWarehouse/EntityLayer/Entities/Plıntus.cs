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
    public class Plıntus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlıntusId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Size")]
        public int SizeId { get; set; }
        public Size? Size { get; set; }

        [Required]
        [ForeignKey("Codes")]
        public int CodeId { get; set; }
        public Codes? Codes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastModifiedDate { get; set; }

        public ICollection<PlıntusDaily>? PlıntusDailyEntries { get; set; }

        public ICollection<PlıntusStock>? PlıntusStocks { get; set; }
    }
}
