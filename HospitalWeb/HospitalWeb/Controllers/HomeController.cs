using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            _notifier.SendMessage("gordeybeatkiller@gmail.com", "Test", "test");
            return Content("ok");
        }
    }
}
