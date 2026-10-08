using MyId.Application.Interface;
using MyId.Domain.Entities;
using MyId.Persistence.Data;

namespace MyId.Persistence.Repositories
{
    public class TaxCalculatorRepository : GenericRepository<TaxCalculator>, ITaxCalculatorRepository
    {
        public TaxCalculatorRepository(AppDbContext context) : base(context) { }
    }
}