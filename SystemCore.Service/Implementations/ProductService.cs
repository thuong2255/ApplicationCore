using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;
using SystemCore.Data.Enums;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Utilities.Dtos;
using SystemCore.Utilities.Helpers;
using SystemCore.Utilities.Constants;
using SystemCore.Infrastructure.Interfaces;
using OfficeOpenXml;
using System.IO;
using SystemCore.Service.ViewModels.Common;

namespace SystemCore.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IProductTagRepository _productTagRepository;
        private readonly IProductQuantityRepository _productQuantityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IWholePriceRepository _wholePriceRepository;

        public ProductService(
            IProductRepository productRepository,
            ITagRepository tagRepository,
            IUnitOfWork unitOfWork,
            IWholePriceRepository wholePriceRepository,
            IProductQuantityRepository productQuantityRepository,
            IProductTagRepository productTagRepository,
            IProductImageRepository productImageRepository)
        {
            _productQuantityRepository = productQuantityRepository;
            _productRepository = productRepository;
            _tagRepository = tagRepository;
            _productTagRepository = productTagRepository;
            _productImageRepository = productImageRepository;
            _unitOfWork = unitOfWork;
            _wholePriceRepository = wholePriceRepository;
        }

        public void Update(ProductViewModel productVm)
        {
            var product = Mapper.Map<ProductViewModel, Product>(productVm);



            if (string.IsNullOrEmpty(productVm.Tags))
            {
                var tagIdInDbs = _productTagRepository.FindAll(x => x.ProductId == productVm.Id).Select(y => y.TagId).ToList();

                //Remove ProductTag
                _productTagRepository.RemoveMulti(_productTagRepository.FindAll(x => x.ProductId == productVm.Id).ToList());

                //Remove Tag
                _tagRepository.RemoveMulti(_tagRepository.FindAll(x => tagIdInDbs.Contains(x.Id)).ToList());

            }
            else
            {

                var productTags = new List<ProductTag>();

                var tags = productVm.Tags.Split(';').ToList();

                foreach (var t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);

                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        var newTag = new Tag
                        {
                            Id = tagId,
                            Name = t,
                            Type = CommonConstants.ProductTag
                        };
                        _tagRepository.Add(newTag);
                    }

                    _productTagRepository.RemoveMulti(_productTagRepository.FindAll(x => x.ProductId == productVm.Id).ToList());

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };

                    productTags.Add(productTag);
                }

                foreach (var item in productTags)
                {
                    product.ProductTags.Add(item);
                }
            }
            _productRepository.Update(product);
        }

        public ProductViewModel Add(ProductViewModel productVm)
        {
            var productTags = new List<ProductTag>();

            var product = Mapper.Map<ProductViewModel, Product>(productVm);

            if (!string.IsNullOrEmpty(productVm.Tags))
            {
                string[] tags = productVm.Tags.Split(";");

                foreach (var t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);
                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag
                        {
                            Id = tagId,
                            Name = t,
                            Type = CommonConstants.ProductTag
                        };
                        _tagRepository.Add(tag);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };

                    productTags.Add(productTag);
                }

                foreach (var productTag in productTags)
                {
                    product.ProductTags.Add(productTag);
                }

            }
            _productRepository.Add(product);

            return productVm;
        }

        public void Delete(int id)
        {
            _productRepository.Remove(id);
        }

        public List<ProductViewModel> GetAll()
        {
            return _productRepository.FindAll(x => x.ProductCategory).ProjectTo<ProductViewModel>().ToList();
        }

        public PagedResult<ProductViewModel> GetAllPaging(int? productCategoryId, string keyword, int page, int pageSize)
        {
            var query = _productRepository.FindAll(x => x.ProductCategory).Where(y => y.Status == Status.Active);



            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }

            if (productCategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == productCategoryId);
            }

            int totalRow = query.Count();

            query = query.OrderByDescending(x => x.DateCreated).Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<ProductViewModel>().ToList();

            var paginationSet = new PagedResult<ProductViewModel>
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public ProductViewModel GetById(int id)
        {
            var result = Mapper.Map<Product, ProductViewModel>(_productRepository.FindById(id));
            return result;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void ImportExcel(string filePath, int categoryId)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                Product product;
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    product = new Product();
                    product.CategoryId = categoryId;

                    product.Name = workSheet.Cells[i, 1].Value.ToString();

                    product.Description = workSheet.Cells[i, 2].Value.ToString();

                    decimal.TryParse(workSheet.Cells[i, 3].Value.ToString(), out var originalPrice);
                    product.OriginalPrice = originalPrice;

                    decimal.TryParse(workSheet.Cells[i, 4].Value.ToString(), out var price);
                    product.Price = price;
                    decimal.TryParse(workSheet.Cells[i, 5].Value.ToString(), out var promotionPrice);

                    product.PromotionPrice = promotionPrice;
                    product.Content = workSheet.Cells[i, 6].Value.ToString();
                    product.SeoKeywords = workSheet.Cells[i, 7].Value.ToString();

                    product.SeoDescription = workSheet.Cells[i, 8].Value.ToString();
                    bool.TryParse(workSheet.Cells[i, 9].Value.ToString(), out var hotFlag);

                    product.HotFlag = hotFlag;
                    bool.TryParse(workSheet.Cells[i, 10].Value.ToString(), out var homeFlag);
                    product.HomeFlag = homeFlag;

                    product.Status = Status.Active;

                    _productRepository.Add(product);
                }
            }
        }

        public List<ProductQuantityViewModel> GetQuantities(int productId)
        {
            return _productQuantityRepository.FindAll(x => x.ProductId == productId).ProjectTo<ProductQuantityViewModel>().ToList();
        }

        public void AddQuantity(int productId, List<ProductQuantityViewModel> quantities)
        {
            _productQuantityRepository.RemoveMulti(_productQuantityRepository.FindAll(x => x.ProductId == productId).ToList());

            foreach (var quantity in quantities)
            {
                _productQuantityRepository.Add(new ProductQuantity
                {
                    ColorId = quantity.ColorId,
                    ProductId = quantity.ProductId,
                    SizeId = quantity.SizeId,
                    Quantity = quantity.Quantity
                });
            }
        }

        public List<ProductImageViewModel> GetImages(int productId)
        {
            return _productImageRepository.FindAll(x => x.ProductId == productId).ProjectTo<ProductImageViewModel>().ToList();
        }

        public void AddImages(int productId, string[] images)
        {
            _productImageRepository.RemoveMulti(_productImageRepository.FindAll(x => x.ProductId == productId).ToList());

            foreach (var image in images)
            {
                _productImageRepository.Add(new ProductImage
                {
                    Caption = null,
                    ProductId = productId,
                    Path = image
                });
            }
        }

        public List<WholePriceViewModel> GetWholePrices(int productId)
        {
            return _wholePriceRepository.FindAll(x => x.ProductId == productId).ProjectTo<WholePriceViewModel>().ToList();
        }

        public void AddWholePrice(int productId, List<WholePriceViewModel> wholePrices)
        {
            _wholePriceRepository.RemoveMulti(_wholePriceRepository.FindAll(x => x.ProductId == productId).ToList());

            foreach (var wholePrice in wholePrices)
            {
                _wholePriceRepository.Add(new WholePrice
                {
                    ProductId = productId,
                    FromQuantity = wholePrice.FromQuantity,
                    ToQuantity = wholePrice.ToQuantity,
                    Price = wholePrice.Price
                });
            }
        }

        public List<ProductViewModel> GetLastest(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active)
                .OrderByDescending(x => x.DateCreated)
                .Take(top).ProjectTo<ProductViewModel>()
                .ToList();
        }

        public List<ProductViewModel> GetHotProduct(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active && x.HotFlag == true)
                .OrderByDescending(x => x.DateCreated)
                .Take(top).ProjectTo<ProductViewModel>()
                .ToList();
        }

        public List<ProductViewModel> GetRelatedProducts(int id, int top)
        {
            var product = _productRepository.FindById(id);
            return _productRepository.FindAll(x => x.Status == Status.Active
                && x.Id != product.Id && x.CategoryId == product.CategoryId).OrderByDescending(x => x.DateCreated).Take(top).ProjectTo<ProductViewModel>().ToList();
        }

        public List<ProductViewModel> GetUpsellProducts(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active && x.PromotionPrice != null).OrderByDescending(x => x.DateCreated).Take(top).ProjectTo<ProductViewModel>().ToList();
        }

        public List<TagViewModel> GetProductTags(int productId)
        {
            var tags = _tagRepository.FindAll();
            var productTags = _productTagRepository.FindAll();

            var query = from t in tags
                        join pt in productTags
                        on t.Id equals pt.TagId
                        where pt.ProductId == productId
                        select new TagViewModel()
                        {
                            Id = t.Id,
                            Name = t.Name
                        };
            return query.ToList();
        }
    }
}