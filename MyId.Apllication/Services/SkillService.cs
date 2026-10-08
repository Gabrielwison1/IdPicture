using MyId.Application.DTOs;
using MyId.Application.Interface;
using MyId.Domain.Entities;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _repository;
    public SkillService(ISkillRepository repository) => _repository = repository;

    public async Task<IEnumerable<SkillDto>> GetAllAsync(string userId, bool isAdmin)
    {
        var items = isAdmin ? await _repository.GetAllAsync() : await _repository.GetByUserIdAsync(userId);
        return items.Select(x => new SkillDto { Id = x.Id, Name = x.Name, Proficiency = x.Proficiency, UserId = x.UserId });
    }

    public async Task<SkillDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null || (!isAdmin && item.UserId != userId)) return null;
        return new SkillDto { Id = item.Id, Name = item.Name, Proficiency = item.Proficiency, UserId = item.UserId };
    }

    public async Task<SkillDto> CreateAsync(CreateSkillDto dto, string userId)
    {
        var entity = new Skills { Name = dto.Name, Proficiency = dto.Proficiency, UserId = userId };
        var created = await _repository.AddAsync(entity);
        return new SkillDto { Id = created.Id, Name = created.Name, Proficiency = created.Proficiency, UserId = created.UserId };
    }

    public async Task<bool> UpdateAsync(int id, CreateSkillDto dto, string userId, bool isAdmin)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null || (!isAdmin && item.UserId != userId)) return false;

        item.Name = dto.Name;
        item.Proficiency = dto.Proficiency;

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