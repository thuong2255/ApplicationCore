using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}
