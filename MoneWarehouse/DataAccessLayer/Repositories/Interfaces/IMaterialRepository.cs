using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using EntityLayer.Enums;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IMaterialRepository : IGenericRepository<Material>
    {
        Task<IEnumerable<Material>> GetMaterialsByUnitAsync(EntityLayer.Enums.MaterialUnit unit);
    }
}
