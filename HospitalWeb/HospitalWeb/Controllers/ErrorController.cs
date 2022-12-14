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

        public IActionResult NotFound(ErrorViewModel model)
        {
            return View(model);
        }

        public IActionResult BadRequest(ErrorViewModel model)
        {
            return View(model);
        }

        public IActionResult InternalServer(ErrorViewModel model)
        {
            return View(model);
        }
    }
}
