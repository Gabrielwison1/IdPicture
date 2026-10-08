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
    public class SkillRepository : GenericRepository<Skills>, ISkillRepository
    {
        public SkillRepository(AppDbContext context) : base(context) { }
    }
}
