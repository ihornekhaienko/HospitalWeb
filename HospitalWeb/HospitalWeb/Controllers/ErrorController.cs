using HospitalWeb.ViewModels.Error;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(ErrorViewModel model)
        {
            return View(model);
        }
    }
}
