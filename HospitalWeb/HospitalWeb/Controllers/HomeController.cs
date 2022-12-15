using HospitalWeb.Clients.Implementations;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HospitalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiUnitOfWork _api;

        public HomeController(
            ILogger<HomeController> logger,
            ApiUnitOfWork api)
        {
            _logger = logger;
            _api = api;
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
    }
}
