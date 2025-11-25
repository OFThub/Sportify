using Microsoft.AspNetCore.Mvc;

namespace Sportify.Controllers
{
    public class GymController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
