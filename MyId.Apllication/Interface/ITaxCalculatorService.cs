using MyId.Application.DTOs;

namespace MyId.Application.Interface
{
    public interface ITaxCalculatorService
    {
        Task<IEnumerable<TaxCalculatorDto>> GetAllAsync(string userId, bool isAdmin);
        Task<TaxCalculatorDto?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<TaxCalculatorDto> CreateAsync(CreateTaxCalculatorDto dto, string userId);
        Task<bool> UpdateAsync(int id, CreateTaxCalculatorDto dto, string userId, bool isAdmin);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }
}