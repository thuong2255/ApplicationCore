using System.Collections.Generic;
using SystemCore.Data.Entities;
using SystemCore.Infrastructure.Interfaces;

namespace SystemCore.Data.EF.IRepositories
{
    public interface IProductCategoryRepository : IRepository<ProductCategory, int>
    {
        List<ProductCategory> GetByAlias(string alias);
    }
}