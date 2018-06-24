using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SystemCore.Service.Interfaces;
using SystemCoreApp.Models;

namespace SystemCoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly ICommonService _commonService;
        private readonly IBlogService _blogService;

        public HomeController(
            IProductCategoryService productCategoryService,
            IBlogService blogService,
            ICommonService commonService,
            IProductService productService)
        {
            _productCategoryService = productCategoryService;
            _productService = productService;
            _blogService = blogService;
            _commonService = commonService;

        }


        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                HomeCategories = _productCategoryService.GetHomeCategories(10),
                HomeSlides = _commonService.GetSildes("top"),
                HotProducts = _productService.GetHotProduct(5),
                TopSellProducts = _productService.GetLastest(5),
                LastestBlogs = _blogService.GetLastest(5)
            };
            return View(homeViewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
