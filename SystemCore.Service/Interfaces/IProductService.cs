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

        List<ProductQuantityViewModel> GetQuantities(int productId);

        void AddQuantity(int productId, List<ProductQuantityViewModel> quantities);

        List<ProductImageViewModel> GetImages(int productId);

        void AddImages(int productId, string[] images);

        List<WholePriceViewModel> GetWholePrices(int productId);

        void AddWholePrice(int productId, List<WholePriceViewModel> wholePrices);
    }
}