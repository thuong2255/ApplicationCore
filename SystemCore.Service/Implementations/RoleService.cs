using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;
using SystemCore.Infrastructure.Interfaces;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IFunctionRepository _functionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(RoleManager<AppRole> roleManager,
            IFunctionRepository functionRepository,
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _functionRepository = functionRepository;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddAsync(RoleVm roleVm)
        {
            var role = new AppRole
            {
                Name = roleVm.Name,
                Description = roleVm.Description
            };

            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded;
        }

        public Task<bool> CheckPermission(string functionId, string action, string[] roles)
        {
            var functions = _functionRepository.FindAll();
            var permissons = _permissionRepository.FindAll();

            var query = from f in functions
                        join p in permissons on f.Id equals p.FunctionId
                        join r in _roleManager.Roles on p.RoleId equals r.Id
                        where roles.Contains(r.Name) && f.Id == functionId
                        && ((p.CanCreate && action == "Create")
                         || (p.CanUpdate && action == "Update")
                         || (p.CanDelete && action == "Delete")
                         || (p.CanRead && action == "Read"))
                        select p;
            return query.AnyAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            await _roleManager.DeleteAsync(role);
        }

        public async Task<List<RoleVm>> GetAllAsync()
        {
            return await _roleManager.Roles.ProjectTo<RoleVm>().ToListAsync();
        }

        public PagedResult<RoleVm> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _roleManager.Roles;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword) || x.Description.Contains(keyword));
            }

            int totalRow = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<RoleVm>().ToList();

            var paginationSet = new PagedResult<RoleVm>
            {
                Results = data,
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRow
            };
            return paginationSet;
        }

        public async Task<RoleVm> GetById(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            return Mapper.Map<AppRole, RoleVm>(role);
        }

        public List<PermissionVm> GetListPermissionWithRole(Guid roleId)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();

            var query = from f in functions
                        join p in permissions on f.Id equals p.FunctionId into fp
                        from p in fp.DefaultIfEmpty()
                        where p != null && p.RoleId == roleId
                        select new PermissionVm
                        {
                            RoleId = roleId,
                            FunctionId = f.Id,
                            CanCreate = p != null ? p.CanCreate : false,
                            CanDelete = p != null ? p.CanDelete : false,
                            CanRead = p != null ? p.CanRead : false,
                            CanUpdate = p != null ? p.CanUpdate : false
                        };
            return query.ToList();
        }

        public void SavePermission(List<PermissionVm> permissionVms, Guid roleId)
        {
            var permissions = Mapper.Map<List<PermissionVm>, List<Permission>>(permissionVms);

            var permissionOld = _permissionRepository.FindAll().Where(x => x.RoleId == roleId).ToList();

            if (permissionOld.Count() > 0)
                _permissionRepository.RemoveMulti(permissionOld);

            foreach (var item in permissions)
            {
                _permissionRepository.Add(item);
            }
            _unitOfWork.Commit();
        }

        public async Task UpdateAsync(RoleVm roleVm)
        {
            var role = await _roleManager.FindByIdAsync(roleVm.Id);
            role.Name = roleVm.Name;
            role.Description = roleVm.Description;
            await _roleManager.UpdateAsync(role);
        }
    }
}