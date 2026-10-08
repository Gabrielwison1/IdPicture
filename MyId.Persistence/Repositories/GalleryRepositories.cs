using MyId.Application.Interface;
using MyId.Domain.Entities;
using MyId.Persistence.Data;

namespace MyId.Persistence.Repositories
{
    public class GalleryRepositories : GenericRepository<GalleryItem>, IGalleryRepository
    {
        public GalleryRepositories(AppDbContext context) : base(context) { }
    }
}