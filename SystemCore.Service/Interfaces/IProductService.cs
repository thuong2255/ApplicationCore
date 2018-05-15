using System.Collections.Generic;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Interfaces
{
    public interface IProductService
    {
        List<ProductViewModel> GetAll();

        ProductViewModel GetById(int id);

        ProductViewModel Add(ProductViewModel productVm);

        void Update(ProductViewModel productVm);

        void Delete(int id);

        void ImportExcel(string filePath, int categoryId);

        void Save();

        PagedResult<ProductViewModel> GetAllPaging(int? productCategoryId, string keyword, int page, int pageSize);
    }
}