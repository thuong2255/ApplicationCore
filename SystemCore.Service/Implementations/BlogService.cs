using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Enums;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.Blog;

namespace SystemCore.Service.Implementations
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public List<BlogViewModel> GetLastest(int top)
        {
            return _blogRepository.FindAll(x => x.Status == Status.Active).OrderByDescending(x => x.DateCreated).Take(top)
                .ProjectTo<BlogViewModel>().ToList();
        }
    }
}