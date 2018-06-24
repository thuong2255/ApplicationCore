using System;
using System.Collections.Generic;
using System.Text;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;

namespace SystemCore.Data.EF.Repositories
{
    public class FooterRepository : EFRepository<Footer, string>, IFooterRepository
    {
        public FooterRepository(AppDbContex contex) : base(contex)
        {
        }
    }
}
