using MyId.Application.DTOs;
using MyId.Application.Interface;
using MyId.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Application.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly IExperienceRepository _repository;
        public ExperienceService(IExperienceRepository repository) => _repository = repository;

        public async Task<IEnumerable<ExperienceDto>> GetAllAsync(string userId, bool isAdmin)
        {
            var items = isAdmin ? await _repository.GetAllAsync() : await _repository.GetByUserIdAsync(userId);
            return items.Select(x => new ExperienceDto { Id = x.Id, Company = x.Company, Role = x.Role, Description = x.Description, StartDate = x.StartDate, EndDate = x.EndDate, UserId = x.UserId });
        }

        public async Task<ExperienceDto?> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return null;
            return new ExperienceDto { Id = item.Id, Company = item.Company, Role = item.Role, Description = item.Description, StartDate = item.StartDate, EndDate = item.EndDate, UserId = item.UserId };
        }

        public async Task<ExperienceDto> CreateAsync(CreateExperienceDto dto, string userId)
        {
            var entity = new Experience { Company = dto.Company, Role = dto.Role, Description = dto.Description, StartDate = dto.StartDate, EndDate = dto.EndDate, UserId = userId };
            var created = await _repository.AddAsync(entity);
            return new ExperienceDto { Id = created.Id, Company = created.Company, Role = created.Role, Description = created.Description, StartDate = created.StartDate, EndDate = created.EndDate, UserId = created.UserId };
        }

        public async Task<bool> UpdateAsync(int id, CreateExperienceDto dto, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return false;

            item.Company = dto.Company;
            item.Role = dto.Role;
            item.Description = dto.Description;
            item.StartDate = dto.StartDate;
            item.EndDate = dto.EndDate;

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
