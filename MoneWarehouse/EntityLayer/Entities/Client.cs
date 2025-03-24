using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }
        [Required, StringLength(100)] public string CompanyName { get; set; }
        [Required, StringLength(100)] public string ContactName { get; set; }
        [StringLength(100)] public string ContactTitle { get; set; }
        [StringLength(100)] public string Address { get; set; }
        [StringLength(50)] public string Phone { get; set; }
        [StringLength(50)] public string Email { get; set; }
        [StringLength(50)] public string RegComNumber { get; set; }
        [StringLength(50)] public string CIFNumber { get; set; }
        [StringLength(100)] public string BankName { get; set; }
        [StringLength(50)] public string IBAN { get; set; }
        [StringLength(100)] public string DeliveryConditions { get; set; }
        [Required, StringLength(100)] public string Country { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public virtual ICollection<Sales> Sales { get; set; } = new List<Sales>();
    }
}
