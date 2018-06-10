using System;
using System.Collections.Generic;
using System.Text;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;

namespace SystemCore.Data.EF.Repositories
{
    public class ProductQuantityRepository : EFRepository<ProductQuantity, int>, IProductQuantityRepository
    {
        public ProductQuantityRepository(AppDbContex contex) : base(contex)
        {
        }
    }
}
