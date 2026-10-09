using Microsoft.AspNetCore.Hosting;
using MyId.Application.DTOs;
using MyId.Application.Interface;
using MyId.Domain.Entities;

namespace MyId.Application.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _repository;
        private readonly IWebHostEnvironment _env;

        public GalleryService(IGalleryRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<IEnumerable<GalleryItemDto>> GetAllAsync(string userId, bool isAdmin)
        {
            var items = isAdmin ? await _repository.GetAllAsync() : await _repository.GetByUserIdAsync(userId);
            return items.Select(x => new GalleryItemDto
            {
                Id = x.Id,
                Title = x.Title,
                ImageUrl = x.ImageUrl,
                Description = x.Description,
                UserId = x.UserId
            });
        }

        public async Task<GalleryItemDto?> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return null;

            return new GalleryItemDto { Id = item.Id, Title = item.Title, ImageUrl = item.ImageUrl, Description = item.Description, UserId = item.UserId };
        }

        public async Task<GalleryItemDto> CreateAsync(CreateGalleryItemDto dto, string userId)
        {
            string finalImageUrl = await SaveImageAsync(dto);

            var item = new GalleryItem
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = finalImageUrl,
                UserId = userId
            };

            var created = await _repository.AddAsync(item);
            return new GalleryItemDto { Id = created.Id, Title = created.Title, ImageUrl = created.ImageUrl, Description = created.Description, UserId = created.UserId };
        }

        public async Task<bool> UpdateAsync(int id, CreateGalleryItemDto dto, string userId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || (!isAdmin && item.UserId != userId)) return false;

            item.Title = dto.Title;
            item.Description = dto.Description;

            if (dto.ImageFile != null || !string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                item.ImageUrl = await SaveImageAsync(dto);
            }

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

        private async Task<string> SaveImageAsync(CreateGalleryItemDto dto)
        {
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // Point to persistent directory outside wwwroot (C:\home\site\gallery_uploads)
                string persistentUploadsPath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "gallery_uploads"));

                // Explicitly ensure folder exists on Azure 
                if (!Directory.Exists(persistentUploadsPath))
                {
                    Directory.CreateDirectory(persistentUploadsPath);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{dto.ImageFile.FileName}";
                string filePath = Path.Combine(persistentUploadsPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                return $"/gallery_uploads/{uniqueFileName}";
            }

            return dto.ImageUrl ?? string.Empty;
        }
    }
}