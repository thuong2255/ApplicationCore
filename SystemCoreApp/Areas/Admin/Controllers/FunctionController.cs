using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;

namespace SystemCoreApp.Areas.Admin.Controllers
{
    public class FunctionController : BaseController
    {
        private readonly IFunctionService _functionService;

        public FunctionController(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var model = await _functionService.GetAll();
            var rootFunctions = model.Where(c => c.ParentId == null);
            var items = new List<FunctionVm>();
            foreach (var function in rootFunctions)
            {
                //add the parent category to the item list
                items.Add(function);
                //now get all its children (separate Category in case you need recursion)
                GetByParentId(model.ToList(), function, items);
            }
            return new ObjectResult(items);
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Private Functions
        private void GetByParentId(IEnumerable<FunctionVm> allFunctions,
            FunctionVm parent, IList<FunctionVm> items)
        {
            var functionsEntities = allFunctions as FunctionVm[] ?? allFunctions.ToArray();
            var subFunctions = functionsEntities.Where(c => c.ParentId == parent.Id);
            foreach (var cat in subFunctions)
            {
                //add this category
                items.Add(cat);
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentId(functionsEntities, cat, items);
            }
        }
        #endregion
    }
}