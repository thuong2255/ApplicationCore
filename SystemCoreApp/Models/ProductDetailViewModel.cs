using System.Collections.Generic;
using SystemCore.Service.ViewModels.Common;
using SystemCore.Service.ViewModels.Product;

namespace SystemCoreApp.Models
{
    public class ProductDetailViewModel
    {
        public ProductViewModel Product { get; set; }

        public List<ProductViewModel> RelatedProducts { get; set; }

        public ProductCategoryViewModel Category { get; set; }

        public List<ProductImageViewModel> ProductImages { set; get; }

        public List<ProductViewModel> UpsellProducts { get; set; }

        public List<TagViewModel> Tags { set; get; }
    }
}