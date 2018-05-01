using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;

namespace SystemCoreApp.Areas.Admin.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return new OkObjectResult(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetById(id);
            return new OkObjectResult(role);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            await _roleService.DeleteAsync(id);
            return new OkObjectResult(id);
        }

        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var roles = _roleService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(roles);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(RoleVm roleVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (string.IsNullOrEmpty(roleVm.Id))
            {
                await _roleService.AddAsync(roleVm);
            }
            else
            {
                await _roleService.UpdateAsync(roleVm);
            }

            return new OkObjectResult(roleVm);
        }
    }
}