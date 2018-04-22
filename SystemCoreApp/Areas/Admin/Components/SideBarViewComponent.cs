using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;
using SystemCoreApp.Extensions;

namespace SystemCoreApp.Areas.Admin.Components
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IFunctionService _functionService;

        public SideBarViewComponent(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roles = ((ClaimsPrincipal)User).GetSpecificClaim("Role");

            List<FunctionVm> functions;

            if(roles.Split(";").Contains("Admin"))
            {
                functions = await _functionService.GetAll();
            }
            else
            {
                functions = new List<FunctionVm>();
                //Todo
            }

            return View(functions);
        }
    }
}
