using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
