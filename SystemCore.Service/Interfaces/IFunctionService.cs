using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemCore.Service.ViewModels.System;

namespace SystemCore.Service.Interfaces
{
    public interface IFunctionService
    {
        Task<List<FunctionVm>> GetAll();

        Task<List<FunctionVm>> GetAllByPermission(Guid userId);
    }
}