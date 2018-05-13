using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;
using SystemCoreApp.Authorization;

namespace SystemCoreApp.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public UserController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            var result =  await _authorizationService.AuthorizeAsync(User, "USER", Operations.Read);
            if(result.Succeeded)
            {
                return View();
            }
            return new RedirectResult("/Admin/Login/Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return new OkObjectResult(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetById(id);
            return new OkObjectResult(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            await _userService.DeleteAsync(id);
            return new OkObjectResult(id);
        }

        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var users = _userService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(users);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(UserVm userVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (userVm.Id == null)
            {
                await _userService.AddAsync(userVm);
            }
            else
            {
                await _userService.UpdateAsync(userVm);
            }

            return new OkObjectResult(userVm);
        }
    }
}