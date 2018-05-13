using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;
using SystemCore.Utilities.Constants;
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
            var roles = ((ClaimsPrincipal)User).GetSpecificClaim(CommonConstants.UserClaims.Roles);

            List<FunctionVm> functions;

            if(roles.Split(";").Contains(CommonConstants.AppRole.AdminRole))
            {
                functions = await _functionService.GetAll();
            }
            else
            {
                var Id = ((ClaimsPrincipal)User).GetSpecificClaim("Id").ToString();
                functions = await _functionService.GetAllByPermission(Guid.Parse(Id));
            }

            return View(functions);
        }
    }
}
