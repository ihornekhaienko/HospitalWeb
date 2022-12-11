using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.ViewModels;
using HospitalWeb.ViewModels.Statistics;
using HospitalWeb.WebApi.Clients.Implementations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var builder = new AppointmentsViewModelBuilder(_api, state: 5, fromTime: fromDate, toTime: toDate, pageSize: int.MaxValue);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Attendance([FromBody] List<AppointmentDTO> appointments)
        {
            var model = appointments
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new AttendanceStatViewModel { VisitDate = g.Key, VisitsCount = g.Count() })
                .ToList();

            return Json(model);
        }
    }
}
