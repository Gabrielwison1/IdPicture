using MyId.Application.DTOs;

namespace MyId.Application.Interface
{
    public interface IGalleryService
    {
        Task<IEnumerable<GalleryItemDto>> GetAllAsync(string userId, bool isAdmin);
        Task<GalleryItemDto?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<GalleryItemDto> CreateAsync(CreateGalleryItemDto dto, string userId);
        Task<bool> UpdateAsync(int id, CreateGalleryItemDto dto, string userId, bool isAdmin);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }
}