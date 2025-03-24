using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLayer.Entities
{

    public class PlıntusDaily
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PlıntusStockId { get; set; }
        public PlıntusStock PlıntusStock { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }
}
