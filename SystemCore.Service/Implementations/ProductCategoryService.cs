using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;
using SystemCore.Data.Enums;
using SystemCore.Infrastructure.Interfaces;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.Product;

namespace SystemCore.Service.Implementations
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductCategoryService(IProductCategoryRepository productCategoryRepository,
            IProductService productService,
            IUnitOfWork unitOfWork)
        {
            _productCategoryRepository = productCategoryRepository;
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        public ProductCategoryViewModel Add(ProductCategoryViewModel productCategoryVm)
        {
            var productCategory = Mapper.Map<ProductCategoryViewModel, ProductCategory>(productCategoryVm);
            _productCategoryRepository.Add(productCategory);
            return productCategoryVm;
        }

        public void Delete(int id)
        {
            _productCategoryRepository.Remove(id);
        }

        public List<ProductCategoryViewModel> GetAll()
        {
            return _productCategoryRepository.FindAll().OrderBy(x => x.ParentId)
                .ProjectTo<ProductCategoryViewModel>().ToList();
        }

        public List<ProductCategoryViewModel> GetAll(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return GetAll();

            return _productCategoryRepository.FindAll(x => x.Name.ToUpper().Contains(keyword.ToUpper()) || x.Description.ToUpper().Contains(keyword.ToUpper()))
                .OrderBy(x => x.ParentId)
                .ProjectTo<ProductCategoryViewModel>().ToList();
        }

        public List<ProductCategoryViewModel> GetAllByParentId(int parentId)
        {
            return _productCategoryRepository.FindAll(x => x.Status == Status.Active && x.ParentId == parentId)
                .ProjectTo<ProductCategoryViewModel>().ToList();
        }

        public ProductCategoryViewModel GetById(int id)
        {
            return Mapper.Map<ProductCategory, ProductCategoryViewModel>(_productCategoryRepository.FindById(id));
        }

        public ProductCategoryViewModel GetByProductId(int productId)
        {
            var product = _productService.GetById(productId);
            return Mapper.Map<ProductCategory, ProductCategoryViewModel>(_productCategoryRepository.FindById(product.CategoryId));
        }

        public List<ProductCategoryViewModel> GetHomeCategories(int top)
        {
            var query = _productCategoryRepository.FindAll(x => x.HomeFlag == true, c => c.Products)
                                                    .Take(top).ProjectTo<ProductCategoryViewModel>();

            return query.ToList();
        }

        public void ReOrder(int sourceId, int targetId)
        {
            var source = _productCategoryRepository.FindById(sourceId);
            var target = _productCategoryRepository.FindById(targetId);
            int tempOrder = source.SortOrder;
            source.SortOrder = target.SortOrder;
            target.SortOrder = tempOrder;

            _productCategoryRepository.Update(source);
            _productCategoryRepository.Update(target);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ProductCategoryViewModel productCategoryVm)
        {
            var productCategory = Mapper.Map<ProductCategoryViewModel, ProductCategory>(productCategoryVm);
            _productCategoryRepository.Update(productCategory);
        }

        public void UpdateParentId(int sourceId, int targetId, Dictionary<int, int> items)
        {
            var sourceCategory = _productCategoryRepository.FindById(sourceId);
            sourceCategory.ParentId = targetId;
            _productCategoryRepository.Update(sourceCategory);

            //Get all sibling
            var sibling = _productCategoryRepository.FindAll(x => items.ContainsKey(x.Id));
            foreach (var child in sibling)
            {
                child.SortOrder = items[child.Id];
                _productCategoryRepository.Update(child);
            }
        }
    }
}