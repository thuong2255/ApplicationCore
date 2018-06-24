using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;

namespace SystemCore.Data.EF.Repositories
{
    public class SystemConfigRepository : EFRepository<SystemConfig, string>, ISystemConfigRepository
    {
        public SystemConfigRepository(AppDbContex contex) : base(contex)
        {
        }
    }
}