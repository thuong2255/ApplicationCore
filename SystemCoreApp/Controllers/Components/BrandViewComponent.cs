using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SystemCoreApp.Controllers.Components
{
    public class BrandViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}