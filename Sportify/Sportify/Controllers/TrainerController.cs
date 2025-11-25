using Microsoft.AspNetCore.Mvc;

namespace Sportify.Controllers
{
    public class TrainerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
