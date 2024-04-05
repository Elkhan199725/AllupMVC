using Microsoft.AspNetCore.Mvc;

namespace AllupWebApplication.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
