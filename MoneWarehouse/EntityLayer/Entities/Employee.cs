using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Employee
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
        [Required, StringLength(50)] public string FirstName { get; set; }
        [Required, StringLength(50)] public string LastName { get; set; }
        [StringLength(50)] public string Department { get; set; }
        [NotMapped] public string EmployeeName => $"{FirstName} {LastName}";
        [StringLength(100)] public string? Email { get; set; }
        [StringLength(20)] public string? Phone { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Now;
        public DateTime? TerminationDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}

