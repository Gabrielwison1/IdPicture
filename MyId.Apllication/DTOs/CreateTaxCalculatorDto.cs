using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Application.DTOs
{
    public class CreateTaxCalculatorDto
    {
        public decimal GrossIncome { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal EstimatedTax { get; set; }
    }
}
