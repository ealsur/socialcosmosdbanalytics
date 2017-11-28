using Microsoft.AspNetCore.Mvc;

namespace cosmoschat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
