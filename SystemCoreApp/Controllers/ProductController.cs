using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SystemCore.Service.Interfaces;
using SystemCoreApp.Models;

namespace SystemCoreApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService productService,
            IProductCategoryService productCategoryService,
            IConfiguration configuration)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _configuration = configuration;
        }

        [Route("products.html")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("{alias}-c.{id}.html")]
        public IActionResult Catalog(int id, int? pageSize, string sortBy, int page = 1)
        {
            pageSize = pageSize ?? _configuration.GetValue<int>("PageSize");

            ViewData["BodyClass"] = "shop_grid_full_width_page";

            var catalog = new CatalogViewModel
            {
                PageSize = pageSize,
                SortType = sortBy,
                Category = _productCategoryService.GetById(id),
                Data = _productService.GetAllPaging(id, string.Empty,page, pageSize.Value)
            };

            return View(catalog);
        }
    }
}