using MyId.Domain.Entities;
using MyId.Persistence.Data;
using MyId.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyId.Persistence.Repositories
{
    public class ExperienceRepository : GenericRepository<Experience>, IExperienceRepository
    {
        public ExperienceRepository(AppDbContext context) : base(context) { }
    }
}
