﻿using AutoMapper;
using System;
using SystemCore.Data.Entities;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Service.ViewModels.System;

namespace SystemCore.Service.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ProductCategoryViewModel, ProductCategory>()
                .ConstructUsing(c => new ProductCategory(c.Name, c.Description, c.ParentId, c.HomeOrder, c.Image, c.HomeFlag,
                c.DateCreated, c.DateModified, c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription, c.Status, c.SortOrder));

            CreateMap<ProductViewModel, Product>()
               .ConstructUsing(c => new Product(c.Name, c.CategoryId, c.Image, c.Price, c.OriginalPrice,
               c.PromotionPrice, c.Description, c.Content, c.HomeFlag, c.HotFlag, c.Tags, c.Unit, c.Status,
               c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription));

            CreateMap<UserVm, AppUser>()
            .ConstructUsing(c => new AppUser(c.Id.GetValueOrDefault(Guid.Empty), c.FullName, c.UserName,
            c.Email, c.PhoneNumber, c.Avatar, c.Status));
        }
    }
}