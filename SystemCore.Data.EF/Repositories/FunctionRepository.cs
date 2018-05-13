using System;
using System.Collections.Generic;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;
using System.Linq;

namespace SystemCore.Data.EF.Repositories
{
    public class FunctionRepository : EFRepository<Function, string>, IFunctionRepository
    {
        private AppDbContex _context;
        public FunctionRepository(AppDbContex contex) : base(contex)
        {
            _context = contex;
        }
   
        public IQueryable<Function> GetAllByPermission(Guid userId)
        {
            var result = from a in _context.Functions
                         join b in _context.Permissions on a.Id equals b.FunctionId
                         join c in _context.AppRoles on b.RoleId equals c.Id
                         join d in _context.UserRoles on c.Id equals d.RoleId
                         where d.UserId == userId && (b.CanCreate || b.CanDelete || b.CanRead)
                         select a;

            return result;
        }
    }
}