using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;

namespace SystemCore.Data.EF.Repositories
{
    public class WholePriceRepository : EFRepository<WholePrice, int>, IWholePriceRepository
    {
        public WholePriceRepository(AppDbContex contex) : base(contex)
        {
        }
    }
}