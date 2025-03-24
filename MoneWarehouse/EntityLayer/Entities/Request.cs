using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.Enums;

namespace EntityLayer.Entities
{
    public class Request
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, StringLength(50)] public string RequestNumber { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Normal";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? CompletedDate { get; set; }
        public string? Description { get; set; }
        public string? RequestType { get; set; }
        public DateTime? DueDate { get; set; }
        public string? AssignedTo { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
