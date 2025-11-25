using Microsoft.AspNetCore.Mvc;

namespace Sportify.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
