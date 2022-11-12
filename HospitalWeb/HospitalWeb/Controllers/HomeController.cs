using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HospitalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotifier _notifier;

        public HomeController(
            ILogger<HomeController> logger,
            INotifier notifier)
        {
            _logger = logger;
            _notifier = notifier;
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

        public IActionResult Test()
        {
            _notifier.SendMessage("gordeybeatkiller@gmail.com", "Test", "test");
            return Content("ok");
        }
    }
}
