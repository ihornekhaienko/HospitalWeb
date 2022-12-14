using HospitalWeb.Mvc.Filters.Builders.Implementations;
using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.ViewModels.Statistics;
using HospitalWeb.Mvc.Clients.Implementations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalWeb.Mvc.Controllers
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
            int? locality = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var builder = new AppointmentsViewModelBuilder(_api, state: 5, locality: locality, fromTime: fromDate, toTime: toDate, pageSize: int.MaxValue);
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

        [HttpPost]
        public IActionResult Diagnosis([FromBody] List<AppointmentDTO> appointments)
        {
            var rawModel = appointments
                .GroupBy(a => a.Diagnosis)
                .Select(g => new DiagnosesStatViewModel { Label = g.Key, Count = g.Count() })
                .ToList();

            var model = rawModel
                .Take(10)
                .ToList();

            if (rawModel.Count > 10)
            {
                var other = new DiagnosesStatViewModel
                {
                    Label = "Other",
                    Count = rawModel
                        .Skip(10)
                        .Sum(m => m.Count)
                };

                model.Add(other);
            }

            return Json(model);
        }

        [HttpPost]
        public IActionResult Hospital([FromBody] List<AppointmentDTO> appointments)
        {
            var rawModel = appointments
                .GroupBy(a => a.Hospital)
                .Select(g => new HospitalStatViewModel { Label = g.Key, Count = g.Count() })
                .OrderBy(m => m.Count)
                .ToList();

            var model = rawModel
                .Take(10)
                .ToList();

            if (rawModel.Count > 10)
            {
                var other = new HospitalStatViewModel
                {
                    Label = "Other",
                    Count = rawModel
                        .Skip(10)
                        .Sum(m => m.Count)
                };

                model.Add(other);
            }

            return Json(model);
        }

        [HttpPost]
        public IActionResult Specialty([FromBody] List<AppointmentDTO> appointments)
        {
            var rawModel = appointments
                .GroupBy(a => a.DoctorSpecialty)
                .Select(g => new SpecialtyStatViewModel { Label = g.Key, Count = g.Count() })
                .ToList();

            var model = rawModel
                .Take(5)
                .ToList();

            if (rawModel.Count > 5)
            {
                var other = new SpecialtyStatViewModel
                {
                    Label = "Other",
                    Count = rawModel
                        .Skip(5)
                        .Sum(m => m.Count)
                };

                model.Add(other);
            }

            return Json(model);
        }
    }
}
