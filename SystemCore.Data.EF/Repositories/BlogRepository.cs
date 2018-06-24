using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;

namespace SystemCore.Data.EF.Repositories
{
    public class BlogRepository : EFRepository<Blog, int>, IBlogRepository
    {
        public BlogRepository(AppDbContex contex) : base(contex)
        {

        }
    }
}