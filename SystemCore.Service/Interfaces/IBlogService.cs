using System;
using System.Collections.Generic;
using System.Text;
using SystemCore.Service.ViewModels.Blog;

namespace SystemCore.Service.Interfaces
{
    public interface IBlogService
    {
        List<BlogViewModel> GetLastest(int top);
    }
}
