using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.WebApi.Clients.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly ApiUnitOfWork _api;

        public StatisticsController(ILogger<StatisticsController> logger, ApiUnitOfWork api)
        {
            _logger = logger;
            _api = api;
        }

        [HttpGet]
        public IActionResult Index(
            DateTime? fromDate,
            DateTime? toDate)
        {
            var builder = new AppointmentsViewModelBuilder(_api, state: 3, fromTime: fromDate, toTime: toDate, pageSize: int.MaxValue);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }
    }
}
