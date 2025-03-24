using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Repositories.Interfaces;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Material>> GetMaterialsByUnitAsync(EntityLayer.Enums.MaterialUnit unit)
        {
            return await _dbSet.Where(m => m.Unit == unit).ToListAsync();
        }
    }
}
