using System;
using System.Collections.Generic;
using System.Linq;
using SystemCore.Data.Entities;
using SystemCore.Infrastructure.Interfaces;

namespace SystemCore.Data.EF.IRepositories
{
    public interface IFunctionRepository : IRepository<Function, string>
    {
        IQueryable<Function> GetAllByPermission(Guid userId); 
    }
}