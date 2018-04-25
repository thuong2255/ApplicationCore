using Microsoft.AspNetCore.Mvc;
using SystemCore.Service.Interfaces;

namespace SystemCoreApp.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _productService.GetAll();
            return new OkObjectResult(result);
        }

        [HttpGet]
        public IActionResult GetAllPaging(int? productCategoryId, string keyword, int page, int pageSize)
        {
            var result = _productService.GetAllPaging(productCategoryId, keyword, page, pageSize);
            return new OkObjectResult(result);
        }
    }
}