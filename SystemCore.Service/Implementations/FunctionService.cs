using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;

namespace SystemCore.Service.Implementations
{
    public class FunctionService : IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;

        public FunctionService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public Task<List<FunctionVm>> GetAll()
        {
            return _functionRepository.FindAll().ProjectTo<FunctionVm>().ToListAsync();
        }

        public Task<List<FunctionVm>> GetAllByPermission(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}