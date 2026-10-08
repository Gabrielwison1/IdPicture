using MyId.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Application.Interface
{
    public interface ITaxCalculatorRepository : IGenericRepository<TaxCalculator> { }
}
