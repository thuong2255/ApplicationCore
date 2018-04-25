using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SystemCore.Service.Interfaces;

namespace SystemCoreApp.Areas.Admin.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReOrder(int sourceId, int targetId)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            if (sourceId == targetId)
                return new BadRequestResult();

            _productCategoryService.ReOrder(sourceId, targetId);

            _productCategoryService.Save();

            return new OkResult();
        }

        [HttpPost]
        public IActionResult UpdateParentId(int sourceId, int targetId, Dictionary<int, int> items)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            if (sourceId == targetId)
                return new BadRequestResult();

            _productCategoryService.UpdateParentId(sourceId, targetId, items);
            _productCategoryService.Save();

            return new OkResult();
        }

        [HttpGet]
        public IActionResult GetProductCategory()
        {
            var productCategories = _productCategoryService.GetAll();
            return new OkObjectResult(productCategories);
        }
    }
}