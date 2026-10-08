using MyId.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Application.Interface
{
    public interface IExperienceService
    {
        Task<IEnumerable<ExperienceDto>> GetAllAsync(string userId, bool isAdmin);
        Task<ExperienceDto?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<ExperienceDto> CreateAsync(CreateExperienceDto dto, string userId);
        Task<bool> UpdateAsync(int id, CreateExperienceDto dto, string userId, bool isAdmin);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }
}
