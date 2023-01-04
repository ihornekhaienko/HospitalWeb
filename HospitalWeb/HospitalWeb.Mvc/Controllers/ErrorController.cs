using HospitalWeb.Mvc.ViewModels.Error;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HospitalWeb.Mvc.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(ErrorViewModel model)
        {
            return View(model);
        }

        public IActionResult Http(HttpStatusCode statusCode, string message)
        {
            if (statusCode == HttpStatusCode.NotFound)
            {
                return RedirectToAction("NotFound", "Error", new ErrorViewModel { Message = message });
            }

            if (statusCode == HttpStatusCode.BadRequest)
            {
                return RedirectToAction("BadRequest", "Error", new ErrorViewModel { Message = message });
            }

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("InternalServer", "Error", new ErrorViewModel { Message = message });
            }

            return RedirectToAction("Index", "Error", new ErrorViewModel { Message = $"Status code: {statusCode}" });
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

        public IActionResult Acquiring(ErrorViewModel model)
        {
            return View(model);
        }

        public IActionResult Timeout()
        {
            return View();
        }

        public IActionResult BrokenCircuit()
        {
            return View();
        }
    }
}
