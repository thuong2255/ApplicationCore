using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Utilities.Helpers;

namespace SystemCoreApp.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductController(IProductService productService, 
            IProductCategoryService productCategoryService,
            IHostingEnvironment hostingEnvironment
            )
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveEntity(ProductViewModel productVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            productVm.SeoAlias = TextHelper.ToUnsignString(productVm.Name);

            if (productVm.Id == 0)
            {
                _productService.Add(productVm);
            }
            else
            {
                _productService.Update(productVm);
            }

            _productService.Save();

            return new OkObjectResult(productVm);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _productService.GetAll();
            return new OkObjectResult(result);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _productService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetProductCategory()
        {
            var productCategories = _productCategoryService.GetAll();
            return new OkObjectResult(productCategories);
        }

        [HttpGet]
        public IActionResult GetAllPaging(int? productCategoryId, string keyword, int page, int pageSize)
        {
            var result = _productService.GetAllPaging(productCategoryId, keyword, page, pageSize);
            return new OkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            _productService.Delete(id);
            _productService.Save();

            return new OkObjectResult(id);
        }

        [HttpPost]
        public IActionResult ImportExcel(int categoryId)
        {

            var files = Request.Form.Files;

            if (files != null && files.Count > 0)
            {
                var file = files[0];
                var filename = ContentDispositionHeaderValue
                                   .Parse(file.ContentDisposition)
                                   .FileName
                                   .Trim('"');

                string folder = _hostingEnvironment.WebRootPath + $@"\uploaded\excels";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = Path.Combine(folder, filename);

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                _productService.ImportExcel(filePath, categoryId);
                _productService.Save();
                return new OkObjectResult(filePath);
            }
            return new NoContentResult();
        }
    }
}