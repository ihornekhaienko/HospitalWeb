using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Filters.Builders.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HospitalWeb.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiUnitOfWork _api;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApiUnitOfWork api,
            SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _api = api;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
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

        [HttpPost]
        public IActionResult LoadLatestNotifications()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return null;
            }

            var response = _api.AppUsers.Get(User.Identity.Name, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Patients.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var patient = _api.Patients.Read(response);

            var builder = new NotificationsViewModelBuilder(_api, 1, patient.Id, isRead: false, pageSize: 5);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var model = builder.GetViewModel();

            return Json(model.Notifications);
        }
    }
}
