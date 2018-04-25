using System;
using System.Collections.Generic;
using System.Text;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Interfaces
{
    public interface IProductService
    {
        List<ProductViewModel> GetAll();

        PagedResult<ProductViewModel> GetAllPaging(int? productCategoryId,string keyword, int page, int pageSize);
    }
}
