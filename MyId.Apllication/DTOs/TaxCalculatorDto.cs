using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Application.DTOs
{
    public class TaxCalculatorDto
    {

        public int Id { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal EstimatedTax { get; set; }
        public DateTime CalculatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
