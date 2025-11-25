using Microsoft.AspNetCore.Mvc;

namespace Sportify.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
