using AutoMapper;
using SystemCore.Data.Entities;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Service.ViewModels.System;

namespace SystemCore.Service.AutoMapper
{
    public class DomainToViewmodelMappingProfile : Profile
    {
        public DomainToViewmodelMappingProfile()
        {
            CreateMap<ProductCategory, ProductCategoryViewModel>();
            CreateMap<Function, FunctionVm>();
            CreateMap<Product, ProductViewModel>();
            CreateMap<AppUser, UserVm>();
            CreateMap<Permission, PermissionVm>();
        }
    }
}