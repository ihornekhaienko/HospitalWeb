using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HospitalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UnitOfWork _uow;

        public HomeController(
            ILogger<HomeController> logger,
            INotifier notifier,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogCritical("CurrentCulture is {0}.", CultureInfo.CurrentCulture.Name);
            return View();
        }

        [HttpPost]
        public IActionResult Index(string searchString)
        {
            return RedirectToAction("Search", "Doctors", new { searchString = searchString });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult UpdateAppointmentStates()
        {
            _uow.Appointments.UpdateStates();
            return Ok();
        }
    }
}
