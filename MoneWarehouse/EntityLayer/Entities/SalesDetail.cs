using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLayer.Entities
{
    public class SalesDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SalesId { get; set; }
        public Sales Sales { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public int? PlıntusStockId { get; set; }
        public PlıntusStock? PlıntusStock { get; set; }
        public int? InjectionStockId { get; set; }
        public InjectionStock? InjectionStock { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
