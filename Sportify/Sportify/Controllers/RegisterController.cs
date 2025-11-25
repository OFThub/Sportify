using Microsoft.AspNetCore.Mvc;

namespace Sportify.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
