using MyId.Application.DTOs;
using MyId.Application.Interface;
using MyId.Domain.Entities;

namespace MyId.Application.Services
{
    public class EducationService : IEducationService
    {
        private readonly IEducationRepository _repository;
        public EducationService(IEducationRepository repository) => _repository = repository;

        public async Task<IEnumerable<EducationDto>> GetAllAsync(string userId, bool isAdmin)
        {
            var items = isAdmin ? await _repository.GetAllAsync() : await _repository.GetByUserIdAsync(userId);
            return items.Select(x => new EducationDto { Id = x.Id, Institution = x.Institution, Degree = x.Degree, FieldOfStudy = x.FieldOfStudy, StartYear = x.StartYear, EndYear = x.EndYear, UserId = x.UserId });
        }

        public async Task<EducationDto?> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return null;
            return new EducationDto { Id = item.Id, Institution = item.Institution, Degree = item.Degree, FieldOfStudy = item.FieldOfStudy, StartYear = item.StartYear, EndYear = item.EndYear, UserId = item.UserId };
        }

        public async Task<EducationDto> CreateAsync(CreateEducationDto dto, string userId)
        {
            var entity = new Education { Institution = dto.Institution, Degree = dto.Degree, FieldOfStudy = dto.FieldOfStudy, StartYear = dto.StartYear, EndYear = dto.EndYear, UserId = userId };
            var created = await _repository.AddAsync(entity);
            return new EducationDto { Id = created.Id, Institution = created.Institution, Degree = created.Degree, FieldOfStudy = created.FieldOfStudy, StartYear = created.StartYear, EndYear = created.EndYear, UserId = created.UserId };
        }

        public async Task<bool> UpdateAsync(int id, CreateEducationDto dto, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return false;

            item.Institution = dto.Institution;
            item.Degree = dto.Degree;
            item.FieldOfStudy = dto.FieldOfStudy;
            item.StartYear = dto.StartYear;
            item.EndYear = dto.EndYear;

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