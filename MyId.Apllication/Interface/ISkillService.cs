using MyId.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Application.Interface
{
    public interface ISkillService
    {
        Task<IEnumerable<SkillDto>> GetAllAsync(string userId, bool isAdmin);
        Task<SkillDto?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<SkillDto> CreateAsync(CreateSkillDto dto, string userId);
        Task<bool> UpdateAsync(int id, CreateSkillDto dto, string userId, bool isAdmin);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }
}
