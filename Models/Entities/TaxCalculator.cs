using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Domain.Entities
{
    public class TaxCalculator
    {
        [Key]
        public int Id { get; set; }

        public decimal GrossIncome { get; set; }

        public decimal TaxableIncome { get; set; }

        public decimal EstimatedTax { get; set; }

        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
    
}
