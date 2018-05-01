using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Service.ViewModels.System;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddAsync(UserVm userVm);

        Task DeleteAsync(string id);

        Task<List<UserVm>> GetAllAsync();

        PagedResult<UserVm> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<UserVm> GetById(string id);

        Task UpdateAsync(UserVm userVm);
    }
}
