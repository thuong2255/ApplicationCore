using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Utilities.Dtos;

namespace SystemCoreApp.Models
{
    public class CatalogViewModel
    {
        public PagedResult<ProductViewModel> Data { get; set; }

        public ProductCategoryViewModel Category { get; set; }

        public int? PageSize { get; set; }

        public string SortType { get; set; }

        public List<SelectListItem> PageSizes { get; } = new List<SelectListItem>
        {
            new SelectListItem {Value = "12", Text = "12"},
            new SelectListItem {Value = "24", Text = "24"},
            new SelectListItem {Value = "48", Text = "48"}
        };

        public List<SelectListItem> SortTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem(){Value = "lastest",Text = "Mới nhất"},
            new SelectListItem(){Value = "price",Text = "Giá"},
            new SelectListItem(){Value = "name",Text = "Tên"},
        };
    }
}
