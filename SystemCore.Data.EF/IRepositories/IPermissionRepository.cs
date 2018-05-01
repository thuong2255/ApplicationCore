using SystemCore.Data.Entities;
using SystemCore.Infrastructure.Interfaces;

namespace SystemCore.Data.EF.IRepositories
{
    public interface IPermissionRepository : IRepository<Permission, int>
    {
    }
}