using MyId.Application.DTOs;
using MyId.Application.Interface;
using MyId.Domain.Entities;

namespace MyId.Application.Services
{
    public class TaxCalculatorService : ITaxCalculatorService
    {
        private readonly ITaxCalculatorRepository _repository;

        public TaxCalculatorService(ITaxCalculatorRepository repository) => _repository = repository;

        public async Task<IEnumerable<TaxCalculatorDto>> GetAllAsync(string userId, bool isAdmin)
        {
            var items = isAdmin ? await _repository.GetAllAsync() : await _repository.GetByUserIdAsync(userId);
            return items.Select(x => new TaxCalculatorDto
            {
                Id = x.Id,
                GrossIncome = x.GrossIncome,
                TaxableIncome = x.TaxableIncome,
                EstimatedTax = x.EstimatedTax,
                CalculatedAt = x.CalculatedAt,
                UserId = x.UserId
            });
        }

        public async Task<TaxCalculatorDto?> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return null;

            return new TaxCalculatorDto
            {
                Id = item.Id,
                GrossIncome = item.GrossIncome,
                TaxableIncome = item.TaxableIncome,
                EstimatedTax = item.EstimatedTax,
                CalculatedAt = item.CalculatedAt,
                UserId = item.UserId
            };
        }

        public async Task<TaxCalculatorDto> CreateAsync(CreateTaxCalculatorDto dto, string userId)
        {
            // Calculate 25% Tax Rate based on Gross Income
            decimal tax = dto.GrossIncome * 0.25m;

            var item = new TaxCalculator
            {
                GrossIncome = dto.GrossIncome,
                TaxableIncome = dto.TaxableIncome,
                EstimatedTax = tax,
                CalculatedAt = DateTime.UtcNow,
                UserId = userId
            };

            var created = await _repository.AddAsync(item);
            return new TaxCalculatorDto
            {
                Id = created.Id,
                GrossIncome = created.GrossIncome,
                TaxableIncome = created.TaxableIncome,
                EstimatedTax = created.EstimatedTax,
                CalculatedAt = created.CalculatedAt,
                UserId = created.UserId
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateTaxCalculatorDto dto, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return false;

            item.GrossIncome = dto.GrossIncome;
            item.TaxableIncome = dto.TaxableIncome;
            item.EstimatedTax = dto.GrossIncome * 0.25m; // Recalculate 25% Gross Tax

            await _repository.UpdateAsync(item);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return false;

            await _repository.DeleteAsync(item);
            return true;
        }
    }
}