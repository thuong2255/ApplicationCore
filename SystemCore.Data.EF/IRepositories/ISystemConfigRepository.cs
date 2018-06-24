using SystemCore.Data.Entities;
using SystemCore.Infrastructure.Interfaces;

namespace SystemCore.Data.EF.IRepositories
{
    public interface ISystemConfigRepository : IRepository<SystemConfig, string>
    {
    }
}