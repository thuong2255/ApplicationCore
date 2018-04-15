using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using SystemCore.Data.Entities;
using SystemCore.Service.ViewModels.Product;

namespace SystemCore.Service.AutoMapper
{
    public class DomainToViewmodelMappingProfile : Profile
    {
        public DomainToViewmodelMappingProfile()
        {
            CreateMap<ProductCategory, ProductCategoryViewModel>();
        }
    }
}
