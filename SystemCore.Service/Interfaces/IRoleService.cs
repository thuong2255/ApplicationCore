using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemCore.Service.ViewModels.System;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AddAsync(RoleVm roleVm);

        Task DeleteAsync(Guid id);

        Task<List<RoleVm>> GetAllAsync();

        PagedResult<RoleVm> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<RoleVm> GetById(Guid id);

        Task UpdateAsync(RoleVm roleVm);

        List<PermissionVm> GetListPermissionWithRole(Guid roleId);

        void SavePermission(List<PermissionVm> permissionVms, Guid roleId);

        Task<bool> CheckPermission(string functionId, string action, string[] roles);
    }
}