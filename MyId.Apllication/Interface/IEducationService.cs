using MyId.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Application.Interface
{
    public interface IEducationService
    {
        Task<IEnumerable<EducationDto>> GetAllAsync(string userId, bool isAdmin);
        Task<EducationDto?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<EducationDto> CreateAsync(CreateEducationDto dto, string userId);
        Task<bool> UpdateAsync(int id, CreateEducationDto dto, string userId, bool isAdmin);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }
}
