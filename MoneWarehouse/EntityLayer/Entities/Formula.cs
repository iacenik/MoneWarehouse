using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLayer.Entities
{
    public class Formula
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FormulaId { get; set; }
        [Required, StringLength(100)] public string FormulaName { get; set; }
        [Required] public string FormulaContent { get; set; }
    }
}
