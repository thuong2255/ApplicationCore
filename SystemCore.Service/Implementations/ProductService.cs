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

namespace SystemCore.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IProductTagRepository _productTagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, ITagRepository tagRepository,
            IUnitOfWork unitOfWork,
            IProductTagRepository productTagRepository)
        {
            _productRepository = productRepository;
            _tagRepository = tagRepository;
            _productTagRepository = productTagRepository;
            _unitOfWork = unitOfWork;
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


    }
}